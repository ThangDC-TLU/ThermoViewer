
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThermoViewer;

namespace ThermoViewer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

/*
using System;
using System.Windows.Forms;
using System.Text;
using System.IO;

namespace ThermoViewer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            string exifPath = @"D:\ThucTap_NganGiang\Code\exiftool-13.45_64\exiftool.exe";
            string imagePath = @"D:\ThucTap_NganGiang\Code\anhnhiet H20T\DJI_20231026125429_0025_T.JPG";

            ThermalReader reader = new ThermalReader(exifPath);

            // Lấy toàn bộ thông tin
            string allInfo = reader.GetFullMetadata(imagePath);

            // Lưu vào file cùng thư mục với code để dễ kiểm tra
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FullMetadataLog.txt");
            File.WriteAllText(logPath, allInfo);

            // Thông báo cho người dùng
            string message = $"Đã trích xuất xong!\n\n" +
                             $"Hãy mở file sau để kiểm tra:\n{logPath}\n\n" +
                             $"Dùng Ctrl+F tìm 'Temp', 'Limit', hoặc 'Celsius'.";

            MessageBox.Show(message, "Thông báo");
        }
    }
}
*/