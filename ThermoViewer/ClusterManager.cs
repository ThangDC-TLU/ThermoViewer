using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ThermoViewer
{
    public class ClusterManager
    {
        private class TempCluster
        {
            public Rectangle Bounds;
            public double MeanTemp;
            public double MinTemp;
            public double MaxTemp;
            public Color Color;
        }

        private readonly ImageCoordinateMapper _mapper;
        private readonly Func<double> _getClusterThreshold;

        private readonly List<TempCluster> _clusters = new List<TempCluster>();
        private int _hoverClusterIndex = -1;

        public bool HasLastRoi { get; private set; }
        private Point _lastRoiP1;
        private Point _lastRoiP2;

        public ClusterManager(ImageCoordinateMapper mapper, Func<double> getClusterThreshold)
        {
            _mapper = mapper;
            _getClusterThreshold = getClusterThreshold;
        }

        public void Reset()
        {
            _clusters.Clear();
            _hoverClusterIndex = -1;
            HasLastRoi = false;
        }

        public void ClearClusters()
        {
            _clusters.Clear();
            _hoverClusterIndex = -1;
        }

        public void SetLastRoi(Point p1, Point p2)
        {
            _lastRoiP1 = p1;
            _lastRoiP2 = p2;
            HasLastRoi = true;
        }

        public struct RoiStats
        {
            public double Min, Max, Avg;
        }

        /// <summary>
        /// Tính toán thông tin trong vùng ROI
        /// </summary>
        public RoiStats? CalculateRoiStats(double[,] data, Point p1, Point p2)
        {
            if (data == null) return null;

            double min = double.MaxValue;
            double max = double.MinValue;
            double sum = 0;
            int cnt = 0;

            int startX = Math.Min(p1.X, p2.X);
            int endX = Math.Max(p1.X, p2.X);
            int startY = Math.Min(p1.Y, p2.Y);
            int endY = Math.Max(p1.Y, p2.Y);

            for (int y = startY; y <= endY; y++)
                for (int x = startX; x <= endX; x++)
                {
                    double v = data[y, x];
                    if (v < min) min = v;
                    if (v > max) max = v;
                    sum += v;
                    cnt++;
                }

            if (cnt == 0) return null;

            return new RoiStats
            {
                Min = min,
                Max = max,
                Avg = sum / cnt
            };
        }

        /// <summary>
        /// Phân cụm các điểm trong ma trận nhiệt thành nhiều vùng nhiệt theo ngưỡng nhiệt độ
        /// </summary>
        public void BuildClustersForRoi(double[,] data, Label infoLabel)
        {
            _clusters.Clear();
            _hoverClusterIndex = -1;

            if (data == null || !HasLastRoi)
                return;

            int width = data.GetLength(1);
            int height = data.GetLength(0);

            Point p1 = _lastRoiP1;
            Point p2 = _lastRoiP2;

            int startX = Math.Max(0, Math.Min(p1.X, p2.X));
            int endX = Math.Min(width - 1, Math.Max(p1.X, p2.X));
            int startY = Math.Max(0, Math.Min(p1.Y, p2.Y));
            int endY = Math.Min(height - 1, Math.Max(p1.Y, p2.Y));

            if (endX < startX || endY < startY) return;

            int roiW = endX - startX + 1;
            int roiH = endY - startY + 1;

            // 1. Min/Max trong ROI
            double roiMin = double.MaxValue;
            double roiMax = double.MinValue;
            for (int y = startY; y <= endY; y++)
                for (int x = startX; x <= endX; x++)
                {
                    double t = data[y, x];
                    if (t < roiMin) roiMin = t;
                    if (t > roiMax) roiMax = t;
                }

            double threshold = _getClusterThreshold();
            if (threshold <= 0) threshold = 0.5;

            int bucketCount = (int)Math.Ceiling((roiMax - roiMin) / threshold);
            if (bucketCount <= 0) bucketCount = 1;

            // 2. Gán bucket
            int[,] bucketIndex = new int[roiH, roiW];
            for (int ry = 0; ry < roiH; ry++)
                for (int rx = 0; rx < roiW; rx++)
                {
                    int x = startX + rx;
                    int y = startY + ry;
                    double t = data[y, x];

                    int bi = (int)((t - roiMin) / threshold);
                    if (bi < 0) bi = 0;
                    if (bi >= bucketCount) bi = bucketCount - 1;
                    bucketIndex[ry, rx] = bi;
                }

            bool[,] visited = new bool[roiH, roiW];
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            var rand = new Random();

            // 3. BFS trong từng bucket để tách cluster
            for (int ry = 0; ry < roiH; ry++)
                for (int rx = 0; rx < roiW; rx++)
                {
                    if (visited[ry, rx]) continue;

                    int thisBucket = bucketIndex[ry, rx];
                    var queue = new Queue<Point>();
                    queue.Enqueue(new Point(rx, ry));
                    visited[ry, rx] = true;

                    double sum = 0, min = double.MaxValue, max = double.MinValue;
                    int cnt = 0;
                    int minX = startX + rx, maxX = startX + rx;
                    int minY = startY + ry, maxY = startY + ry;

                    while (queue.Count > 0)
                    {
                        var pr = queue.Dequeue();
                        int cx = startX + pr.X;
                        int cy = startY + pr.Y;

                        double t = data[cy, cx];
                        sum += t;
                        if (t < min) min = t;
                        if (t > max) max = t;
                        cnt++;

                        if (cx < minX) minX = cx;
                        if (cx > maxX) maxX = cx;
                        if (cy < minY) minY = cy;
                        if (cy > maxY) maxY = cy;

                        for (int k = 0; k < 4; k++)
                        {
                            int nrx = pr.X + dx[k];
                            int nry = pr.Y + dy[k];

                            if (nrx < 0 || nry < 0 || nrx >= roiW || nry >= roiH)
                                continue;
                            if (visited[nry, nrx]) continue;
                            if (bucketIndex[nry, nrx] != thisBucket) continue;

                            visited[nry, nrx] = true;
                            queue.Enqueue(new Point(nrx, nry));
                        }
                    }

                    if (cnt == 0) continue;

                    double mean = sum / cnt;
                    Rectangle bounds = Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);

                    Color color = Color.FromArgb(
                        80,
                        rand.Next(50, 255),
                        rand.Next(50, 255),
                        rand.Next(50, 255));

                    _clusters.Add(new TempCluster
                    {
                        Bounds = bounds,
                        MeanTemp = mean,
                        MinTemp = min,
                        MaxTemp = max,
                        Color = color
                    });
                }

            if (infoLabel != null)
                infoLabel.Text = $"{_clusters.Count} vùng (ngưỡng {threshold:F1}°C, ROI Min {roiMin:F1}, Max {roiMax:F1})";
        }

        // Vẽ cluster + highlight
        public void DrawClusters(Graphics g, bool showClusters)
        {
            if (!showClusters || _clusters.Count == 0)
                return;

            foreach (var c in _clusters)
            {
                Rectangle r = _mapper.ImageRectToScreenRect(c.Bounds);
                using (Brush b = new SolidBrush(c.Color))
                    g.FillRectangle(b, r);
                using (Pen p = new Pen(Color.Black, 1))
                    g.DrawRectangle(p, r);
            }

            if (_hoverClusterIndex >= 0 && _hoverClusterIndex < _clusters.Count)
            {
                var c = _clusters[_hoverClusterIndex];
                Rectangle r = _mapper.ImageRectToScreenRect(c.Bounds);
                using (Pen p = new Pen(Color.Yellow, 3))
                    g.DrawRectangle(p, r);
            }
        }

        /// <summary>
        /// Hover: ƯU TIÊN cluster có diện tích NHỎ NHẤT trong tất cả vùng chứa điểm.
        /// </summary>
        public void HandleHover(Point screenPoint,
                                bool showClusters,
                                Label infoLabel)
        {
            if (!showClusters || _clusters.Count == 0)
                return;

            Point imgPt = _mapper.ScreenToImage(screenPoint);

            int bestIdx = -1;
            int bestArea = int.MaxValue;

            for (int i = 0; i < _clusters.Count; i++)
            {
                Rectangle b = _clusters[i].Bounds;

                // Nới vùng 1 pixel để dễ bắt vùng rất nhỏ
                Rectangle expanded = Rectangle.Inflate(b, 1, 1);
                if (!expanded.Contains(imgPt)) continue;

                int area = b.Width * b.Height;
                if (area < bestArea)
                {
                    bestArea = area;
                    bestIdx = i;
                }
            }

            if (bestIdx != _hoverClusterIndex)
            {
                _hoverClusterIndex = bestIdx;

                if (bestIdx >= 0 && infoLabel != null)
                {
                    var c = _clusters[bestIdx];
                    infoLabel.Text =
                                $"Vùng {bestIdx + 1}: Nhiệt độ trung bình: {c.MeanTemp:F1}°C " +
                                $"(Min: {c.MinTemp:F2}°C, Max: {c.MaxTemp:F2}°C)";
                }
            }
        }
    }
}