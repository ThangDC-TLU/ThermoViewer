using System;
using System.Drawing;
using System.Windows.Forms;

namespace ThermoViewer
{
    public partial class Form1 : Form
    {
        private Bitmap thermalImage;
        private double[,] thermalData; // ma trận nhiệt độ [height, width]
        private ThermalReader reader;
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

        // Pan
        private bool isPanning = false;
        private Point panStartPoint;
        private Point autoScrollStart;

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

            lblInfo.Text = "Chưa có ảnh. Nhấn \"Mở ảnh...\" để chọn ảnh nhiệt.";
            if (statusLabel != null)
                statusLabel.Text = "Sẵn sàng.";
        }

        // NÚT MỞ ẢNH
        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            string filePath = openFileDialog1.FileName;

            try
            {
                if (thermalImage != null)
                    thermalImage.Dispose();

                thermalImage = new Bitmap(filePath);
                pictureBox1.Image = thermalImage;

                reader = new ThermalReader(exifPath);

                // Lấy ma trận nhiệt độ (H20T: 640 x 512)
                thermalData = reader.GetThermalMatrix(filePath, 640, 512);

                // Lấy dải nhiệt độ
                var range = reader.GetDynamicRange(filePath);
                double realMin = range.Min;
                double realMax = range.Max;
                double originMin = range.OriginMin;
                double originMax = range.OriginMax;

                // Reset zoom & ROI
                zoomFactor = 1.0f;
                selectionRect = Rectangle.Empty;

                // Kích thước cơ sở lấy từ ma trận nhiệt (nếu có), để mapping chuẩn
                if (thermalData != null)
                {
                    int baseW = thermalData.GetLength(1); // width
                    int baseH = thermalData.GetLength(0); // height

                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Size = new Size(baseW, baseH);
                }
                else
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Size = thermalImage.Size;
                }

                // Cập nhật thông tin ảnh
                lblFilePath.Text = System.IO.Path.GetFileName(filePath);
                lblResolution.Text = $"{thermalImage.Width} x {thermalImage.Height}";
                lblCamera.Text = "DJI ZH20T";

                // Cập nhật thông tin nhiệt độ tổng
                lblRealMin.Text = $"{realMin:F2} °C";
                lblRealMax.Text = $"{realMax:F2} °C";
                lblOriginMin.Text = $"{originMin:F2} °C";
                lblOriginMax.Text = $"{originMax:F2} °C";

                // Reset nhiệt độ tại chuột & ROI
                lblCursorTemp.Text = "- °C";
                lblRoiMin.Text = "- °C";
                lblRoiMax.Text = "- °C";
                lblRoiAvg.Text = "- °C";

                if (thermalData != null)
                    lblInfo.Text = $"Đã nạp dữ liệu nhiệt độ: {thermalData.GetLength(1)} x {thermalData.GetLength(0)}";
                else
                    lblInfo.Text = "Cảnh báo: Không trích xuất được dữ liệu nhiệt độ thô!";

                if (statusLabel != null)
                    statusLabel.Text = "Đã mở ảnh: " + filePath;
            }
            catch (Exception ex)
            {
                lblInfo.Text = "Lỗi khi mở ảnh hoặc đọc dữ liệu nhiệt.";
                if (statusLabel != null)
                    statusLabel.Text = "Lỗi: " + ex.Message;

                MessageBox.Show(
                    "Không thể mở ảnh hoặc đọc dữ liệu nhiệt.\n\nChi tiết: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ZOOM: Ctrl + lăn chuột
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (thermalImage == null) return;

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
            if (thermalImage == null) return;

            int baseW, baseH;
            if (thermalData != null)
            {
                baseW = thermalData.GetLength(1);
                baseH = thermalData.GetLength(0);
            }
            else
            {
                baseW = thermalImage.Width;
                baseH = thermalImage.Height;
            }

            int newW = (int)(baseW * zoomFactor);
            int newH = (int)(baseH * zoomFactor);

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Size = new Size(newW, newH);
            pictureBox1.Image = thermalImage;

            lblInfo.Text = $"Zoom: {(int)(zoomFactor * 100)}%";
        }

        // MOUSE DOWN
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (thermalImage == null) return;

            // Chuột phải: pan
            if (e.Button == MouseButtons.Right)
            {
                isPanning = true;
                panStartPoint = e.Location;
                autoScrollStart = panelImage.AutoScrollPosition; // giá trị âm
                return;
            }

            // Chuột trái: ROI / điểm
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
            // Pan
            if (isPanning)
            {
                int dx = e.Location.X - panStartPoint.X;
                int dy = e.Location.Y - panStartPoint.Y;

                panelImage.AutoScrollPosition = new Point(
                    -(autoScrollStart.X + dx),
                    -(autoScrollStart.Y + dy)
                );
                return;
            }

            // ROI
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

            if (e.Button == MouseButtons.Right)
            {
                isPanning = false;
                return;
            }

            if (e.Button != MouseButtons.Left)
                return;

            isSelecting = false;

            // CLICK 1 ĐIỂM
            if (selectionRect.Width < 5 || selectionRect.Height < 5)
            {
                Point imgPt = ScreenToImage(e.Location);
                double temp = thermalData[imgPt.Y, imgPt.X];

                // Hiển thị trong grpThermalInfo
                lblCursorTemp.Text = $"{temp:F2} °C";
                lblInfo.Text = $"Điểm ({imgPt.X}, {imgPt.Y})";

                selectionRect = Rectangle.Empty;
            }
            // CHỌN VÙNG ROI
            else
            {
                Point p1 = ScreenToImage(selectionRect.Location);
                Point p2 = ScreenToImage(new Point(selectionRect.Right, selectionRect.Bottom));
                CalculateThermalStats(p1, p2);
            }

            pictureBox1.Invalidate();
        }

        // VẼ KHUNG ROI
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (selectionRect != Rectangle.Empty)
            {
                using (Pen pen = new Pen(Color.Lime, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);
                }
            }
        }

        /// <summary>
        /// Chuyển toạ độ trong pictureBox (đã zoom) sang toạ độ ma trận nhiệt (thermalData).
        /// Ảnh đang Stretch đầy pictureBox1 và base size lấy từ thermalData.
        /// </summary>
        private Point ScreenToImage(Point p)
        {
            if (thermalData == null)
                return Point.Empty;

            int dataW = thermalData.GetLength(1); // 640
            int dataH = thermalData.GetLength(0); // 512

            int pbW = pictureBox1.ClientSize.Width;
            int pbH = pictureBox1.ClientSize.Height;

            if (pbW <= 0 || pbH <= 0)
                return new Point(0, 0);

            // pictureBox1.StretchImage, không căn giữa (ta không dùng offset),
            // nên toạ độ tỉ lệ tuyến tính:
            float scaleX = (float)dataW / pbW;
            float scaleY = (float)dataH / pbH;

            int imgX = (int)(p.X * scaleX);
            int imgY = (int)(p.Y * scaleY);

            imgX = Math.Max(0, Math.Min(dataW - 1, imgX));
            imgY = Math.Max(0, Math.Min(dataH - 1, imgY));

            return new Point(imgX, imgY);
        }

        // TÍNH MIN / MAX / AVG TRONG ROI
        private void CalculateThermalStats(Point p1, Point p2)
        {
            if (thermalData == null) return;

            double min = double.MaxValue;
            double max = double.MinValue;
            double sum = 0;
            int count = 0;

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

            if (count == 0) return;

            double avg = sum / count;

            // Hiển thị vào grpThermalInfo
            lblRoiMin.Text = $"{min:F1} °C";
            lblRoiMax.Text = $"{max:F1} °C";
            lblRoiAvg.Text = $"{avg:F1} °C";
        }
    }
}