using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermoViewer
{
    public class ThermalDataProcessor
    {
        // Hàm đọc file nhị phân và chuyển thành mảng nhiệt độ độ C
        public double[,] ExtractThermalMatrix(string binFilePath, int width, int height)
        {
            byte[] rawBytes = File.ReadAllBytes(binFilePath);
            double[,] tempMatrix = new double[height, width];

            using (BinaryReader reader = new BinaryReader(new MemoryStream(rawBytes)))
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        // Đọc số nguyên 16-bit (2 bytes)
                        ushort rawValue = reader.ReadUInt16();

                        // Công thức phổ biến của DJI: Giá trị thô / 10
                        // Bạn có thể kiểm tra lại với Max/Min metadata để khớp tỷ lệ
                        tempMatrix[i, j] = rawValue / 10.0;
                    }
                }
            }
            return tempMatrix;
        }
    }
}
