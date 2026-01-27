using System;
using System.Drawing;
using System.Windows.Forms;

namespace ThermoViewer
{
    public class ImageCoordinateMapper
    {
        private readonly Func<PictureBox> _getPictureBox;
        private readonly Func<double[,]> _getThermalData;

        public ImageCoordinateMapper(Func<PictureBox> getPictureBox,
                                     Func<double[,]> getThermalData)
        {
            _getPictureBox = getPictureBox;
            _getThermalData = getThermalData;
        }

        public Point ScreenToImage(Point p)
        {
            var data = _getThermalData();
            var pb = _getPictureBox();
            if (data == null || pb == null)
                return Point.Empty;

            int dataW = data.GetLength(1);
            int dataH = data.GetLength(0);

            int pbW = pb.ClientSize.Width;
            int pbH = pb.ClientSize.Height;
            if (pbW <= 0 || pbH <= 0)
                return Point.Empty;

            float scaleX = (float)dataW / pbW;
            float scaleY = (float)dataH / pbH;

            int imgX = (int)(p.X * scaleX);
            int imgY = (int)(p.Y * scaleY);

            imgX = Math.Max(0, Math.Min(dataW - 1, imgX));
            imgY = Math.Max(0, Math.Min(dataH - 1, imgY));

            return new Point(imgX, imgY);
        }

        public Rectangle ImageRectToScreenRect(Rectangle imgRect)
        {
            var data = _getThermalData();
            var pb = _getPictureBox();
            if (data == null || pb == null)
                return Rectangle.Empty;

            int dataW = data.GetLength(1);
            int dataH = data.GetLength(0);

            int pbW = pb.ClientSize.Width;
            int pbH = pb.ClientSize.Height;
            if (pbW <= 0 || pbH <= 0)
                return Rectangle.Empty;

            float scaleX = (float)pbW / dataW;
            float scaleY = (float)pbH / dataH;

            int x = (int)(imgRect.X * scaleX);
            int y = (int)(imgRect.Y * scaleY);
            int w = (int)(imgRect.Width * scaleX);
            int h = (int)(imgRect.Height * scaleY);

            return new Rectangle(x, y, w, h);
        }
    }
}