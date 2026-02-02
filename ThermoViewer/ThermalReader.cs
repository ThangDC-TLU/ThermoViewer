using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Json;

namespace ThermoViewer
{
    public class ThermalReader
    {
        private readonly string _exifToolPath;

        public ThermalReader(string customExifToolPath)
        {
            _exifToolPath = customExifToolPath;
        }

        /// <summary>
        /// Lấy toàn bộ metadata dạng text (để debug, xem thông tin).
        /// </summary>
        public string GetFullMetadata(string imagePath)
        {
            return ExecuteExifTool($"-a -u -g1 -S \"{imagePath}\"");
        }

        /// <summary>
        /// Đọc ThermalData nhị phân và nội suy thành ma trận nhiệt độ °C.
        /// Đồng thời log chi tiết các thông tin phục vụ viết báo cáo.
        /// </summary>
        public double[,] GetThermalMatrix(string imagePath, int width = 640, int height = 512)
        {
            Debug.WriteLine("========== THERMAL READER DEBUG ==========");
            Debug.WriteLine($"[INFO] Ảnh đầu vào: {imagePath}");
            Debug.WriteLine($"[INFO] Kích thước dự kiến: width={width}, height={height}");

            // 1. Lấy dải nhiệt độ thực tế từ Metadata (DJI)
            var range = GetDynamicRange(imagePath);
            double realMin = range.Min;
            double realMax = range.Max;

            Debug.WriteLine($"[STEP 1] Dải nhiệt độ (từ metadata): Min={realMin} °C, Max={realMax} °C");

            // 2. Trích xuất file nhị phân thô từ ThermalData
            string binPath = Path.ChangeExtension(imagePath, ".bin");
            Debug.WriteLine($"[STEP 2] Chạy exiftool để trích ThermalData → {binPath}");
            ExecuteExifTool($"-ThermalData -b -w! .bin \"{imagePath}\"");

            if (!File.Exists(binPath))
            {
                Debug.WriteLine("[ERROR] Không tạo được file .bin (ThermalData).");
                return null;
            }

            double[,] matrix = new double[height, width];

            try
            {
                byte[] rawBytes = File.ReadAllBytes(binPath);
                Debug.WriteLine($"[INFO] Đã đọc {rawBytes.Length} bytes từ file .bin");

                // Xóa file tạm sau khi đọc (cố gắng, không bắt buộc)
                try { File.Delete(binPath); } catch { }

                if (rawBytes.Length < width * height * 2)
                {
                    Debug.WriteLine($"[ERROR] rawBytes.Length = {rawBytes.Length}, nhỏ hơn {width * height * 2} (2 bytes/pixel).");
                    return null;
                }

                using (var reader = new BinaryReader(new MemoryStream(rawBytes)))
                {
                    ushort[] allPixels = new ushort[width * height];
                    for (int i = 0; i < allPixels.Length; i++)
                    {
                        // 2 byte / pixel (Little-endian)
                        allPixels[i] = reader.ReadUInt16();
                    }

                    // 2.1. Tìm dải raw
                    ushort rawMin = allPixels.Min();
                    ushort rawMax = allPixels.Max();
                    double rawRange = (rawMax - rawMin == 0) ? 1.0 : (rawMax - rawMin);

                    Debug.WriteLine($"[STEP 3] Dải raw (từ ThermalData): rawMin={rawMin}, rawMax={rawMax}, rawRange={rawRange}");

                    // 2.2. Lấy một vài điểm mẫu để minh họa trong báo cáo
                    int centerX = width / 2;
                    int centerY = height / 2;
                    int topLeftX = 0;
                    int topLeftY = 0;

                    // Điểm (0,0)
                    ushort raw00 = allPixels[topLeftY * width + topLeftX];
                    double t00 = realMin + (double)(raw00 - rawMin) / rawRange * (realMax - realMin);

                    // Điểm (centerX, centerY)
                    ushort rawCenter = allPixels[centerY * width + centerX];
                    double tCenter = realMin + (double)(rawCenter - rawMin) / rawRange * (realMax - realMin);

                    Debug.WriteLine("[STEP 4] Ví dụ giá trị tại một số điểm ảnh:");
                    Debug.WriteLine($"         - Pixel (0,0):      raw={raw00},  T≈{t00:F2} °C");
                    Debug.WriteLine($"         - Pixel ({centerX},{centerY}): raw={rawCenter}, T≈{tCenter:F2} °C");

                    // 3. Nội suy tuyến tính raw → °C cho toàn bộ ma trận
                    Debug.WriteLine("[STEP 5] Bắt đầu nội suy tuyến tính toàn bộ ma trận raw → °C...");

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            ushort currentRaw = allPixels[y * width + x];
                            matrix[y, x] = realMin + (double)(currentRaw - rawMin) / rawRange * (realMax - realMin);
                        }
                    }

                    Debug.WriteLine("[INFO] Hoàn thành nội suy ma trận nhiệt độ °C.");
                }

                Debug.WriteLine("========== END THERMAL READER DEBUG ==========");
                return matrix;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Lỗi nội suy: " + ex.Message);
                Debug.WriteLine("========== END THERMAL READER DEBUG (ERROR) ==========");
                return null;
            }
        }

        /// <summary>
        /// Đọc UserComment (JSON) và trả về dải nhiệt độ: temperature_range.high / low.
        /// </summary>
        public (double Max, double Min) GetDynamicRange(string imagePath)
        {
            string metadata = ExecuteExifTool($"-UserComment -j \"{imagePath}\"");

            double defaultMax = 0;
            double defaultMin = 0;

            if (string.IsNullOrWhiteSpace(metadata))
            {
                Debug.WriteLine("[WARN] GetDynamicRange: metadata rỗng.");
                return (defaultMax, defaultMin);
            }

            try
            {
                // JSON ngoài cùng: mảng 1 phần tử
                using (JsonDocument doc = JsonDocument.Parse(metadata))
                {
                    JsonElement root = doc.RootElement;

                    if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                    {
                        Debug.WriteLine("[WARN] GetDynamicRange: Root không phải mảng hoặc mảng rỗng.");
                        return (defaultMax, defaultMin);
                    }

                    JsonElement obj = root[0];

                    // Trường UserComment (chuỗi JSON lồng bên trong)
                    if (!obj.TryGetProperty("UserComment", out JsonElement userCommentProp))
                    {
                        Debug.WriteLine("[WARN] GetDynamicRange: Không có trường UserComment.");
                        return (defaultMax, defaultMin);
                    }

                    string userCommentStr = userCommentProp.GetString();
                    if (string.IsNullOrWhiteSpace(userCommentStr))
                    {
                        Debug.WriteLine("[WARN] GetDynamicRange: UserComment rỗng.");
                        return (defaultMax, defaultMin);
                    }

                    // Parse JSON bên trong UserComment
                    using (JsonDocument ucDoc = JsonDocument.Parse(userCommentStr))
                    {
                        JsonElement ucRoot = ucDoc.RootElement;

                        if (!ucRoot.TryGetProperty("temperature_range", out JsonElement tempRange))
                        {
                            Debug.WriteLine("[WARN] GetDynamicRange: Không tìm thấy temperature_range trong UserComment.");
                            return (defaultMax, defaultMin);
                        }

                        double max = TryGetDouble(tempRange, "high", defaultMax);
                        double min = TryGetDouble(tempRange, "low", defaultMin);

                        Debug.WriteLine($"[DEBUG] Parsed temperature_range từ metadata DJI: high={max} °C, low={min} °C");

                        return (max, min);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Lỗi parse JSON UserComment: " + ex.Message);
                return (defaultMax, defaultMin);
            }
        }

        /// <summary>
        /// Helper: lấy double từ JsonElement, nếu lỗi trả về defaultValue.
        /// </summary>
        private double TryGetDouble(JsonElement obj, string propertyName, double defaultValue)
        {
            if (obj.TryGetProperty(propertyName, out JsonElement prop))
            {
                if (prop.TryGetDouble(out double value))
                    return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Thực thi exiftool và trả về stdout.
        /// </summary>
        private string ExecuteExifTool(string arguments)
        {
            if (!File.Exists(_exifToolPath))
            {
                Debug.WriteLine("[ERROR] Không tìm thấy exiftool.exe");
                return "Lỗi: Không tìm thấy exiftool.exe";
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _exifToolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Debug.WriteLine("[ERROR] ExecuteExifTool: Không khởi động được process.");
                        return null;
                    }

                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        process.WaitForExit();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Ngoại lệ khi gọi exiftool: " + ex.Message);
                return "Ngoại lệ: " + ex.Message;
            }
        }

        /// <summary>
        /// Xuất toàn bộ giá trị raw (UInt16) từ ThermalData ra file .txt
        /// Mỗi dòng: index, x, y, rawValue
        /// </summary>
        public void ExportRawToText(string imagePath, int width = 640, int height = 512, string outputTxtPath = null)
        {
            Debug.WriteLine("========== EXPORT RAW TO TEXT ==========");
            Debug.WriteLine($"[INFO] Ảnh đầu vào: {imagePath}");

            // 1. Tạo đường dẫn file .bin và .txt mặc định
            string binPath = Path.ChangeExtension(imagePath, ".bin");

            if (string.IsNullOrWhiteSpace(outputTxtPath))
            {
                // Mặc định: cùng thư mục, cùng tên, đuôi .raw.txt
                outputTxtPath = Path.ChangeExtension(imagePath, ".raw.txt");
            }

            string outputDir = Path.GetDirectoryName(outputTxtPath) ?? "";
            Debug.WriteLine($"[INFO] File .bin tạm: {binPath}");
            Debug.WriteLine($"[INFO] File .txt xuất raw: {outputTxtPath}");
            Debug.WriteLine($"[INFO] Thư mục chứa file txt: {outputDir}");

            // 2. Trích xuất ThermalData → .bin
            ExecuteExifTool($"-ThermalData -b -w! .bin \"{imagePath}\"");

            if (!File.Exists(binPath))
            {
                Debug.WriteLine("[ERROR] Không tạo được file .bin (ThermalData).");
                return;
            }

            try
            {
                byte[] rawBytes = File.ReadAllBytes(binPath);
                Debug.WriteLine($"[INFO] Đã đọc {rawBytes.Length} bytes từ file .bin");

                int expectedBytes = width * height * 2;
                if (rawBytes.Length < expectedBytes)
                {
                    Debug.WriteLine($"[WARN] rawBytes.Length = {rawBytes.Length}, nhỏ hơn {expectedBytes} (2 bytes/pixel).");
                    // Vẫn tiếp tục nhưng cảnh báo
                }

                using (var reader = new BinaryReader(new MemoryStream(rawBytes)))
                using (var writer = new StreamWriter(outputTxtPath, false, Encoding.UTF8))
                {
                    // Header cho file txt (để đọc báo cáo cho dễ)
                    writer.WriteLine("# Export raw ThermalData");
                    writer.WriteLine($"# Image: {imagePath}");
                    writer.WriteLine($"# Size: width={width}, height={height}");
                    writer.WriteLine("# Format per line: index; x; y; rawValue");
                    writer.WriteLine();

                    int totalPixels = width * height;
                    for (int i = 0; i < totalPixels; i++)
                    {
                        if (reader.BaseStream.Position + 2 > reader.BaseStream.Length)
                            break;

                        ushort rawValue = reader.ReadUInt16();

                        int y = i / width;
                        int x = i % width;

                        // Mỗi dòng: index; x; y; raw
                        writer.WriteLine($"{i};{x};{y};{rawValue}");
                    }
                }

                Debug.WriteLine("[INFO] Xuất file raw txt thành công.");
                Debug.WriteLine($"[INFO] Đường dẫn file txt: {outputTxtPath}");
                Debug.WriteLine($"[INFO] Thư mục: {outputDir}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Lỗi khi xuất raw txt: " + ex.Message);
            }
            finally
            {
                // Xóa file .bin tạm (nếu muốn giữ lại thì bỏ đoạn này)
                try
                {
                    if (File.Exists(binPath))
                        File.Delete(binPath);
                }
                catch { }

                Debug.WriteLine("========== END EXPORT RAW TO TEXT ==========");
            }
        }
    }
}