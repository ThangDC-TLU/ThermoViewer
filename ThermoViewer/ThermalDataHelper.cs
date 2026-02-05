using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThermoViewer
{
    public static class ThermalDataHelper
    {
        /// <summary>
        /// Trích xuất trường "ThermalData" (Binary data 655360 bytes, use -b option to extract)
        /// từ file ảnh DJI và lưu ra file nhị phân đúng nguyên trạng.
        /// Không giải mã, không đổi endianness, không nội suy.
        /// </summary>
        /// <param name="exifToolPath">Đường dẫn exiftool.exe</param>
        /// <param name="imagePath">Đường dẫn file ảnh nhiệt DJI (*.JPG)</param>
        /// <param name="outputBinPath">
        /// Đường dẫn file nhị phân để lưu.
        /// Nếu null/empty, sẽ mặc định là cùng tên ảnh, đuôi ".thermal.bin"
        /// </param>
        public static void ExportThermalDataRaw(
            string exifToolPath,
            string imagePath,
            string outputBinPath = null)
        {
            if (string.IsNullOrWhiteSpace(exifToolPath) || !File.Exists(exifToolPath))
                throw new FileNotFoundException("Không tìm thấy exiftool.exe", exifToolPath);

            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                throw new FileNotFoundException("Không tìm thấy file ảnh", imagePath);

            if (string.IsNullOrWhiteSpace(outputBinPath))
            {
                // Ví dụ: DJI_20231026125429_0025_T.JPG -> DJI_20231026125429_0025_T.thermal.bin
                outputBinPath = Path.ChangeExtension(imagePath, ".thermal.bin");
            }

            // Gọi exiftool với -ThermalData -b để lấy đúng Binary data gốc ra stdout
            var startInfo = new ProcessStartInfo
            {
                FileName = exifToolPath,
                Arguments = $"-ThermalData -b \"{imagePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                    throw new InvalidOperationException("Không khởi động được exiftool.");

                // ThermalData là nhị phân, nên đọc stdout dạng stream byte
                using (var output = new MemoryStream())
                {
                    process.StandardOutput.BaseStream.CopyTo(output);
                    process.WaitForExit();

                    byte[] thermalBytes = output.ToArray();

                    // Ghi ra đúng nguyên trạng
                    File.WriteAllBytes(outputBinPath, thermalBytes);

                    Debug.WriteLine($"[ThermalDataHelper] Đã xuất ThermalData raw: {outputBinPath}");
                    Debug.WriteLine($"[ThermalDataHelper] Số byte: {thermalBytes.Length}");
                }

                string err = process.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(err))
                {
                    Debug.WriteLine("[ThermalDataHelper] exiftool stderr: " + err);
                }
            }
        }
    }
}