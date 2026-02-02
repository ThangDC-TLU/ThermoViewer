using System.Windows.Forms;
using System.Drawing;

namespace ThermoViewer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnOpenImage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.GroupBox grpImageInfo;
        private System.Windows.Forms.GroupBox grpThermalInfo;
        private System.Windows.Forms.Label lblFilePathTitle;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label lblResolutionTitle;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.Label lblRealMinTitle;
        private System.Windows.Forms.Label lblRealMin;
        private System.Windows.Forms.Label lblRealMaxTitle;
        private System.Windows.Forms.Label lblRealMax;
        private System.Windows.Forms.Label lblCursorTempTitle;
        private System.Windows.Forms.Label lblCursorTemp;
        private System.Windows.Forms.Label lblRoiMinTitle;
        private System.Windows.Forms.Label lblRoiMin;
        private System.Windows.Forms.Label lblRoiMaxTitle;
        private System.Windows.Forms.Label lblRoiMax;
        private System.Windows.Forms.Label lblRoiAvgTitle;
        private System.Windows.Forms.Label lblRoiAvg;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;

        // Job2 UI
        private System.Windows.Forms.CheckBox chkShowClusters;
        private System.Windows.Forms.NumericUpDown numClusterThreshold;
        private System.Windows.Forms.Label lblClusterThresholdTitle;

        // ====== Thông số môi trường ======
        private System.Windows.Forms.GroupBox grpEnvInfo;
        private System.Windows.Forms.Label lblDistanceTitle;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Label lblHumidityTitle;
        private System.Windows.Forms.Label lblHumidity;
        private System.Windows.Forms.Label lblEmissivityTitle;
        private System.Windows.Forms.Label lblEmissivity;
        private System.Windows.Forms.Label lblReflectedTempTitle;
        private System.Windows.Forms.Label lblReflectedTemp;
        private System.Windows.Forms.Label lblAmbientTempTitle;
        private System.Windows.Forms.Label lblAmbientTemp;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panelTop = new System.Windows.Forms.Panel();
            this.numClusterThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblClusterThresholdTitle = new System.Windows.Forms.Label();
            this.chkShowClusters = new System.Windows.Forms.CheckBox();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelImage = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.grpEnvInfo = new System.Windows.Forms.GroupBox();
            this.lblAmbientTempTitle = new System.Windows.Forms.Label();
            this.lblAmbientTemp = new System.Windows.Forms.Label();
            this.lblReflectedTempTitle = new System.Windows.Forms.Label();
            this.lblReflectedTemp = new System.Windows.Forms.Label();
            this.lblEmissivityTitle = new System.Windows.Forms.Label();
            this.lblEmissivity = new System.Windows.Forms.Label();
            this.lblHumidityTitle = new System.Windows.Forms.Label();
            this.lblHumidity = new System.Windows.Forms.Label();
            this.lblDistanceTitle = new System.Windows.Forms.Label();
            this.lblDistance = new System.Windows.Forms.Label();
            this.grpThermalInfo = new System.Windows.Forms.GroupBox();
            this.lblRealMinTitle = new System.Windows.Forms.Label();
            this.lblRealMin = new System.Windows.Forms.Label();
            this.lblRealMaxTitle = new System.Windows.Forms.Label();
            this.lblRealMax = new System.Windows.Forms.Label();
            this.lblCursorTempTitle = new System.Windows.Forms.Label();
            this.lblCursorTemp = new System.Windows.Forms.Label();
            this.lblRoiMinTitle = new System.Windows.Forms.Label();
            this.lblRoiMin = new System.Windows.Forms.Label();
            this.lblRoiMaxTitle = new System.Windows.Forms.Label();
            this.lblRoiMax = new System.Windows.Forms.Label();
            this.lblRoiAvgTitle = new System.Windows.Forms.Label();
            this.lblRoiAvg = new System.Windows.Forms.Label();
            this.grpImageInfo = new System.Windows.Forms.GroupBox();
            this.lblFilePathTitle = new System.Windows.Forms.Label();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.lblResolutionTitle = new System.Windows.Forms.Label();
            this.lblResolution = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClusterThreshold)).BeginInit();
            this.panelMain.SuspendLayout();
            this.panelImage.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.grpEnvInfo.SuspendLayout();
            this.grpThermalInfo.SuspendLayout();
            this.grpImageInfo.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.Location = new System.Drawing.Point(10, 10);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(130, 30);
            this.btnOpenImage.TabIndex = 0;
            this.btnOpenImage.Text = "Mở ảnh...";
            this.btnOpenImage.UseVisualStyleBackColor = true;
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(160, 17);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(226, 20);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Chưa có ảnh. Nhấn \"Mở ảnh...\"";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff|All files|*.*";
            this.openFileDialog1.Title = "Chọn ảnh nhiệt";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelTop.Controls.Add(this.numClusterThreshold);
            this.panelTop.Controls.Add(this.lblClusterThresholdTitle);
            this.panelTop.Controls.Add(this.chkShowClusters);
            this.panelTop.Controls.Add(this.btnOpenImage);
            this.panelTop.Controls.Add(this.lblInfo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(10, 10, 10, 5);
            this.panelTop.Size = new System.Drawing.Size(1214, 50);
            this.panelTop.TabIndex = 1;
            // 
            // numClusterThreshold
            // 
            this.numClusterThreshold.DecimalPlaces = 1;
            this.numClusterThreshold.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numClusterThreshold.Location = new System.Drawing.Point(985, 15);
            this.numClusterThreshold.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numClusterThreshold.Name = "numClusterThreshold";
            this.numClusterThreshold.Size = new System.Drawing.Size(60, 26);
            this.numClusterThreshold.TabIndex = 4;
            this.numClusterThreshold.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numClusterThreshold.ValueChanged += new System.EventHandler(this.numClusterThreshold_ValueChanged);
            // 
            // lblClusterThresholdTitle
            // 
            this.lblClusterThresholdTitle.AutoSize = true;
            this.lblClusterThresholdTitle.Location = new System.Drawing.Point(881, 17);
            this.lblClusterThresholdTitle.Name = "lblClusterThresholdTitle";
            this.lblClusterThresholdTitle.Size = new System.Drawing.Size(99, 20);
            this.lblClusterThresholdTitle.TabIndex = 3;
            this.lblClusterThresholdTitle.Text = "Ngưỡng (°C):";
            // 
            // chkShowClusters
            // 
            this.chkShowClusters.AutoSize = true;
            this.chkShowClusters.Location = new System.Drawing.Point(723, 16);
            this.chkShowClusters.Name = "chkShowClusters";
            this.chkShowClusters.Size = new System.Drawing.Size(153, 24);
            this.chkShowClusters.TabIndex = 2;
            this.chkShowClusters.Text = "Bật hiển thị vùng";
            this.chkShowClusters.UseVisualStyleBackColor = true;
            this.chkShowClusters.CheckedChanged += new System.EventHandler(this.chkShowClusters_CheckedChanged);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelImage);
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 50);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.panelMain.Size = new System.Drawing.Size(1214, 561);
            this.panelMain.TabIndex = 0;
            // 
            // panelImage
            // 
            this.panelImage.AutoScroll = true;
            this.panelImage.BackColor = System.Drawing.Color.Black;
            this.panelImage.Controls.Add(this.pictureBox1);
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(10, 5);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(914, 551);
            this.panelImage.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panelRight.Controls.Add(this.grpEnvInfo);
            this.panelRight.Controls.Add(this.grpThermalInfo);
            this.panelRight.Controls.Add(this.grpImageInfo);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(924, 5);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.panelRight.Size = new System.Drawing.Size(280, 551);
            this.panelRight.TabIndex = 1;
            // 
            // grpEnvInfo
            // 
            this.grpEnvInfo.Controls.Add(this.lblAmbientTempTitle);
            this.grpEnvInfo.Controls.Add(this.lblAmbientTemp);
            this.grpEnvInfo.Controls.Add(this.lblReflectedTempTitle);
            this.grpEnvInfo.Controls.Add(this.lblReflectedTemp);
            this.grpEnvInfo.Controls.Add(this.lblEmissivityTitle);
            this.grpEnvInfo.Controls.Add(this.lblEmissivity);
            this.grpEnvInfo.Controls.Add(this.lblHumidityTitle);
            this.grpEnvInfo.Controls.Add(this.lblHumidity);
            this.grpEnvInfo.Controls.Add(this.lblDistanceTitle);
            this.grpEnvInfo.Controls.Add(this.lblDistance);
            this.grpEnvInfo.Location = new System.Drawing.Point(10, 363);
            this.grpEnvInfo.Name = "grpEnvInfo";
            this.grpEnvInfo.Padding = new System.Windows.Forms.Padding(10);
            this.grpEnvInfo.Size = new System.Drawing.Size(270, 188);
            this.grpEnvInfo.TabIndex = 2;
            this.grpEnvInfo.TabStop = false;
            this.grpEnvInfo.Text = "Thông số môi trường";
            // 
            // lblAmbientTempTitle
            // 
            this.lblAmbientTempTitle.AutoSize = true;
            this.lblAmbientTempTitle.Location = new System.Drawing.Point(13, 150);
            this.lblAmbientTempTitle.Name = "lblAmbientTempTitle";
            this.lblAmbientTempTitle.Size = new System.Drawing.Size(151, 20);
            this.lblAmbientTempTitle.TabIndex = 8;
            this.lblAmbientTempTitle.Text = "Nhiệt độ môi trường:";
            // 
            // lblAmbientTemp
            // 
            this.lblAmbientTemp.AutoSize = true;
            this.lblAmbientTemp.Location = new System.Drawing.Point(160, 150);
            this.lblAmbientTemp.Name = "lblAmbientTemp";
            this.lblAmbientTemp.Size = new System.Drawing.Size(34, 20);
            this.lblAmbientTemp.TabIndex = 9;
            this.lblAmbientTemp.Text = "- °C";
            // 
            // lblReflectedTempTitle
            // 
            this.lblReflectedTempTitle.AutoSize = true;
            this.lblReflectedTempTitle.Location = new System.Drawing.Point(13, 120);
            this.lblReflectedTempTitle.Name = "lblReflectedTempTitle";
            this.lblReflectedTempTitle.Size = new System.Drawing.Size(132, 20);
            this.lblReflectedTempTitle.TabIndex = 6;
            this.lblReflectedTempTitle.Text = "Nhiệt độ phản xạ:";
            // 
            // lblReflectedTemp
            // 
            this.lblReflectedTemp.AutoSize = true;
            this.lblReflectedTemp.Location = new System.Drawing.Point(148, 120);
            this.lblReflectedTemp.Name = "lblReflectedTemp";
            this.lblReflectedTemp.Size = new System.Drawing.Size(34, 20);
            this.lblReflectedTemp.TabIndex = 7;
            this.lblReflectedTemp.Text = "- °C";
            // 
            // lblEmissivityTitle
            // 
            this.lblEmissivityTitle.AutoSize = true;
            this.lblEmissivityTitle.Location = new System.Drawing.Point(13, 90);
            this.lblEmissivityTitle.Name = "lblEmissivityTitle";
            this.lblEmissivityTitle.Size = new System.Drawing.Size(90, 20);
            this.lblEmissivityTitle.TabIndex = 4;
            this.lblEmissivityTitle.Text = "Độ phát xạ:";
            // 
            // lblEmissivity
            // 
            this.lblEmissivity.AutoSize = true;
            this.lblEmissivity.Location = new System.Drawing.Point(130, 90);
            this.lblEmissivity.Name = "lblEmissivity";
            this.lblEmissivity.Size = new System.Drawing.Size(26, 20);
            this.lblEmissivity.TabIndex = 5;
            this.lblEmissivity.Text = "- ε";
            // 
            // lblHumidityTitle
            // 
            this.lblHumidityTitle.AutoSize = true;
            this.lblHumidityTitle.Location = new System.Drawing.Point(13, 60);
            this.lblHumidityTitle.Name = "lblHumidityTitle";
            this.lblHumidityTitle.Size = new System.Drawing.Size(60, 20);
            this.lblHumidityTitle.TabIndex = 2;
            this.lblHumidityTitle.Text = "Độ ẩm:";
            // 
            // lblHumidity
            // 
            this.lblHumidity.AutoSize = true;
            this.lblHumidity.Location = new System.Drawing.Point(130, 60);
            this.lblHumidity.Name = "lblHumidity";
            this.lblHumidity.Size = new System.Drawing.Size(32, 20);
            this.lblHumidity.TabIndex = 3;
            this.lblHumidity.Text = "- %";
            // 
            // lblDistanceTitle
            // 
            this.lblDistanceTitle.AutoSize = true;
            this.lblDistanceTitle.Location = new System.Drawing.Point(13, 30);
            this.lblDistanceTitle.Name = "lblDistanceTitle";
            this.lblDistanceTitle.Size = new System.Drawing.Size(106, 20);
            this.lblDistanceTitle.TabIndex = 0;
            this.lblDistanceTitle.Text = "Khoảng cách:";
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(130, 30);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(31, 20);
            this.lblDistance.TabIndex = 1;
            this.lblDistance.Text = "- m";
            // 
            // grpThermalInfo
            // 
            this.grpThermalInfo.Controls.Add(this.lblRealMinTitle);
            this.grpThermalInfo.Controls.Add(this.lblRealMin);
            this.grpThermalInfo.Controls.Add(this.lblRealMaxTitle);
            this.grpThermalInfo.Controls.Add(this.lblRealMax);
            this.grpThermalInfo.Controls.Add(this.lblCursorTempTitle);
            this.grpThermalInfo.Controls.Add(this.lblCursorTemp);
            this.grpThermalInfo.Controls.Add(this.lblRoiMinTitle);
            this.grpThermalInfo.Controls.Add(this.lblRoiMin);
            this.grpThermalInfo.Controls.Add(this.lblRoiMaxTitle);
            this.grpThermalInfo.Controls.Add(this.lblRoiMax);
            this.grpThermalInfo.Controls.Add(this.lblRoiAvgTitle);
            this.grpThermalInfo.Controls.Add(this.lblRoiAvg);
            this.grpThermalInfo.Location = new System.Drawing.Point(10, 122);
            this.grpThermalInfo.Name = "grpThermalInfo";
            this.grpThermalInfo.Padding = new System.Windows.Forms.Padding(10);
            this.grpThermalInfo.Size = new System.Drawing.Size(270, 235);
            this.grpThermalInfo.TabIndex = 0;
            this.grpThermalInfo.TabStop = false;
            this.grpThermalInfo.Text = "Thông số nhiệt độ";
            // 
            // lblRealMinTitle
            // 
            this.lblRealMinTitle.AutoSize = true;
            this.lblRealMinTitle.Location = new System.Drawing.Point(13, 30);
            this.lblRealMinTitle.Name = "lblRealMinTitle";
            this.lblRealMinTitle.Size = new System.Drawing.Size(83, 20);
            this.lblRealMinTitle.TabIndex = 0;
            this.lblRealMinTitle.Text = "Min (thực):";
            // 
            // lblRealMin
            // 
            this.lblRealMin.AutoSize = true;
            this.lblRealMin.Location = new System.Drawing.Point(130, 30);
            this.lblRealMin.Name = "lblRealMin";
            this.lblRealMin.Size = new System.Drawing.Size(34, 20);
            this.lblRealMin.TabIndex = 1;
            this.lblRealMin.Text = "- °C";
            // 
            // lblRealMaxTitle
            // 
            this.lblRealMaxTitle.AutoSize = true;
            this.lblRealMaxTitle.Location = new System.Drawing.Point(13, 54);
            this.lblRealMaxTitle.Name = "lblRealMaxTitle";
            this.lblRealMaxTitle.Size = new System.Drawing.Size(87, 20);
            this.lblRealMaxTitle.TabIndex = 2;
            this.lblRealMaxTitle.Text = "Max (thực):";
            // 
            // lblRealMax
            // 
            this.lblRealMax.AutoSize = true;
            this.lblRealMax.Location = new System.Drawing.Point(130, 54);
            this.lblRealMax.Name = "lblRealMax";
            this.lblRealMax.Size = new System.Drawing.Size(34, 20);
            this.lblRealMax.TabIndex = 3;
            this.lblRealMax.Text = "- °C";
            // 
            // lblCursorTempTitle
            // 
            this.lblCursorTempTitle.AutoSize = true;
            this.lblCursorTempTitle.Location = new System.Drawing.Point(13, 84);
            this.lblCursorTempTitle.Name = "lblCursorTempTitle";
            this.lblCursorTempTitle.Size = new System.Drawing.Size(137, 20);
            this.lblCursorTempTitle.TabIndex = 8;
            this.lblCursorTempTitle.Text = "Nhiệt độ tại chuột:";
            // 
            // lblCursorTemp
            // 
            this.lblCursorTemp.AutoSize = true;
            this.lblCursorTemp.Location = new System.Drawing.Point(156, 84);
            this.lblCursorTemp.Name = "lblCursorTemp";
            this.lblCursorTemp.Size = new System.Drawing.Size(34, 20);
            this.lblCursorTemp.TabIndex = 9;
            this.lblCursorTemp.Text = "- °C";
            // 
            // lblRoiMinTitle
            // 
            this.lblRoiMinTitle.AutoSize = true;
            this.lblRoiMinTitle.Location = new System.Drawing.Point(13, 112);
            this.lblRoiMinTitle.Name = "lblRoiMinTitle";
            this.lblRoiMinTitle.Size = new System.Drawing.Size(71, 20);
            this.lblRoiMinTitle.TabIndex = 10;
            this.lblRoiMinTitle.Text = "ROI Min:";
            // 
            // lblRoiMin
            // 
            this.lblRoiMin.AutoSize = true;
            this.lblRoiMin.Location = new System.Drawing.Point(130, 112);
            this.lblRoiMin.Name = "lblRoiMin";
            this.lblRoiMin.Size = new System.Drawing.Size(34, 20);
            this.lblRoiMin.TabIndex = 11;
            this.lblRoiMin.Text = "- °C";
            // 
            // lblRoiMaxTitle
            // 
            this.lblRoiMaxTitle.AutoSize = true;
            this.lblRoiMaxTitle.Location = new System.Drawing.Point(13, 136);
            this.lblRoiMaxTitle.Name = "lblRoiMaxTitle";
            this.lblRoiMaxTitle.Size = new System.Drawing.Size(75, 20);
            this.lblRoiMaxTitle.TabIndex = 12;
            this.lblRoiMaxTitle.Text = "ROI Max:";
            // 
            // lblRoiMax
            // 
            this.lblRoiMax.AutoSize = true;
            this.lblRoiMax.Location = new System.Drawing.Point(130, 136);
            this.lblRoiMax.Name = "lblRoiMax";
            this.lblRoiMax.Size = new System.Drawing.Size(34, 20);
            this.lblRoiMax.TabIndex = 13;
            this.lblRoiMax.Text = "- °C";
            // 
            // lblRoiAvgTitle
            // 
            this.lblRoiAvgTitle.AutoSize = true;
            this.lblRoiAvgTitle.Location = new System.Drawing.Point(13, 160);
            this.lblRoiAvgTitle.Name = "lblRoiAvgTitle";
            this.lblRoiAvgTitle.Size = new System.Drawing.Size(73, 20);
            this.lblRoiAvgTitle.TabIndex = 14;
            this.lblRoiAvgTitle.Text = "ROI Avg:";
            // 
            // lblRoiAvg
            // 
            this.lblRoiAvg.AutoSize = true;
            this.lblRoiAvg.Location = new System.Drawing.Point(130, 160);
            this.lblRoiAvg.Name = "lblRoiAvg";
            this.lblRoiAvg.Size = new System.Drawing.Size(34, 20);
            this.lblRoiAvg.TabIndex = 15;
            this.lblRoiAvg.Text = "- °C";
            // 
            // grpImageInfo
            // 
            this.grpImageInfo.Controls.Add(this.lblFilePathTitle);
            this.grpImageInfo.Controls.Add(this.lblFilePath);
            this.grpImageInfo.Controls.Add(this.lblResolutionTitle);
            this.grpImageInfo.Controls.Add(this.lblResolution);
            this.grpImageInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpImageInfo.Location = new System.Drawing.Point(10, 0);
            this.grpImageInfo.Name = "grpImageInfo";
            this.grpImageInfo.Padding = new System.Windows.Forms.Padding(10);
            this.grpImageInfo.Size = new System.Drawing.Size(270, 116);
            this.grpImageInfo.TabIndex = 1;
            this.grpImageInfo.TabStop = false;
            this.grpImageInfo.Text = "Thông tin ảnh";
            // 
            // lblFilePathTitle
            // 
            this.lblFilePathTitle.AutoSize = true;
            this.lblFilePathTitle.Location = new System.Drawing.Point(13, 28);
            this.lblFilePathTitle.Name = "lblFilePathTitle";
            this.lblFilePathTitle.Size = new System.Drawing.Size(92, 20);
            this.lblFilePathTitle.TabIndex = 0;
            this.lblFilePathTitle.Text = "Đường dẫn:";
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoEllipsis = true;
            this.lblFilePath.Location = new System.Drawing.Point(110, 28);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(160, 20);
            this.lblFilePath.TabIndex = 1;
            this.lblFilePath.Text = "(chưa có)";
            // 
            // lblResolutionTitle
            // 
            this.lblResolutionTitle.AutoSize = true;
            this.lblResolutionTitle.Location = new System.Drawing.Point(13, 60);
            this.lblResolutionTitle.Name = "lblResolutionTitle";
            this.lblResolutionTitle.Size = new System.Drawing.Size(87, 20);
            this.lblResolutionTitle.TabIndex = 2;
            this.lblResolutionTitle.Text = "Kích thước:";
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(110, 60);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(14, 20);
            this.lblResolution.TabIndex = 3;
            this.lblResolution.Text = "-";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 611);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1214, 32);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 2;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(88, 25);
            this.statusLabel.Text = "Sẵn sàng.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1214, 643);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "Form1";
            this.Text = "Thermo Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClusterThreshold)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.panelImage.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.grpEnvInfo.ResumeLayout(false);
            this.grpEnvInfo.PerformLayout();
            this.grpThermalInfo.ResumeLayout(false);
            this.grpThermalInfo.PerformLayout();
            this.grpImageInfo.ResumeLayout(false);
            this.grpImageInfo.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}