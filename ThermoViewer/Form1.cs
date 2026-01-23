using System;
using System.Drawing;
using System.Windows.Forms;

namespace ThermoViewer
{
    public partial class Form1 : Form
    {
        private Bitmap thermalImage;
        private double[,] thermalData; // Lưu ma trận nhiệt độ 640x512
        private ThermalReader reader;  // Đối tượng đọc dữ liệu
        private string exifPath = @"D:\ThucTap_NganGiang\Code\exiftool-13.45_64\exiftool.exe";

        // Zoom
        private float zoomFactor = 1.0f;
        private const float ZoomStep = 0.1f;
        private const float MaxZoom = 5.0f;
        private const float MinZoom = 0.2f;

        // ROI
        private bool isSelecting = false;
        private Point startPoint;
        private Rectangle selectionRect = Rectangle.Empty;

        public Form1()
        {
            InitializeComponent();

            // Mouse events
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.Paint += pictureBox1_Paint;

            // Zoom
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            pictureBox1.MouseEnter += (s, e) => pictureBox1.Focus();
        }

        // OPEN IMAGE
        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;

                // 1. Hiển thị ảnh Visual
                if (thermalImage != null) thermalImage.Dispose();
                thermalImage = new Bitmap(filePath);
                pictureBox1.Image = thermalImage;

                // 2. Trích xuất ma trận nhiệt độ thô
                reader = new ThermalReader(exifPath);
                // H20T thường có độ phân giải 640x512
                thermalData = reader.GetThermalMatrix(filePath, 640, 512);

                // Reset giao diện
                zoomFactor = 1.0f;
                pictureBox1.Size = thermalImage.Size;
                selectionRect = Rectangle.Empty;

                if (thermalData != null)
                    lblInfo.Text = $"Đã nạp dữ liệu nhiệt độ: {thermalData.GetLength(1)}x{thermalData.GetLength(0)}";
                else
                    lblInfo.Text = "Cảnh báo: Không trích xuất được dữ liệu nhiệt độ thô!";
            }
        }

        // CTRL + WHEEL ZOOM
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (thermalImage == null) return;

            // Chỉ zoom khi giữ Ctrl
            if ((ModifierKeys & Keys.Control) != Keys.Control)
                return;

            float oldZoom = zoomFactor;

            if (e.Delta > 0)
                zoomFactor += ZoomStep;
            else
                zoomFactor -= ZoomStep;

            zoomFactor = Math.Max(MinZoom, Math.Min(MaxZoom, zoomFactor));

            if (Math.Abs(oldZoom - zoomFactor) < 0.001f)
                return;

            ApplyZoom();
        }

        private void ApplyZoom()
        {
            int newW = (int)(thermalImage.Width * zoomFactor);
            int newH = (int)(thermalImage.Height * zoomFactor);

            pictureBox1.Size = new Size(newW, newH);
            pictureBox1.Image = thermalImage;

            lblInfo.Text = $"Zoom: {(int)(zoomFactor * 100)}%";
        }

        // MOUSE DOWN 
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (thermalImage == null) return;

            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoint = e.Location;
                selectionRect = new Rectangle(e.Location, Size.Empty);
            }
        }

        // MOUSE MOVE 
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting) return;

            int x = Math.Min(startPoint.X, e.X);
            int y = Math.Min(startPoint.Y, e.Y);
            int w = Math.Abs(startPoint.X - e.X);
            int h = Math.Abs(startPoint.Y - e.Y);

            selectionRect = new Rectangle(x, y, w, h);
            pictureBox1.Invalidate();
        }

        // MOUSE UP
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (thermalImage == null || thermalData == null) return;

            isSelecting = false;

            // TRƯỜNG HỢP 1: Click vào 1 điểm (Pixel)
            if (selectionRect.Width < 5 || selectionRect.Height < 5)
            {
                Point imgPt = ScreenToImage(e.Location);
                double temp = thermalData[imgPt.Y, imgPt.X]; // Lấy từ ma trận

                lblInfo.Text = $"Điểm ({imgPt.X}, {imgPt.Y}): {temp:F1} °C";
                selectionRect = Rectangle.Empty;
            }
            // TRƯỜNG HỢP 2: Chọn vùng (ROI)
            else
            {
                Point p1 = ScreenToImage(selectionRect.Location);
                Point p2 = ScreenToImage(new Point(selectionRect.Right, selectionRect.Bottom));

                CalculateThermalStats(p1, p2);
            }

            pictureBox1.Invalidate();
        }

        // DRAW ROI
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (selectionRect != Rectangle.Empty)
            {
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);
                }
            }
        }

        // COORDINATE MAP
        private Point ScreenToImage(Point p)
        {
            int x = (int)(p.X / zoomFactor);
            int y = (int)(p.Y / zoomFactor);

            x = Math.Max(0, Math.Min(thermalImage.Width - 1, x));
            y = Math.Max(0, Math.Min(thermalImage.Height - 1, y));

            return new Point(x, y);
        }

        private void CalculateThermalStats(Point p1, Point p2)
        {
            if (thermalData == null) return;

            double min = double.MaxValue;
            double max = double.MinValue;
            double sum = 0;
            int count = 0;

            // Đảm bảo p1 là góc trên trái, p2 là góc dưới phải
            int startX = Math.Min(p1.X, p2.X);
            int endX = Math.Max(p1.X, p2.X);
            int startY = Math.Min(p1.Y, p2.Y);
            int endY = Math.Max(p1.Y, p2.Y);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    double val = thermalData[y, x];
                    if (val < min) min = val;
                    if (val > max) max = val;
                    sum += val;
                    count++;
                }
            }

            double avg = sum / count;
            lblInfo.Text = $"ROI: Min {min:F1}°C | Max {max:F1}°C | Avg {avg:F1}°C";
        }
    }
}
