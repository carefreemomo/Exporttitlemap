using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportTitleMap
{
    public struct ZoomInfo
    {
        public int MinZoom;
        public int MaxZoom;
        public int ImageZoom;
    }
    public partial class Form1 : Form
    {
        TextBox debuglog;
        TextBox lng;
        TextBox txtlng;
        TextBox lat;
        TextBox txtlat;
        TextBox fileTxt;
        TextBox fileFolderTxt; ComboBox m1; ComboBox m2; ComboBox m3;
        ProgressBar progressBar;
        private Bitmap tileImage;   //目标图片
        private string outputPath;  //输出目录
        private float imgWidth, imgHeight;
        private const int titleSize = 256;
        private ZoomInfo zoomInfo = new ZoomInfo();
        private LatLng blLatLng;
        private int finishCount = 0;
        private int totalCount = 0;
        private bool isalpha;
        public Form1()
        {
            Name = "腾讯地图瓦片生成工具";
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lng = new TextBox();
            lng.Text = "经度：";
            lng.Size = new Size(120, 20);
            lng.Location = new Point(50, 20);
            lng.Enabled = false;
            lng.BorderStyle = BorderStyle.None;
            this.Controls.Add(lng);
            txtlng = new TextBox();
            txtlng.Text = "118.056349000000";
            txtlng.Size = new Size(120, 20);
            txtlng.Location = new Point(170, 20);
            this.Controls.Add(txtlng);

            lat = new TextBox();
            lat.Text = "纬度：";
            lat.Size = new Size(120, 20);
            lat.Location = new Point(50, 45);
            lat.Enabled = false;
            lat.BorderStyle = BorderStyle.None;
            this.Controls.Add(lat);
            txtlat = new TextBox();
            txtlat.Text = "24.435892000000";
            txtlat.Size = new Size(120, 20);
            txtlat.Location = new Point(170, 45);
            this.Controls.Add(txtlat);

            TextBox text = new TextBox();
            text.Text = "层级：";
            text.Size = new Size(50, 20);
            text.Location = new Point(50, 75);
            text.Enabled = false;
            text.BorderStyle = BorderStyle.None;
            this.Controls.Add(text);
            text = new TextBox();
            text.Text = "-";
            text.Size = new Size(10, 20);
            text.Location = new Point(235, 75);
            text.Enabled = false;
            text.BorderStyle = BorderStyle.None;
            this.Controls.Add(text);
            m1 = new ComboBox();
            m1.Location = new Point(170, 70); // 按钮屏幕位置
            m1.Size = new Size(50, 40);
            m2 = new ComboBox();
            m2.Location = new Point(250, 70); // 按钮屏幕位置
            m2.Size = new Size(50, 40);
            for (int i = 13; i <= 20; i++)
            {
                m1.Items.Add(i);
                m2.Items.Add(i);
            }
            m1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            m2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            this.Controls.Add(m1);
            this.Controls.Add(m2);
            m1.SelectedItem = 16;
            m2.SelectedItem = 16;

            text = new TextBox();
            text.Text = "当前层级：";
            text.Size = new Size(60, 20);
            text.Location = new Point(320, 75);
            text.Enabled = false;
            text.BorderStyle = BorderStyle.None;
            this.Controls.Add(text);
            m3 = new ComboBox();
            m3.Location = new Point(380, 70); // 按钮屏幕位置
            m3.Size = new Size(50, 40);
            for (int i = 4; i <= 20; i++)
            {
                m3.Items.Add(i);
            }
            m3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            this.Controls.Add(m3);
            m3.SelectedItem = 20;


            Button fileBtn = new Button();
            fileBtn.Text = "目标路径"; // 按钮上面文字
            fileBtn.Location = new Point(50, 105); // 按钮屏幕位置
            fileBtn.Size = new Size(100, 40);
            fileBtn.Parent = this;
            fileBtn.Click += new EventHandler(FileBtn_Click);
            this.Controls.Add(fileBtn);
            fileTxt = new TextBox();
            fileTxt.Text = "";
            fileTxt.Size = new Size(500, 40);
            fileTxt.Location = new Point(170, 115);
            this.Controls.Add(fileTxt);
            //fileTxt.Text = @"C:\Users\Administrator\Desktop\maptitle.jpg";
            //tileImage = new Bitmap(fileTxt.Text);
            //imgWidth = tileImage.Width;
            //imgHeight = tileImage.Height;

            Button filefolderBtn = new Button();
            filefolderBtn.Text = "存档路径"; // 按钮上面文字
            filefolderBtn.Location = new Point(50, 155); // 按钮屏幕位置
            filefolderBtn.Size = new Size(100, 40);
            filefolderBtn.Parent = this;
            filefolderBtn.Click += new EventHandler(FileFolderBtn_Click);
            this.Controls.Add(filefolderBtn);
            fileFolderTxt = new TextBox();
            fileFolderTxt.Text = "";
            fileFolderTxt.Size = new Size(500, 40);
            fileFolderTxt.Location = new Point(170, 170);
            this.Controls.Add(fileFolderTxt);
            //fileFolderTxt.Text = @"C:\Users\Administrator\Desktop\nn";
            //outputPath = fileFolderTxt.Text;


            Button contectBtn = new Button();
            contectBtn.Name = "contectBtn1";
            contectBtn.Text = "运行"; // 按钮上面文字
            contectBtn.Location = new Point(50, 400); // 按钮屏幕位置
            contectBtn.Size = new Size(100, 40);
            contectBtn.Parent = this;
            contectBtn.Click += new EventHandler(ContectBtn_Click);
            this.Controls.Add(contectBtn);

            Button deleteBtn = new Button();
            deleteBtn.Text = "清除缓存"; // 按钮上面文字
            deleteBtn.Location = new Point(200, 400); // 按钮屏幕位置
            deleteBtn.Size = new Size(100, 40);
            deleteBtn.Parent = this;
            deleteBtn.Click += new EventHandler(DeleteBtn_Click);
            this.Controls.Add(deleteBtn);

            Button contectBtn2 = new Button();
            contectBtn2.Name = "contectBtn2";
            contectBtn2.Text = "运行半透明"; // 按钮上面文字
            contectBtn2.Location = new Point(350, 400); // 按钮屏幕位置
            contectBtn2.Size = new Size(100, 40);
            contectBtn2.Parent = this;
            contectBtn2.Click += new EventHandler(ContectBtn_Click);
            this.Controls.Add(contectBtn2);

            debuglog = new TextBox();
            debuglog.Text = "";
            debuglog.Size = new Size(650, 170);
            debuglog.Location = new Point(50, 200);
            debuglog.WordWrap = true;
            debuglog.Multiline = true;
            //debuglog.Enabled = false;
            //debuglog.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(debuglog);

            progressBar = new ProgressBar();
            progressBar.Value = 0;
            progressBar.Size = new Size(650, 10);
            progressBar.Location = new Point(50, 380);
            progressBar.Minimum = 0;
            progressBar.Maximum = 1;
            this.Controls.Add(progressBar);

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            zoomInfo.MinZoom = int.Parse(m1.SelectedItem.ToString());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            zoomInfo.MaxZoom = int.Parse(m2.SelectedItem.ToString());
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            zoomInfo.ImageZoom = int.Parse(m3.SelectedItem.ToString());
        }

        private void FileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "(图片文件)|*.png;*.jpg";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                string tableName = fd.SafeFileName.Split('.')[0];
                fileTxt.Text = fd.FileName;
                tileImage = new Bitmap(fd.FileName);
                imgWidth = tileImage.Width;
                imgHeight = tileImage.Height;
            }
        }

        private void FileFolderBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择Exl所在文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                fileFolderTxt.Text = foldPath;
                outputPath = foldPath;
            }
        }
        private void ContectBtn_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Name == "contectBtn1")
            {
                isalpha = false;
            }
            else
            {
                isalpha = true;
            }
            if (m2.SelectedIndex < m1.SelectedIndex)
            {
                MessageBox.Show("最大级不能小于最小级");
                return;
            }
            blLatLng = new LatLng(double.Parse(txtlat.Text), double.Parse(txtlng.Text));//getStart();
            CheckMap();
            debuglog.Text += string.Format("经度：{0},纬度：{1}\r\n", blLatLng.Lat, blLatLng.Lng);
            beginCut();
            generateHTMLFile();
            //tileImage.Dispose();
        }
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }

        private void CheckMap()
        {
            debuglog.Clear();
            double[] sz = getXY(118.06505262851715, 24.44439528675154);
            int z = zoomInfo.MinZoom;
            double[] bl = getXY(-180, -85.05112877980659);
            double[] tl = getXY(-180, 85.05112877980659);
            double[] br = getXY(180, -85.05112877980659);
            Console.WriteLine(String.Format("坐标BL点：%f,%f\n", bl[0], bl[1]));
            Console.WriteLine(String.Format("坐标TL点：%f,%f\n", tl[0], tl[1]));
            Console.WriteLine(String.Format("坐标BR点：%f,%f\n", br[0], br[1]));
            Console.WriteLine(String.Format("坐标：%f,%f\n", sz[0], sz[1]));
            double w = (br[0] - bl[0]) / Math.Pow(2, z);//格网宽度
            double h = (tl[1] - bl[1]) / Math.Pow(2, z);//格网高度
            Console.WriteLine(String.Format("格网大小：%f x %f\n", w, h));
            int[] gridxy = new int[2];
            int c = (int)((sz[0] - bl[0]) / w);
            int r = (int)((sz[1] - bl[1]) / h);
            Console.WriteLine(String.Format("对应行列号：%d,%d\n", c, r));
            double c_d = Math.Floor(c / 16.0);
            double r_d = Math.Floor(r / 16.0);
            string url = String.Format("对应请求url:http://p3.map.gtimg.com/maptilesv2/{0}/{1}/{2}/{3}_{4}.png \r\n", z, (int)c_d, (int)r_d, c, r);
            debuglog.Text += url;
            Console.WriteLine(String.Format("对应起始位置请求url:http://p3.map.gtimg.com/maptilesv2/{0}/{1}/{2}/{3}_{4}.png \r\n", z, (int)c_d, (int)r_d, c, r));
        }

        private double[] getXY(double lon, double lat)
        {
            double earthRad = 6378137.0;
            double x = lon * Math.PI / 180.0 * earthRad;
            double a = lat * Math.PI / 180.0;
            double y = earthRad / 2.0 * Math.Log((1.0 + Math.Sin(a)) / (1.0 - Math.Sin(a)));
            return new double[] { x, y };
        }
        public LatLng getStart()
        {
            float lat = Convert.ToSingle(txtlat.Text);
            float lng = Convert.ToSingle(txtlng.Text);
            return new LatLng(lat, lng);
        }

        private void generateHTMLFile()
        {
            StreamWriter htmlFile = File.CreateText(outputPath + "/index.html");
            string path = @"key.txt";
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    htmlFile.WriteLine(sr.ReadLine());
                }
            }
            htmlFile.WriteLine("<script>");
            htmlFile.WriteLine("var map;");
            htmlFile.WriteLine("function init()");
            htmlFile.WriteLine("{");
            htmlFile.WriteLine("    var earthlayer = new qq.maps.ImageMapType({");
            htmlFile.WriteLine("    name: 'tentxun',");
            htmlFile.WriteLine("    alt: 'tentxun',");
            htmlFile.WriteLine("    tileSize: new qq.maps.Size(256, 256),");
            htmlFile.WriteLine(string.Format("    minZoom: {0},", zoomInfo.MinZoom));
            htmlFile.WriteLine(string.Format("    maxZoom: {0},", zoomInfo.MaxZoom));
            htmlFile.WriteLine("    opacity: 1,");
            htmlFile.WriteLine("    getTileUrl: function(tile, zoom) {");
            htmlFile.WriteLine("     var z = zoom,");
            htmlFile.WriteLine("            x = tile.x,");
            htmlFile.WriteLine("            y = tile.y;");
            htmlFile.WriteLine("    return 'tiles/' + z + '/' + x + '_' + y + '.png';");
            htmlFile.WriteLine("}");
            htmlFile.WriteLine("});");
            htmlFile.WriteLine("map = new qq.maps.Map(document.getElementById(\"container\"), {");
            htmlFile.WriteLine(string.Format("    center: new qq.maps.LatLng({0}, {1}),", blLatLng.Lat, blLatLng.Lng));
            htmlFile.WriteLine(string.Format("    zoom: {0}", zoomInfo.ImageZoom));
            htmlFile.WriteLine("});");
            htmlFile.WriteLine("map.overlayMapTypes.push(earthlayer);");
            htmlFile.WriteLine("}");
            htmlFile.WriteLine("</script>");
            htmlFile.WriteLine("</head>");
            htmlFile.WriteLine("<body onload = \"init(); \" >");
            htmlFile.WriteLine("<div id=\"container\"></div>");
            htmlFile.WriteLine("<span id = \"info\" ></ span >");
            htmlFile.WriteLine("</ body >");
            htmlFile.WriteLine("</ html >");
            htmlFile.Close();
        }

        private void beginCut()
        {
            for (int i = zoomInfo.MinZoom; i <= zoomInfo.MaxZoom; i++)
            {
                Bitmap image;
                if (i == zoomInfo.ImageZoom)
                {
                    image = tileImage;
                }
                else
                {
                    // 生成临时图片
                    Size newSize = new Size();
                    double factor = Math.Pow(2, i - zoomInfo.ImageZoom);
                    newSize.Width = (int)Math.Round(imgWidth * factor);
                    newSize.Height = (int)Math.Round(imgHeight * factor);
                    //if (newSize.Width < 256 || newSize.Height < 256)
                    //{
                    //    // 图片尺寸过小不再切了
                    //    Console.WriteLine(string.Format("({0},{1})尺寸过小，跳过", newSize.Width, newSize.Height));
                    //    debuglog.Text += string.Format("({0},{1})尺寸过小，跳过\r\n", newSize.Width, newSize.Height);
                    //    continue;
                    //}
                    Console.WriteLine(tileImage.Height + ", " + tileImage.Width);
                    image = new Bitmap(tileImage, newSize);
                }
                debuglog.Text += string.Format("生成图片\r\n");
                debuglog.Text += string.Format("开始切片处理{0}层级\r\n", i.ToString());
                cutImage(image, i);
                debuglog.Text += string.Format("结束切片处理{0}层级\r\n", i.ToString());
            }
        }

        /// <summary>
        /// 切某一级别的图
        /// </summary>
        /// <param name="imgFile">图片对象</param>
        /// <param name="zoom">图片对应的级别</param>
        private void cutImage(Bitmap imgFile, int zoom)
        {
            //int halfWidth = (int)Math.Round((double)imgFile.Width / 2);
            //int halfHeight = (int)Math.Round((double)imgFile.Height / 2);
            Directory.CreateDirectory(outputPath + "/tiles/" + zoom.ToString());

            double[] sz = getXY(blLatLng.Lng, blLatLng.Lat);
            int z = zoom;
            double[] bl = getXY(-180, -85.05112877980659);
            double[] tl = getXY(-180, 85.05112877980659);
            double[] br = getXY(180, -85.05112877980659);
            double w = (br[0] - bl[0]) / Math.Pow(2, z);//格网宽度
            double h = (tl[1] - bl[1]) / Math.Pow(2, z);//格网高度
            int[] gridxy = new int[2];
            int pointOffsetx = (int)((((sz[0] - bl[0]) / w) % 1) * 256);
            int pointOffsety = (int)((((sz[1] - bl[1]) / h) % 1) * 256);
            debuglog.Text += string.Format("向右偏移:{0}\r\n", pointOffsetx);
            debuglog.Text += string.Format("向上偏移:{0}\r\n", pointOffsety);
            int c = (int)((sz[0] - bl[0]) / w);
            int r = (int)(Math.Pow(2, z) - 1 - (int)((sz[1] - bl[1]) / h));
            // 图片左下角的像素坐标和网格编号
            MapPoint bottomLeftTileCoords = new MapPoint(c, r);
            bottomLeftTileCoords.Floor();

            // 图片左上角的像素坐标和网格编号
            //MapPoint upperLeftTileCoords = new MapPoint(bottomLeftTileCoords.X, bottomLeftTileCoords.Y - Math.Ceiling(imgFile.Height * 1.0f / 256));
            //upperLeftTileCoords.Floor();
            MapPoint upperLeftTilePixel = new MapPoint(bottomLeftTileCoords.X * 256,
          bottomLeftTileCoords.Y * 256 - imgFile.Height);
            upperLeftTilePixel.Floor();
            MapPoint upperLeftTileCoords = upperLeftTilePixel.Divide(256);
            upperLeftTileCoords.Floor();
            // 图片右上角的像素坐标和网格编号
            MapPoint upperRightPixel = new MapPoint(bottomLeftTileCoords.X * 256 + imgFile.Width,
                bottomLeftTileCoords.Y * 256 + imgFile.Height);
            upperRightPixel.Floor();
            MapPoint upperRightTileCoords = upperRightPixel.Divide(256);
            upperRightTileCoords.Floor();

            int totalCols = (int)(upperRightTileCoords.X - bottomLeftTileCoords.X + 1);
            int totalRows = (int)(bottomLeftTileCoords.Y - upperLeftTileCoords.Y + 1);
            Console.WriteLine("total col and row: " + totalCols + ", " + totalRows);
            debuglog.Text += string.Format("层级个数{0}\r\n", totalCols * totalRows);
            progressBar.Value = 0;
            totalCount = totalCols * totalRows;
            finishCount = 0;
            for (int i = 0; i < totalCols; i++)
            {
                for (int j = 0; j < totalRows; j++)
                {
                    Bitmap img = new Bitmap(256, 256);
                    MapPoint eachTileCoords = new MapPoint(upperLeftTileCoords.X + i, upperLeftTileCoords.Y + j);
                    int offsetX = i * 256 - pointOffsetx;
                    int offsetY = j * 256 - (totalRows * 256 - imgFile.Height - pointOffsety);
                    copyImagePixel(img, imgFile, offsetX, offsetY);
                    string imgFileName = outputPath + "/tiles/" + zoom.ToString() + "/"
                        + eachTileCoords.X.ToString() + "_" + eachTileCoords.Y.ToString() + ".png";
                    img.Save(imgFileName, System.Drawing.Imaging.ImageFormat.Png);
                    img.Dispose();
                    finishCount++;
                    progressBar.Value = finishCount / totalCount;
                }
            }
            if (imgFile != tileImage)
            {
                imgFile.Dispose();
            }
        }

        /// <summary>
        /// 将图片的部分像素复制到目标图像上
        /// </summary>
        /// <param name="destImage">目标图像</param>
        /// <param name="sourceImage">原始图像</param>
        /// <param name="offsetX">原始图像的像素水平偏移值</param>
        /// <param name="offsetY">原始图像的像素竖直偏移值</param>
        private void copyImagePixel(Bitmap destImage, Bitmap sourceImage, int offsetX, int offsetY)
        {
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    // 默认透明色
                    Color color = Color.FromArgb(0, 0, 0, 0);
                    int pixelX = offsetX + i;
                    int pixelY = offsetY + j;
                    if (pixelX >= 0 && pixelX < sourceImage.Width
                        && pixelY >= 0 && pixelY < sourceImage.Height)
                    {
                        color = sourceImage.GetPixel(pixelX, pixelY);
                    }
                    if (isalpha)
                    {
                        color = Color.FromArgb(100, color.R, color.G, color.B);
                    }
                    destImage.SetPixel(i, j, color);
                }
            }
        }

    }
}

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace ExportTitleMap
//{
//    public struct ZoomInfo
//    {
//        public int MinZoom;
//        public int MaxZoom;
//        public int ImageZoom;
//    }
//    public partial class Form1 : Form
//    {
//        TextBox debuglog;
//        TextBox lng;
//        TextBox txtlng;
//        TextBox lat;
//        TextBox txtlat;
//        TextBox fileTxt;
//        TextBox fileFolderTxt; ComboBox m1; ComboBox m2; ComboBox m3;
//        ProgressBar progressBar;
//        private Bitmap tileImage;   //目标图片
//        private string outputPath;  //输出目录
//        private float imgWidth, imgHeight;
//        private const int titleSize = 256;
//        private ZoomInfo zoomInfo = new ZoomInfo();
//        private LatLng blLatLng;
//        private int finishCount = 0;
//        private int totalCount = 0;

//        public bool isalpha = false;
//        public Form1()
//        {
//            Name = "腾讯地图瓦片生成工具";
//            InitializeComponent();
//        }

//        private void Form1_Load(object sender, EventArgs e)
//        {
//            lng = new TextBox();
//            lng.Text = "经度：";
//            lng.Size = new Size(120, 20);
//            lng.Location = new Point(50, 20);
//            lng.Enabled = false;
//            lng.BorderStyle = BorderStyle.None;
//            this.Controls.Add(lng);
//            txtlng = new TextBox();
//            txtlng.Text = "118.065112000000";
//            txtlng.Size = new Size(120, 20);
//            txtlng.Location = new Point(170, 20);
//            this.Controls.Add(txtlng);

//            lat = new TextBox();
//            lat.Text = "纬度：";
//            lat.Size = new Size(120, 20);
//            lat.Location = new Point(50, 45);
//            lat.Enabled = false;
//            lat.BorderStyle = BorderStyle.None;
//            this.Controls.Add(lat);
//            txtlat = new TextBox();
//            txtlat.Text = "24.443130000000";
//            txtlat.Size = new Size(120, 20);
//            txtlat.Location = new Point(170, 45);
//            this.Controls.Add(txtlat);

//            TextBox text = new TextBox();
//            text.Text = "层级：";
//            text.Size = new Size(50, 20);
//            text.Location = new Point(50, 75);
//            text.Enabled = false;
//            text.BorderStyle = BorderStyle.None;
//            this.Controls.Add(text);
//            text = new TextBox();
//            text.Text = "-";
//            text.Size = new Size(10, 20);
//            text.Location = new Point(235, 75);
//            text.Enabled = false;
//            text.BorderStyle = BorderStyle.None;
//            this.Controls.Add(text);
//            m1 = new ComboBox();
//            m1.Location = new Point(170, 70); // 按钮屏幕位置
//            m1.Size = new Size(50, 40);
//            m2 = new ComboBox();
//            m2.Location = new Point(250, 70); // 按钮屏幕位置
//            m2.Size = new Size(50, 40);
//            for (int i = 13; i <= 20; i++)
//            {
//                m1.Items.Add(i);
//                m2.Items.Add(i);
//            }
//            m1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
//            m2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
//            this.Controls.Add(m1);
//            this.Controls.Add(m2);
//            m1.SelectedItem = 16;
//            m2.SelectedItem = 16;

//            text = new TextBox();
//            text.Text = "当前层级：";
//            text.Size = new Size(60, 20);
//            text.Location = new Point(320, 75);
//            text.Enabled = false;
//            text.BorderStyle = BorderStyle.None;
//            this.Controls.Add(text);
//            m3 = new ComboBox();
//            m3.Location = new Point(380, 70); // 按钮屏幕位置
//            m3.Size = new Size(50, 40);
//            for (int i = 4; i <= 20; i++)
//            {
//                m3.Items.Add(i);
//            }
//            m3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
//            this.Controls.Add(m3);
//            m3.SelectedItem = 20;


//            Button fileBtn = new Button();
//            fileBtn.Text = "目标路径"; // 按钮上面文字
//            fileBtn.Location = new Point(50, 105); // 按钮屏幕位置
//            fileBtn.Size = new Size(100, 40);
//            fileBtn.Parent = this;
//            fileBtn.Click += new EventHandler(FileBtn_Click);
//            this.Controls.Add(fileBtn);
//            fileTxt = new TextBox();
//            fileTxt.Text = "";
//            fileTxt.Size = new Size(500, 40);
//            fileTxt.Location = new Point(170, 115);
//            this.Controls.Add(fileTxt);
//            //fileTxt.Text = @"C:\Users\Administrator\Desktop\maptitle.jpg";
//            //tileImage = new Bitmap(fileTxt.Text);
//            //imgWidth = tileImage.Width;
//            //imgHeight = tileImage.Height;

//            Button filefolderBtn = new Button();
//            filefolderBtn.Text = "存档路径"; // 按钮上面文字
//            filefolderBtn.Location = new Point(50, 155); // 按钮屏幕位置
//            filefolderBtn.Size = new Size(100, 40);
//            filefolderBtn.Parent = this;
//            filefolderBtn.Click += new EventHandler(FileFolderBtn_Click);
//            this.Controls.Add(filefolderBtn);
//            fileFolderTxt = new TextBox();
//            fileFolderTxt.Text = "";
//            fileFolderTxt.Size = new Size(500, 40);
//            fileFolderTxt.Location = new Point(170, 170);
//            this.Controls.Add(fileFolderTxt);
//            //fileFolderTxt.Text = @"C:\Users\Administrator\Desktop\nn";
//            //outputPath = fileFolderTxt.Text;


//            Button contectBtn = new Button();
//            contectBtn.Name = "contectBtn1";
//            contectBtn.Text = "运行"; // 按钮上面文字
//            contectBtn.Location = new Point(50, 400); // 按钮屏幕位置
//            contectBtn.Size = new Size(100, 40);
//            contectBtn.Parent = this;
//            contectBtn.Click += new EventHandler(ContectBtn_Click);
//            this.Controls.Add(contectBtn);

//            Button deleteBtn = new Button();
//            deleteBtn.Text = "清除缓存"; // 按钮上面文字
//            deleteBtn.Location = new Point(200, 400); // 按钮屏幕位置
//            deleteBtn.Size = new Size(100, 40);
//            deleteBtn.Parent = this;
//            deleteBtn.Click += new EventHandler(DeleteBtn_Click);
//            this.Controls.Add(deleteBtn);

//            Button contectBtn2 = new Button();
//            contectBtn2.Name = "contectBtn2";
//            contectBtn2.Text = "运行半透明"; // 按钮上面文字
//            contectBtn2.Location = new Point(350, 400); // 按钮屏幕位置
//            contectBtn2.Size = new Size(100, 40);
//            contectBtn2.Parent = this;
//            contectBtn2.Click += new EventHandler(ContectBtn_Click);
//            this.Controls.Add(contectBtn2);

//            debuglog = new TextBox();
//            debuglog.Text = "";
//            debuglog.Size = new Size(650, 170);
//            debuglog.Location = new Point(50, 200);
//            debuglog.WordWrap = true;
//            debuglog.Multiline = true;
//            //debuglog.Enabled = false;
//            //debuglog.BorderStyle = BorderStyle.Fixed3D;
//            this.Controls.Add(debuglog);

//            progressBar = new ProgressBar();
//            progressBar.Value = 0;
//            progressBar.Size = new Size(650, 10);
//            progressBar.Location = new Point(50, 380);
//            progressBar.Minimum = 0;
//            progressBar.Maximum = 1;
//            this.Controls.Add(progressBar);

//        }
//        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            zoomInfo.MinZoom = int.Parse(m1.SelectedItem.ToString());
//        }

//        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            zoomInfo.MaxZoom = int.Parse(m2.SelectedItem.ToString());
//        }

//        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            zoomInfo.ImageZoom = int.Parse(m3.SelectedItem.ToString());
//        }

//        private void FileBtn_Click(object sender, EventArgs e)
//        {
//            OpenFileDialog fd = new OpenFileDialog();
//            fd.Filter = "(图片文件)|*.png;*.jpg";
//            if (fd.ShowDialog() == DialogResult.OK)
//            {
//                string tableName = fd.SafeFileName.Split('.')[0];
//                fileTxt.Text = fd.FileName;
//                tileImage = new Bitmap(fd.FileName);
//                imgWidth = tileImage.Width;
//                imgHeight = tileImage.Height;
//            }
//        }

//        private void FileFolderBtn_Click(object sender, EventArgs e)
//        {
//            FolderBrowserDialog dialog = new FolderBrowserDialog();
//            dialog.Description = "请选择Exl所在文件夹";
//            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
//            {
//                string foldPath = dialog.SelectedPath;
//                fileFolderTxt.Text = foldPath;
//                outputPath = foldPath;
//            }
//        }
//        private void ContectBtn_Click(object sender, EventArgs e)
//        {
//            if ((sender as Button).Name == "contectBtn1")
//            {
//                isalpha = false;
//            }
//            else
//            {
//                isalpha = true;
//            }
//            if (m2.SelectedIndex < m1.SelectedIndex)
//            {
//                MessageBox.Show("最大级不能小于最小级");
//                return;
//            }
//            blLatLng = new LatLng(double.Parse(txtlat.Text), double.Parse(txtlng.Text));//getStart();
//            CheckMap();
//            debuglog.Text += string.Format("经度：{0},纬度：{1}\r\n", blLatLng.Lat, blLatLng.Lng);
//            beginCut();
//            generateHTMLFile();
//            //tileImage.Dispose();
//        }
//        private void DeleteBtn_Click(object sender, EventArgs e)
//        {
//            if (File.Exists(outputPath))
//            {
//                File.Delete(outputPath);
//            }
//        }

//        private void CheckMap()
//        {
//            debuglog.Clear();
//            double[] sz = getXY(118.06505262851715, 24.44439528675154);
//            int z = zoomInfo.MinZoom;
//            double[] bl = getXY(-180, -85.05112877980659);
//            double[] tl = getXY(-180, 85.05112877980659);
//            double[] br = getXY(180, -85.05112877980659);
//            Console.WriteLine(String.Format("坐标BL点：%f,%f\n", bl[0], bl[1]));
//            Console.WriteLine(String.Format("坐标TL点：%f,%f\n", tl[0], tl[1]));
//            Console.WriteLine(String.Format("坐标BR点：%f,%f\n", br[0], br[1]));
//            Console.WriteLine(String.Format("坐标：%f,%f\n", sz[0], sz[1]));
//            double w = (br[0] - bl[0]) / Math.Pow(2, z);//格网宽度
//            double h = (tl[1] - bl[1]) / Math.Pow(2, z);//格网高度
//            Console.WriteLine(String.Format("格网大小：%f x %f\n", w, h));
//            int[] gridxy = new int[2];
//            int c = (int)((sz[0] - bl[0]) / w);
//            int r = (int)((sz[1] - bl[1]) / h);
//            Console.WriteLine(String.Format("对应行列号：%d,%d\n", c, r));
//            double c_d = Math.Floor(c / 16.0);
//            double r_d = Math.Floor(r / 16.0);
//            string url = String.Format("对应请求url:http://p3.map.gtimg.com/maptilesv2/{0}/{1}/{2}/{3}_{4}.png \r\n", z, (int)c_d, (int)r_d, c, r);
//            debuglog.Text += url;
//            Console.WriteLine(String.Format("对应起始位置请求url:http://p3.map.gtimg.com/maptilesv2/{0}/{1}/{2}/{3}_{4}.png \r\n", z, (int)c_d, (int)r_d, c, r));
//        }

//        private double[] getXY(double lon, double lat)
//        {
//            double earthRad = 6378137.0;
//            double x = lon * Math.PI / 180.0 * earthRad;
//            double a = lat * Math.PI / 180.0;
//            double y = earthRad / 2.0 * Math.Log((1.0 + Math.Sin(a)) / (1.0 - Math.Sin(a)));
//            return new double[] { x, y };
//        }
//        public LatLng getStart()
//        {
//            float lat = Convert.ToSingle(txtlat.Text);
//            float lng = Convert.ToSingle(txtlng.Text);
//            return new LatLng(lat, lng);
//        }

//        private void generateHTMLFile()
//        {
//            StreamWriter htmlFile = File.CreateText(outputPath + "/index.html");
//            string path = @"key.txt";
//            using (StreamReader sr = new StreamReader(path, Encoding.Default))
//            {
//                while (!sr.EndOfStream)
//                {
//                    htmlFile.WriteLine(sr.ReadLine());
//                }
//            }
//            htmlFile.WriteLine("<script>");
//            htmlFile.WriteLine("var map;");
//            htmlFile.WriteLine("function init()");
//            htmlFile.WriteLine("{");
//            htmlFile.WriteLine("    var earthlayer = new qq.maps.ImageMapType({");
//            htmlFile.WriteLine("    name: 'tentxun',");
//            htmlFile.WriteLine("    alt: 'tentxun',");
//            htmlFile.WriteLine("    tileSize: new qq.maps.Size(256, 256),");
//            htmlFile.WriteLine(string.Format("    minZoom: {0},", zoomInfo.MinZoom));
//            htmlFile.WriteLine(string.Format("    maxZoom: {0},", zoomInfo.MaxZoom));
//            htmlFile.WriteLine("    opacity: 1,");
//            htmlFile.WriteLine("    getTileUrl: function(tile, zoom) {");
//            htmlFile.WriteLine("     var z = zoom,");
//            htmlFile.WriteLine("            x = tile.x,");
//            htmlFile.WriteLine("            y = tile.y;");
//            htmlFile.WriteLine("    return 'tiles/' + z + '/' + x + '_' + y + '.png';");
//            htmlFile.WriteLine("}");
//            htmlFile.WriteLine("});");
//            htmlFile.WriteLine("map = new qq.maps.Map(document.getElementById(\"container\"), {");
//            htmlFile.WriteLine(string.Format("    center: new qq.maps.LatLng({0}, {1}),", blLatLng.Lat, blLatLng.Lng));
//            htmlFile.WriteLine(string.Format("    zoom: {0}", zoomInfo.ImageZoom));
//            htmlFile.WriteLine("});");
//            htmlFile.WriteLine("map.overlayMapTypes.push(earthlayer);");
//            htmlFile.WriteLine("}");
//            htmlFile.WriteLine("</script>");
//            htmlFile.WriteLine("</head>");
//            htmlFile.WriteLine("<body onload = \"init(); \" >");
//            htmlFile.WriteLine("<div id=\"container\"></div>");
//            htmlFile.WriteLine("<span id = \"info\" ></ span >");
//            htmlFile.WriteLine("</ body >");
//            htmlFile.WriteLine("</ html >");
//            htmlFile.Close();
//        }

//        private void beginCut()
//        {
//            for (int i = zoomInfo.MinZoom; i <= zoomInfo.MaxZoom; i++)
//            {
//                Bitmap image;
//                if (i == zoomInfo.ImageZoom)
//                {
//                    image = tileImage;
//                }
//                else
//                {
//                    // 生成临时图片
//                    Size newSize = new Size();
//                    double factor = Math.Pow(2, i - zoomInfo.ImageZoom);
//                    newSize.Width = (int)Math.Round(imgWidth * factor);
//                    newSize.Height = (int)Math.Round(imgHeight * factor);
//                    //if (newSize.Width < 256 || newSize.Height < 256)
//                    //{
//                    //    // 图片尺寸过小不再切了
//                    //    Console.WriteLine(string.Format("({0},{1})尺寸过小，跳过", newSize.Width, newSize.Height));
//                    //    debuglog.Text += string.Format("({0},{1})尺寸过小，跳过\r\n", newSize.Width, newSize.Height);
//                    //    continue;
//                    //}
//                    Console.WriteLine(tileImage.Height + ", " + tileImage.Width);
//                    image = new Bitmap(tileImage, newSize);
//                }
//                debuglog.Text += string.Format("生成图片\r\n");
//                debuglog.Text += string.Format("开始切片处理{0}层级\r\n", i.ToString());
//                cutImage(image, i);
//                debuglog.Text += string.Format("结束切片处理{0}层级\r\n", i.ToString());
//            }
//        }

//        /// <summary>
//        /// 切某一级别的图
//        /// </summary>
//        /// <param name="imgFile">图片对象</param>
//        /// <param name="zoom">图片对应的级别</param>
//        private void cutImage(Bitmap imgFile, int zoom)
//        {
//            //int halfWidth = (int)Math.Round((double)imgFile.Width / 2);
//            //int halfHeight = (int)Math.Round((double)imgFile.Height / 2);
//            Directory.CreateDirectory(outputPath + "/tiles/" + zoom.ToString());

//            double[] sz = getXY(blLatLng.Lng, blLatLng.Lat);
//            int z = zoom;
//            double[] bl = getXY(-180, -85.05112877980659);
//            double[] tl = getXY(-180, 85.05112877980659);
//            double[] br = getXY(180, -85.05112877980659);
//            double w = (br[0] - bl[0]) / Math.Pow(2, z);//格网宽度
//            double h = (tl[1] - bl[1]) / Math.Pow(2, z);//格网高度
//            int[] gridxy = new int[2];
//            double cx = imgFile.Width/2;
//            double ry = imgFile.Height/2;
//            int pointOffsetx = (int)((((sz[0] - cx - bl[0]) / w) % 1) * 256);
//            int pointOffsety = (int)((((sz[1] - ry - bl[1]) / h) % 1) * 256);
//            debuglog.Text += string.Format("向右偏移:{0}\r\n", pointOffsetx);
//            debuglog.Text += string.Format("向上偏移:{0}\r\n", pointOffsety);
//            int c = (int)((sz[0] - cx - bl[0]) / w);
//            int r = (int)(Math.Pow(2, z) - 1 - (int)((sz[1] - ry - bl[1]) / h));
//            // 图片左下角的像素坐标和网格编号
//            MapPoint bottomLeftTileCoords = new MapPoint(c, r);
//            bottomLeftTileCoords.Floor();

//            // 图片左上角的像素坐标和网格编号
//            //MapPoint upperLeftTileCoords = new MapPoint(bottomLeftTileCoords.X, bottomLeftTileCoords.Y - Math.Ceiling(imgFile.Height * 1.0f / 256));
//            //upperLeftTileCoords.Floor();
//            MapPoint upperLeftTilePixel = new MapPoint(bottomLeftTileCoords.X * 256,
//          bottomLeftTileCoords.Y * 256 - imgFile.Height);
//            upperLeftTilePixel.Floor();
//            MapPoint upperLeftTileCoords = upperLeftTilePixel.Divide(256);
//            upperLeftTileCoords.Floor();
//            // 图片右上角的像素坐标和网格编号
//            MapPoint upperRightPixel = new MapPoint(bottomLeftTileCoords.X * 256 + imgFile.Width,
//                bottomLeftTileCoords.Y * 256 - imgFile.Height);
//            upperRightPixel.Floor();
//            MapPoint upperRightTileCoords = upperRightPixel.Divide(256);
//            upperRightTileCoords.Floor();

//            int totalCols = (int)(upperRightTileCoords.X - bottomLeftTileCoords.X + 1);
//            int totalRows = (int)(bottomLeftTileCoords.Y - upperLeftTileCoords.Y + 1);
//            Console.WriteLine("total col and row: " + totalCols + ", " + totalRows);
//            debuglog.Text += string.Format("层级个数{0}\r\n", totalCols * totalRows);
//            progressBar.Value = 0;
//            totalCount = totalCols * totalRows;
//            finishCount = 0;
//            for (int i = 0; i < totalCols; i++)
//            {
//                for (int j = 0; j < totalRows; j++)
//                {
//                    Bitmap img = new Bitmap(256, 256);
//                    MapPoint eachTileCoords = new MapPoint(upperLeftTileCoords.X + i, upperLeftTileCoords.Y + j);
//                    int offsetX = i * 256 - pointOffsetx;
//                    int offsetY = j * 256 - (totalRows * 256 - imgFile.Height - pointOffsety);
//                    copyImagePixel(img, imgFile, offsetX, offsetY);
//                    string imgFileName = outputPath + "/tiles/" + zoom.ToString() + "/"
//                        + eachTileCoords.X.ToString() + "_" + eachTileCoords.Y.ToString() + ".png";
//                    img.Save(imgFileName, System.Drawing.Imaging.ImageFormat.Png);
//                    img.Dispose();
//                    finishCount++;
//                    progressBar.Value = finishCount / totalCount;
//                }
//            }
//            if (imgFile != tileImage)
//            {
//                imgFile.Dispose();
//            }
//        }

//        private double OffSetCoords(double value, float offset)
//        {
//            return ((value * 256 + offset) * 1.0f / 256);
//        }
//        private double OffsetPoint(double value, float offset)
//        {
//            return ((((value * 256 + offset) * 1.0f / 256) % 1) * 256);
//        }
//        /// <summary>
//        /// 将图片的部分像素复制到目标图像上
//        /// </summary>
//        /// <param name="destImage">目标图像</param>
//        /// <param name="sourceImage">原始图像</param>
//        /// <param name="offsetX">原始图像的像素水平偏移值</param>
//        /// <param name="offsetY">原始图像的像素竖直偏移值</param>
//        private void copyImagePixel(Bitmap destImage, Bitmap sourceImage, int offsetX, int offsetY)
//        {
//            for (int i = 0; i < 256; i++)
//            {
//                for (int j = 0; j < 256; j++)
//                {
//                    // 默认透明色
//                    Color color = Color.FromArgb(0, 0, 0, 0);
//                    int pixelX = offsetX + i;
//                    int pixelY = offsetY + j;
//                    if (pixelX >= 0 && pixelX < sourceImage.Width
//                        && pixelY >= 0 && pixelY < sourceImage.Height)
//                    {
//                        color = sourceImage.GetPixel(pixelX, pixelY);
//                    }
//                    if (isalpha)
//                    {
//                        color = Color.FromArgb(100, color.R, color.G, color.B);
//                    }
//                    destImage.SetPixel(i, j, color);
//                }
//            }
//        }
//    }
//}