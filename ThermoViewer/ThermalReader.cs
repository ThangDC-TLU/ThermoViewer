using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ThermoViewer
{
    public class ThermalReader
    {
        private readonly string _exifToolPath;
        private DjiEnvironmentReader _envReader;

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
            Debug.WriteLine("========== THERMAL READER (PHYSICS MODEL) ==========");
            Debug.WriteLine($"[INFO] Ảnh đầu vào: {imagePath}");

            // 1. Lấy dải nhiệt độ hiển thị từ Metadata (Đáp án chuẩn của DJI)
            var range = GetDynamicRange(imagePath);
            double realMin = range.Min;
            double realMax = range.Max;
            Debug.WriteLine($"[STEP 1] Dải nhiệt độ (Metadata): Min={realMin} °C, Max={realMax} °C");

            // 2. Tự động lấy tham số môi trường từ ảnh
            double dist, hum, emiss, refl;
            GetEnvironmentalParams(imagePath, out dist, out hum, out emiss, out refl);

            // 3. Tính toán Hệ số Khuếch đại Vật lý (Physical Gain)
            // Đây là bước thay thế con số 2.5 bằng công thức chuẩn: G = 1 / (Tau * E)
            double physicalGain = CalculatePhysicalGain(dist, hum, emiss);
            
            // Lấy Tau riêng để log cho vui
            double tau = CalculateTransmission(dist, hum);
            Debug.WriteLine($"[PHYSICS] Dist={dist}m, Hum={hum*100}%, Emiss={emiss}");
            Debug.WriteLine($"[PHYSICS] Transmission (Tau)={tau:F4}, Physical Gain={physicalGain:F4}");

            // 4. Trích xuất file nhị phân thô
            string binPath = Path.ChangeExtension(imagePath, ".bin");
            ExecuteExifTool($"-ThermalData -b -w! .bin \"{imagePath}\"");

            if (!File.Exists(binPath))
            {
                Debug.WriteLine("[ERROR] Không tạo được file .bin.");
                return null;
            }

            double[,] matrix = new double[height, width];

            try
            {
                byte[] rawBytes = File.ReadAllBytes(binPath);
                try { File.Delete(binPath); } catch { } // Dọn dẹp

                if (rawBytes.Length < width * height * 2) return null;

                ushort[] allPixels = new ushort[width * height];
                using (var reader = new BinaryReader(new MemoryStream(rawBytes)))
                {
                    for (int i = 0; i < allPixels.Length; i++)
                        allPixels[i] = reader.ReadUInt16();
                }

                // 5. LỌC NHIỄU (PERCENTILE FILTERING)
                // Loại bỏ 0.05% điểm thấp nhất và cao nhất (điểm chết/nhiễu)
                var sorted = allPixels.OrderBy(p => p).ToArray();
                ushort validRawMin = sorted[(int)(sorted.Length * 0.0005)];
                ushort validRawMax = sorted[(int)(sorted.Length * 0.9995)];
                double rawRange = validRawMax - validRawMin;

                Debug.WriteLine($"[STEP 3] Dải Raw lọc nhiễu: {validRawMin} -> {validRawMax} (Range: {rawRange})");

                // 6. VÒNG LẶP TÍNH TOÁN CHÍNH
                double tempRange = realMax - realMin;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        ushort currentRaw = allPixels[y * width + x];

                        // Clamp giá trị để tránh tràn biên
                        if (currentRaw < validRawMin) currentRaw = validRawMin;
                        if (currentRaw > validRawMax) currentRaw = validRawMax;

                        // a. Tính vị trí tương đối (0.0 -> 1.0)
                        double p = (double)(currentRaw - validRawMin) / rawRange;

                        // b. Tính nhiệt độ tuyến tính cơ bản (Linear Temp)
                        double linearTemp = realMin + (p * tempRange);

                        // c. Bù trừ Vật lý (Radiometric Correction)
                        // Logic: DeltaT_Thực = DeltaT_TuyếnTính * Gain
                        // Ta nhân độ chênh nhiệt (so với nền) với Gain để bù lại năng lượng bị mất
                        double deltaT = linearTemp - realMin;
                        double correctedDeltaT = deltaT * physicalGain;

                        // Cộng thêm Planck Shape Factor (khoảng 1.1-1.2) để mô phỏng đường cong
                        double finalTemp = realMin + (correctedDeltaT * 1.1); 

                        matrix[y, x] = finalTemp;
                    }
                }

                // Debug kiểm tra điểm mẫu
                int idx = 1 * width + 378;
                Debug.WriteLine($"[TEST] (378,1) Final Result: {matrix[1, 378]:F2} °C");

                return matrix;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tính Hệ số Khuếch đại Vật lý (Physical Gain Factor).
        /// Công thức: G = 1 / (Tau * Emissivity)
        /// </summary>
        private double CalculatePhysicalGain(double distMeters, double humidityPercent, double emissivity)
        {
            double tau = CalculateTransmission(distMeters, humidityPercent);

            // Bảo vệ giá trị đầu vào
            if (tau < 0.1) tau = 0.1;       
            if (emissivity < 0.1) emissivity = 0.1; 

            // Gain = 1 / (Tau * Emissivity)
            double gain = 1.0 / (tau * emissivity);

            // Giới hạn Gain để tránh nhiễu quá mức (Max 3x)
            if (gain > 3.0) gain = 3.0;

            return gain;
        }

        /// <summary>
        /// Tính Tau theo mô hình Passman-Larmore (LWIR)
        /// </summary>
        private double CalculateTransmission(double dist, double hum)
        {
            double h = (hum > 1.0) ? hum / 100.0 : hum; // Chuẩn hóa về 0.0 - 1.0
            const double ALPHA_DRY = 0.0066;
            const double BETA_WET = 0.0126;

            double omega = Math.Sqrt(dist) * (ALPHA_DRY + BETA_WET * Math.Sqrt(h));
            double tau = Math.Exp(-omega);

            if (tau < 0.2) return 0.2;
            if (tau > 1.0) return 1.0;
            return tau;
        }

        /// <summary>
        /// Lấy và chuẩn hóa tham số môi trường từ Metadata
        /// </summary>
        private void GetEnvironmentalParams(string imagePath, out double dist, out double hum, out double emiss, out double refl)
        {
            // Giá trị mặc định an toàn
            dist = 5.0; hum = 0.40; emiss = 1.0; refl = 23.0;

            string output = ExecuteExifTool($"-ObjectDistance -RelativeHumidity -Emissivity -Reflection -n -S \"{imagePath}\"");
            if (string.IsNullOrEmpty(output)) return;

            dist = ParseTagValue(output, "ObjectDistance", dist);

            double hVal = ParseTagValue(output, "RelativeHumidity", hum * 100);
            hum = hVal / 100.0; // DJI lưu 70 -> 0.7

            double eVal = ParseTagValue(output, "Emissivity", emiss * 100);
            emiss = eVal / 100.0; // DJI lưu 100 -> 1.0

            double rVal = ParseTagValue(output, "Reflection", refl);
            if (rVal > 100) refl = rVal / 10.0; // DJI lưu 230 -> 23.0
            else refl = rVal;
        }

        private double ParseTagValue(string text, string tag, double defaultVal)
        {
            var m = Regex.Match(text, $@"{tag}\s*:\s*([0-9\.]+)", RegexOptions.IgnoreCase);
            return (m.Success && double.TryParse(m.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double v)) ? v : defaultVal;
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