using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ThermoViewer
{
    /// <summary>
    /// Đọc thông số môi trường (khoảng cách, độ ẩm, phát xạ, T phản xạ, T môi trường)
    /// từ metadata của ảnh DJI, dùng chung cho Form1 và ThermalReader.
    /// </summary>
    public class DjiEnvironmentReader
    {
        public class EnvironmentInfo
        {
            public double DistanceMeters { get; set; }        // Khoảng cách (m)
            public double HumidityPercent { get; set; }       // Độ ẩm (%)
            public double Emissivity { get; set; }            // Độ phát xạ (0–1)
            public double ReflectedTempCelsius { get; set; }  // Nhiệt độ phản xạ (°C)
            public double AmbientTempCelsius { get; set; }    // Nhiệt độ môi trường (°C)
        }

        private readonly Func<string, string> _getFullMetadata;

        /// <summary>
        /// Nhận vào 1 hàm delegate để lấy full metadata text.
        /// Ví dụ: (path) => thermalReader.GetFullMetadata(path)
        /// hoặc dùng trực tiếp exiftool wrapper khác.
        /// </summary>
        public DjiEnvironmentReader(Func<string, string> getFullMetadata)
        {
            _getFullMetadata = getFullMetadata;
        }

        public EnvironmentInfo LoadEnvironmentInfo(string imagePath)
        {
            string fullMeta = _getFullMetadata(imagePath);
            if (string.IsNullOrWhiteSpace(fullMeta))
                return null;

            var env = new EnvironmentInfo();

            // 1. Lấy từ block DJI
            env.DistanceMeters = TryMatchDouble(fullMeta, @"^ObjectDistance:\s*([0-9\.\-]+)", 5.0);
            env.HumidityPercent = TryMatchDouble(fullMeta, @"^RelativeHumidity:\s*([0-9\.\-]+)", 70.0);

            double emissivityPercent = TryMatchDouble(fullMeta, @"^Emissivity:\s*([0-9\.\-]+)", 100.0);
            env.Emissivity = emissivityPercent / 100.0; // 100 -> 1.00

            double reflection10 = TryMatchDouble(fullMeta, @"^Reflection:\s*([0-9\.\-]+)", 230.0);
            env.ReflectedTempCelsius = reflection10 / 10.0;  // 230 -> 23.0 °C

            env.AmbientTempCelsius = TryMatchDouble(fullMeta, @"^AmbientTemperature:\s*([0-9\.\-]+)", 21.0);

            // 2. (Tùy chọn) override từ UserComment.measurement_params (JSON)
            try
            {
                var m = Regex.Match(fullMeta, @"^UserComment:\s*(\{.*\})", RegexOptions.Multiline);
                if (m.Success)
                {
                    string json = m.Groups[1].Value;
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("measurement_params", out JsonElement mp))
                        {
                            if (mp.TryGetProperty("distance", out JsonElement d) && d.TryGetDouble(out double dist))
                                env.DistanceMeters = dist;

                            if (mp.TryGetProperty("humidity", out JsonElement h) && h.TryGetDouble(out double hum))
                                env.HumidityPercent = hum;

                            if (mp.TryGetProperty("emissivity", out JsonElement e) && e.TryGetDouble(out double eps))
                                env.Emissivity = eps; // JSON đã là 0–1

                            if (mp.TryGetProperty("reflection", out JsonElement r) && r.TryGetDouble(out double refl))
                                env.ReflectedTempCelsius = refl; // JSON là °C

                            if (mp.TryGetProperty("ambient_temp", out JsonElement a) && a.TryGetDouble(out double amb))
                            {
                                // metadata mẫu ambient_temp = 0, chỉ ghi đè nếu có giá trị khác 0
                                if (Math.Abs(amb) > 0.01)
                                    env.AmbientTempCelsius = amb;
                            }
                        }
                    }
                }
            }
            catch
            {
                // nếu parse JSON lỗi, giữ nguyên giá trị từ block DJI
            }

            return env;
        }

        private double TryMatchDouble(string text, string pattern, double defaultValue)
        {
            var m = Regex.Match(text, pattern, RegexOptions.Multiline);
            if (!m.Success) return defaultValue;

            if (double.TryParse(
                    m.Groups[1].Value,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double v))
                return v;

            return defaultValue;
        }
    }
}