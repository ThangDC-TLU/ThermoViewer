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
        private System.Windows.Forms.Label lblCameraTitle;
        private System.Windows.Forms.Label lblCamera;
        private System.Windows.Forms.Label lblRealMinTitle;
        private System.Windows.Forms.Label lblRealMin;
        private System.Windows.Forms.Label lblRealMaxTitle;
        private System.Windows.Forms.Label lblRealMax;
        private System.Windows.Forms.Label lblOriginMinTitle;
        private System.Windows.Forms.Label lblOriginMin;
        private System.Windows.Forms.Label lblOriginMaxTitle;
        private System.Windows.Forms.Label lblOriginMax;
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelImage = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.grpThermalInfo = new System.Windows.Forms.GroupBox();
            this.lblRealMinTitle = new System.Windows.Forms.Label();
            this.lblRealMin = new System.Windows.Forms.Label();
            this.lblRealMaxTitle = new System.Windows.Forms.Label();
            this.lblRealMax = new System.Windows.Forms.Label();
            this.lblOriginMinTitle = new System.Windows.Forms.Label();
            this.lblOriginMin = new System.Windows.Forms.Label();
            this.lblOriginMaxTitle = new System.Windows.Forms.Label();
            this.lblOriginMax = new System.Windows.Forms.Label();
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
            this.lblCameraTitle = new System.Windows.Forms.Label();
            this.lblCamera = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelImage.SuspendLayout();
            this.panelRight.SuspendLayout();
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
            this.panelTop.Controls.Add(this.btnOpenImage);
            this.panelTop.Controls.Add(this.lblInfo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(10, 10, 10, 5);
            this.panelTop.Size = new System.Drawing.Size(1214, 50);
            this.panelTop.TabIndex = 1;
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
            this.panelRight.Controls.Add(this.grpThermalInfo);
            this.panelRight.Controls.Add(this.grpImageInfo);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(924, 5);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.panelRight.Size = new System.Drawing.Size(280, 551);
            this.panelRight.TabIndex = 1;
            // 
            // grpThermalInfo
            // 
            this.grpThermalInfo.Controls.Add(this.lblRealMinTitle);
            this.grpThermalInfo.Controls.Add(this.lblRealMin);
            this.grpThermalInfo.Controls.Add(this.lblRealMaxTitle);
            this.grpThermalInfo.Controls.Add(this.lblRealMax);
            this.grpThermalInfo.Controls.Add(this.lblOriginMinTitle);
            this.grpThermalInfo.Controls.Add(this.lblOriginMin);
            this.grpThermalInfo.Controls.Add(this.lblOriginMaxTitle);
            this.grpThermalInfo.Controls.Add(this.lblOriginMax);
            this.grpThermalInfo.Controls.Add(this.lblCursorTempTitle);
            this.grpThermalInfo.Controls.Add(this.lblCursorTemp);
            this.grpThermalInfo.Controls.Add(this.lblRoiMinTitle);
            this.grpThermalInfo.Controls.Add(this.lblRoiMin);
            this.grpThermalInfo.Controls.Add(this.lblRoiMaxTitle);
            this.grpThermalInfo.Controls.Add(this.lblRoiMax);
            this.grpThermalInfo.Controls.Add(this.lblRoiAvgTitle);
            this.grpThermalInfo.Controls.Add(this.lblRoiAvg);
            this.grpThermalInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpThermalInfo.Location = new System.Drawing.Point(10, 134);
            this.grpThermalInfo.Name = "grpThermalInfo";
            this.grpThermalInfo.Padding = new System.Windows.Forms.Padding(10);
            this.grpThermalInfo.Size = new System.Drawing.Size(270, 417);
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
            // lblOriginMinTitle
            // 
            this.lblOriginMinTitle.AutoSize = true;
            this.lblOriginMinTitle.Location = new System.Drawing.Point(13, 78);
            this.lblOriginMinTitle.Name = "lblOriginMinTitle";
            this.lblOriginMinTitle.Size = new System.Drawing.Size(83, 20);
            this.lblOriginMinTitle.TabIndex = 4;
            this.lblOriginMinTitle.Text = "Origin Min:";
            // 
            // lblOriginMin
            // 
            this.lblOriginMin.AutoSize = true;
            this.lblOriginMin.Location = new System.Drawing.Point(130, 78);
            this.lblOriginMin.Name = "lblOriginMin";
            this.lblOriginMin.Size = new System.Drawing.Size(34, 20);
            this.lblOriginMin.TabIndex = 5;
            this.lblOriginMin.Text = "- °C";
            // 
            // lblOriginMaxTitle
            // 
            this.lblOriginMaxTitle.AutoSize = true;
            this.lblOriginMaxTitle.Location = new System.Drawing.Point(13, 102);
            this.lblOriginMaxTitle.Name = "lblOriginMaxTitle";
            this.lblOriginMaxTitle.Size = new System.Drawing.Size(87, 20);
            this.lblOriginMaxTitle.TabIndex = 6;
            this.lblOriginMaxTitle.Text = "Origin Max:";
            // 
            // lblOriginMax
            // 
            this.lblOriginMax.AutoSize = true;
            this.lblOriginMax.Location = new System.Drawing.Point(130, 102);
            this.lblOriginMax.Name = "lblOriginMax";
            this.lblOriginMax.Size = new System.Drawing.Size(34, 20);
            this.lblOriginMax.TabIndex = 7;
            this.lblOriginMax.Text = "- °C";
            // 
            // lblCursorTempTitle
            // 
            this.lblCursorTempTitle.AutoSize = true;
            this.lblCursorTempTitle.Location = new System.Drawing.Point(13, 150);
            this.lblCursorTempTitle.Name = "lblCursorTempTitle";
            this.lblCursorTempTitle.Size = new System.Drawing.Size(137, 20);
            this.lblCursorTempTitle.TabIndex = 8;
            this.lblCursorTempTitle.Text = "Nhiệt độ tại chuột:";
            // 
            // lblCursorTemp
            // 
            this.lblCursorTemp.AutoSize = true;
            this.lblCursorTemp.Location = new System.Drawing.Point(156, 150);
            this.lblCursorTemp.Name = "lblCursorTemp";
            this.lblCursorTemp.Size = new System.Drawing.Size(34, 20);
            this.lblCursorTemp.TabIndex = 9;
            this.lblCursorTemp.Text = "- °C";
            // 
            // lblRoiMinTitle
            // 
            this.lblRoiMinTitle.AutoSize = true;
            this.lblRoiMinTitle.Location = new System.Drawing.Point(13, 180);
            this.lblRoiMinTitle.Name = "lblRoiMinTitle";
            this.lblRoiMinTitle.Size = new System.Drawing.Size(71, 20);
            this.lblRoiMinTitle.TabIndex = 10;
            this.lblRoiMinTitle.Text = "ROI Min:";
            // 
            // lblRoiMin
            // 
            this.lblRoiMin.AutoSize = true;
            this.lblRoiMin.Location = new System.Drawing.Point(130, 180);
            this.lblRoiMin.Name = "lblRoiMin";
            this.lblRoiMin.Size = new System.Drawing.Size(34, 20);
            this.lblRoiMin.TabIndex = 11;
            this.lblRoiMin.Text = "- °C";
            // 
            // lblRoiMaxTitle
            // 
            this.lblRoiMaxTitle.AutoSize = true;
            this.lblRoiMaxTitle.Location = new System.Drawing.Point(13, 204);
            this.lblRoiMaxTitle.Name = "lblRoiMaxTitle";
            this.lblRoiMaxTitle.Size = new System.Drawing.Size(75, 20);
            this.lblRoiMaxTitle.TabIndex = 12;
            this.lblRoiMaxTitle.Text = "ROI Max:";
            // 
            // lblRoiMax
            // 
            this.lblRoiMax.AutoSize = true;
            this.lblRoiMax.Location = new System.Drawing.Point(130, 204);
            this.lblRoiMax.Name = "lblRoiMax";
            this.lblRoiMax.Size = new System.Drawing.Size(34, 20);
            this.lblRoiMax.TabIndex = 13;
            this.lblRoiMax.Text = "- °C";
            // 
            // lblRoiAvgTitle
            // 
            this.lblRoiAvgTitle.AutoSize = true;
            this.lblRoiAvgTitle.Location = new System.Drawing.Point(13, 228);
            this.lblRoiAvgTitle.Name = "lblRoiAvgTitle";
            this.lblRoiAvgTitle.Size = new System.Drawing.Size(73, 20);
            this.lblRoiAvgTitle.TabIndex = 14;
            this.lblRoiAvgTitle.Text = "ROI Avg:";
            // 
            // lblRoiAvg
            // 
            this.lblRoiAvg.AutoSize = true;
            this.lblRoiAvg.Location = new System.Drawing.Point(130, 228);
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
            this.grpImageInfo.Controls.Add(this.lblCameraTitle);
            this.grpImageInfo.Controls.Add(this.lblCamera);
            this.grpImageInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpImageInfo.Location = new System.Drawing.Point(10, 0);
            this.grpImageInfo.Name = "grpImageInfo";
            this.grpImageInfo.Padding = new System.Windows.Forms.Padding(10);
            this.grpImageInfo.Size = new System.Drawing.Size(270, 134);
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
            // lblCameraTitle
            // 
            this.lblCameraTitle.AutoSize = true;
            this.lblCameraTitle.Location = new System.Drawing.Point(13, 92);
            this.lblCameraTitle.Name = "lblCameraTitle";
            this.lblCameraTitle.Size = new System.Drawing.Size(69, 20);
            this.lblCameraTitle.TabIndex = 4;
            this.lblCameraTitle.Text = "Camera:";
            // 
            // lblCamera
            // 
            this.lblCamera.AutoSize = true;
            this.lblCamera.Location = new System.Drawing.Point(110, 92);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.Size = new System.Drawing.Size(14, 20);
            this.lblCamera.TabIndex = 5;
            this.lblCamera.Text = "-";
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
            this.panelMain.ResumeLayout(false);
            this.panelImage.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
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