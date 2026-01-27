using System;
using System.Drawing;
using System.Windows.Forms;

namespace ThermoViewer
{
    public partial class Form1 : Form
    {
        private Bitmap thermalImage;
        private double[,] thermalData; // [height,width]
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

        // Helpers
        private ImageCoordinateMapper _mapper;
        private ClusterManager _clusterManager;

        public Form1()
        {
            InitializeComponent();

            _mapper = new ImageCoordinateMapper(() => pictureBox1, () => thermalData);
            _clusterManager = new ClusterManager(_mapper, () => (double)numClusterThreshold.Value);

            // Mouse events
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.Paint += pictureBox1_Paint;

            // Zoom
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            pictureBox1.MouseEnter += (s, e) => pictureBox1.Focus();

            // UI events
            chkShowClusters.CheckedChanged += chkShowClusters_CheckedChanged;
            numClusterThreshold.ValueChanged += numClusterThreshold_ValueChanged;

            lblInfo.Text = "Chưa có ảnh. Nhấn \"Mở ảnh...\" để chọn ảnh nhiệt.";
            if (statusLabel != null)
                statusLabel.Text = "Sẵn sàng.";
        }

        // ================== MỞ ẢNH ==================
        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            string filePath = openFileDialog1.FileName;

            try
            {
                thermalImage?.Dispose();
                thermalImage = new Bitmap(filePath);
                pictureBox1.Image = thermalImage;

                reader = new ThermalReader(exifPath);
                thermalData = reader.GetThermalMatrix(filePath, 640, 512);

                var range = reader.GetDynamicRange(filePath);
                double realMin = range.Min;
                double realMax = range.Max;

                // Reset
                zoomFactor = 1.0f;
                selectionRect = Rectangle.Empty;
                _clusterManager.Reset();

                // Đồng bộ size pictureBox theo thermalData
                if (thermalData != null)
                {
                    int w = thermalData.GetLength(1);
                    int h = thermalData.GetLength(0);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Size = new Size(w, h);
                }
                else
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Size = thermalImage.Size;
                }

                // Info ảnh
                lblFilePath.Text = System.IO.Path.GetFileName(filePath);
                lblResolution.Text = $"{thermalImage.Width} x {thermalImage.Height}";
                lblRealMin.Text = $"{realMin:F2} °C";
                lblRealMax.Text = $"{realMax:F2} °C";

                // Reset panel phải
                lblCursorTemp.Text = "- °C";
                lblRoiMin.Text = "- °C";
                lblRoiMax.Text = "- °C";
                lblRoiAvg.Text = "- °C";

                lblInfo.Text = thermalData != null
                    ? $"Đã nạp dữ liệu nhiệt độ: {thermalData.GetLength(1)} x {thermalData.GetLength(0)}"
                    : "Cảnh báo: Không trích xuất được dữ liệu nhiệt độ thô!";

                statusLabel.Text = "Đã mở ảnh: " + filePath;
            }
            catch (Exception ex)
            {
                lblInfo.Text = "Lỗi khi mở ảnh hoặc đọc dữ liệu nhiệt.";
                statusLabel.Text = "Lỗi: " + ex.Message;

                MessageBox.Show(
                    "Không thể mở ảnh hoặc đọc dữ liệu nhiệt.\n\nChi tiết: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ================== ZOOM ==================
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (thermalImage == null) return;
            if ((ModifierKeys & Keys.Control) != Keys.Control)
                return;

            float oldZoom = zoomFactor;
            zoomFactor += (e.Delta > 0 ? ZoomStep : -ZoomStep);
            zoomFactor = Math.Max(MinZoom, Math.Min(MaxZoom, zoomFactor));
            if (Math.Abs(oldZoom - zoomFactor) < 0.001f) return;

            ApplyZoom();
        }

        private void ApplyZoom()
        {
            if (thermalImage == null) return;

            int baseW = thermalData != null ? thermalData.GetLength(1) : thermalImage.Width;
            int baseH = thermalData != null ? thermalData.GetLength(0) : thermalImage.Height;

            int newW = (int)(baseW * zoomFactor);
            int newH = (int)(baseH * zoomFactor);

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Size = new Size(newW, newH);
            pictureBox1.Image = thermalImage;

            lblInfo.Text = $"Zoom: {(int)(zoomFactor * 100)}%";
        }

        // ================== MOUSE DOWN ==================
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (thermalImage == null) return;

            if (e.Button == MouseButtons.Right)
            {
                isPanning = true;
                panStartPoint = e.Location;
                autoScrollStart = panelImage.AutoScrollPosition;
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoint = e.Location;
                selectionRect = new Rectangle(e.Location, Size.Empty);
            }
        }

        // ================== MOUSE MOVE ==================
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // KHÔNG hiển thị nhiệt độ tại chuột ở đây (chỉ click mới cập nhật lblCursorTemp)

            // Pan
            if (isPanning)
            {
                int dx = e.Location.X - panStartPoint.X;
                int dy = e.Location.Y - panStartPoint.Y;

                panelImage.AutoScrollPosition = new Point(
                    -(autoScrollStart.X + dx),
                    -(autoScrollStart.Y + dy));
                return;
            }

            // Đang kéo ROI
            if (isSelecting)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int w = Math.Abs(startPoint.X - e.X);
                int h = Math.Abs(startPoint.Y - e.Y);

                selectionRect = new Rectangle(x, y, w, h);
                pictureBox1.Invalidate();
                return;
            }

            // HOVER TRONG CLUSTER – ưu tiên vùng nhỏ nhất chứa điểm
            _clusterManager.HandleHover(
                e.Location,
                chkShowClusters.Checked,
                lblInfo);

            pictureBox1.Invalidate();
        }

        // ================== MOUSE UP ==================
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
                Point imgPt = _mapper.ScreenToImage(e.Location);
                double temp = thermalData[imgPt.Y, imgPt.X];

                lblCursorTemp.Text = $"{temp:F2} °C";
                lblInfo.Text = $"Điểm ({imgPt.X}, {imgPt.Y}): {temp:F2} °C";

                selectionRect = Rectangle.Empty;
            }
            // CHỌN ROI
            else
            {
                Point p1 = _mapper.ScreenToImage(selectionRect.Location);
                Point p2 = _mapper.ScreenToImage(
                    new Point(selectionRect.Right, selectionRect.Bottom));

                _clusterManager.SetLastRoi(p1, p2);

                var stats = _clusterManager.CalculateRoiStats(thermalData, p1, p2);
                if (stats.HasValue)
                {
                    lblRoiMin.Text = $"{stats.Value.Min:F1} °C";
                    lblRoiMax.Text = $"{stats.Value.Max:F1} °C";
                    lblRoiAvg.Text = $"{stats.Value.Avg:F1} °C";
                    lblInfo.Text = $"ROI: Min {stats.Value.Min:F1}°C | Max {stats.Value.Max:F1}°C | Avg {stats.Value.Avg:F1}°C";
                }

                if (chkShowClusters.Checked)
                    _clusterManager.BuildClustersForRoi(thermalData, lblInfo);
                else
                    _clusterManager.ClearClusters();
            }

            pictureBox1.Invalidate();
        }

        // ================== PAINT ==================
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // ROI
            if (selectionRect != Rectangle.Empty)
            {
                using (Pen pen = new Pen(Color.Lime, 2))
                    e.Graphics.DrawRectangle(pen, selectionRect);
            }

            // Clusters
            _clusterManager.DrawClusters(e.Graphics, chkShowClusters.Checked);
        }

        // ================== CHECKBOX & NUMERIC ==================
        private void chkShowClusters_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShowClusters.Checked)
            {
                _clusterManager.ClearClusters();
                pictureBox1.Invalidate();
                lblInfo.Text = "Tắt hiển thị vùng.";
                return;
            }

            if (_clusterManager.HasLastRoi)
            {
                _clusterManager.BuildClustersForRoi(thermalData, lblInfo);
                pictureBox1.Invalidate();
            }
            else
            {
                lblInfo.Text = "Hãy kéo một ROI để tạo vùng nhiệt.";
            }
        }

        private void numClusterThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (!chkShowClusters.Checked) return;
            if (!_clusterManager.HasLastRoi) return;

            _clusterManager.BuildClustersForRoi(thermalData, lblInfo);
            pictureBox1.Invalidate();
        }
    }
}