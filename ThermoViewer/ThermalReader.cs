using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;

namespace ThermoViewer
{
    public class ThermalReader
    {
        private readonly string _exifToolPath;

        public ThermalReader(string customExifToolPath)
        {
            _exifToolPath = customExifToolPath;
        }

        public string GetFullMetadata(string imagePath)
        {
            return ExecuteExifTool($"-a -u -g1 -S \"{imagePath}\"");
        }

        public double[,] GetThermalMatrix(string imagePath, int width = 640, int height = 512)
        {
            // 1. Lấy dải nhiệt độ thực tế từ Metadata
            var range = GetDynamicRange(imagePath);
            double realMin = range.Min;
            double realMax = range.Max;

            // Debug giá trị trích xuất được
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Metadata Range: Min={realMin}, Max={realMax}");

            // 2. Trích xuất file nhị phân thô
            string binPath = Path.ChangeExtension(imagePath, ".bin");
            ExecuteExifTool($"-ThermalData -b -w! .bin \"{imagePath}\"");

            if (!File.Exists(binPath))
            {
                System.Diagnostics.Debug.WriteLine("LỖI: Không tạo được file .bin");
                return null;
            }

            double[,] matrix = new double[height, width];
            try
            {
                byte[] rawBytes = File.ReadAllBytes(binPath);
                if (File.Exists(binPath)) File.Delete(binPath); // Xóa file tạm ngay sau khi đọc

                if (rawBytes.Length < width * height * 2) return null;

                using (BinaryReader reader = new BinaryReader(new MemoryStream(rawBytes)))
                {
                    ushort[] allPixels = new ushort[width * height];
                    for (int i = 0; i < allPixels.Length; i++)
                    {
                        // Đọc 2 bytes (Little-endian) cho mỗi pixel
                        allPixels[i] = reader.ReadUInt16();
                    }

                    // Tìm giá trị thô (Raw) Min/Max trong mảng
                    ushort rawMin = allPixels.Min();
                    ushort rawMax = allPixels.Max();
                    double rawRange = (rawMax - rawMin == 0) ? 1.0 : (rawMax - rawMin);

                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Raw Range: {rawMin} to {rawMax}");

                    // 3. Nội suy tuyến tính để đưa về độ C
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ushort currentRaw = allPixels[i * width + j];
                            // Công thức ánh xạ từ dải Raw sang dải Nhiệt độ thực tế
                            matrix[i, j] = realMin + (double)(currentRaw - rawMin) / rawRange * (realMax - realMin);
                        }
                    }
                }
                return matrix;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi nội suy: " + ex.Message);
                return null;
            }
        }

        private (double Max, double Min) GetDynamicRange(string imagePath)
        {
            // Sử dụng -j để lấy JSON giúp parse giá trị an toàn hơn
            string metadata = ExecuteExifTool($"-UserComment -j \"{imagePath}\"");

            // Giá trị mặc định nếu ảnh DJI ZH20T mẫu
            double max = 60;
            double min = 23;

            if (string.IsNullOrEmpty(metadata)) return (max, min);

            try
            {
                // Parse giá trị từ chuỗi JSON
                double dMax = ParseValue(metadata, "\"high\":");
                double dMin = ParseValue(metadata, "\"low\":");

                // Nếu parse thành công (khác 0) thì trả về giá trị đó
                return (dMax != 0 && dMin != 0) ? (dMax, dMin) : (max, min);
            }
            catch
            {
                return (max, min);
            }
        }

        private double ParseValue(string text, string key)
        {
            int start = text.IndexOf(key);
            if (start == -1) return 0;

            start += key.Length;

            // SỬA LỖI: Thay FindAny bằng IndexOfAny để tương thích .NET Framework
            int end = text.IndexOfAny(new char[] { ',', '}', ' ' }, start);
            if (end == -1) return 0;

            string val = text.Substring(start, end - start).Replace("\"", "").Trim();

            // Chuyển đổi sang double, đảm bảo dùng InvariantCulture để nhận diện dấu chấm thập phân
            if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return 0;
        }

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