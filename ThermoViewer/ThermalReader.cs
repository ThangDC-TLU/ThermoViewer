using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Globalization;
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
        /// </summary>
        public double[,] GetThermalMatrix(string imagePath, int width = 640, int height = 512)
        {
            // 1. Lấy dải nhiệt độ thực tế từ Metadata
            var range = GetDynamicRange(imagePath);
            double realMin = range.Min;
            double realMax = range.Max;
            double originMin = range.OriginMin;
            double originMax = range.OriginMax;

            Debug.WriteLine($"[DEBUG] Metadata Range: Min={realMin}, Max={realMax}, OriginMin={originMin}, OriginMax={originMax}");

            // 2. Trích xuất file nhị phân thô từ ThermalData
            string binPath = Path.ChangeExtension(imagePath, ".bin");
            ExecuteExifTool($"-ThermalData -b -w! .bin \"{imagePath}\"");

            if (!File.Exists(binPath))
            {
                Debug.WriteLine("LỖI: Không tạo được file .bin");
                return null;
            }

            double[,] matrix = new double[height, width];

            try
            {
                byte[] rawBytes = File.ReadAllBytes(binPath);

                // Xóa file tạm sau khi đọc (cố gắng, không bắt buộc)
                try { File.Delete(binPath); } catch { }

                if (rawBytes.Length < width * height * 2)
                {
                    Debug.WriteLine($"[DEBUG] rawBytes.Length = {rawBytes.Length}, nhỏ hơn {width * height * 2}");
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

                    // Tìm dải raw
                    ushort rawMin = allPixels.Min();
                    ushort rawMax = allPixels.Max();
                    double rawRange = (rawMax - rawMin == 0) ? 1.0 : (rawMax - rawMin);

                    Debug.WriteLine($"[DEBUG] Raw Range: {rawMin} to {rawMax}");

                    // 3. Nội suy tuyến tính raw → °C
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            ushort currentRaw = allPixels[y * width + x];
                            matrix[y, x] = realMin + (double)(currentRaw - rawMin) / rawRange * (realMax - realMin);
                        }
                    }
                }

                return matrix;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi nội suy: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Đọc UserComment (JSON) và trả về dải nhiệt độ:
        /// temperature_range.high / low / originMax / originMin.
        /// </summary>
        public (double Max, double Min, double OriginMax, double OriginMin) GetDynamicRange(string imagePath)
        {
            string metadata = ExecuteExifTool($"-UserComment -j \"{imagePath}\"");

            double defaultMax = 60;
            double defaultMin = 23;
            double defaultOriginMax = 60;
            double defaultOriginMin = 23;

            if (string.IsNullOrWhiteSpace(metadata))
                return (defaultMax, defaultMin, defaultOriginMax, defaultOriginMin);

            try
            {
                // JSON ngoài cùng: mảng 1 phần tử
                using (JsonDocument doc = JsonDocument.Parse(metadata))
                {
                    JsonElement root = doc.RootElement;

                    if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                        return (defaultMax, defaultMin, defaultOriginMax, defaultOriginMin);

                    JsonElement obj = root[0];

                    // Trường UserComment (chuỗi JSON lồng bên trong)
                    JsonElement userCommentProp;
                    if (!obj.TryGetProperty("UserComment", out userCommentProp))
                        return (defaultMax, defaultMin, defaultOriginMax, defaultOriginMin);

                    string userCommentStr = userCommentProp.GetString();
                    if (string.IsNullOrWhiteSpace(userCommentStr))
                        return (defaultMax, defaultMin, defaultOriginMax, defaultOriginMin);

                    // Parse JSON bên trong UserComment
                    using (JsonDocument ucDoc = JsonDocument.Parse(userCommentStr))
                    {
                        JsonElement ucRoot = ucDoc.RootElement;

                        JsonElement tempRange;
                        if (!ucRoot.TryGetProperty("temperature_range", out tempRange))
                            return (defaultMax, defaultMin, defaultOriginMax, defaultOriginMin);

                        double max = TryGetDouble(tempRange, "high", defaultMax);
                        double min = TryGetDouble(tempRange, "low", defaultMin);
                        double originMax = TryGetDouble(tempRange, "originMax", defaultOriginMax);
                        double originMin = TryGetDouble(tempRange, "originMin", defaultOriginMin);

                        Debug.WriteLine(
                            $"[DEBUG] Parsed temperature_range: high={max}, low={min}, originMax={originMax}, originMin={originMin}");

                        return (max, min, originMax, originMin);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi parse JSON UserComment: " + ex.Message);
                return (defaultMax, defaultMin, defaultOriginMax, defaultOriginMin);
            }
        }

        /// <summary>
        /// Helper: lấy double từ JsonElement, nếu lỗi trả về defaultValue.
        /// </summary>
        private double TryGetDouble(JsonElement obj, string propertyName, double defaultValue)
        {
            JsonElement prop;
            if (obj.TryGetProperty(propertyName, out prop))
            {
                double value;
                if (prop.TryGetDouble(out value))
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
                return "Lỗi: Không tìm thấy exiftool.exe";

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
                    if (process == null) return null;

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
                return "Ngoại lệ: " + ex.Message;
            }
        }
    }
}