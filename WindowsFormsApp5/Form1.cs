using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Alturos.Yolo;
using Alturos.Yolo.Model;
using Microsoft.ML;
using Tensorflow.Data;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;
        public Form1()
        {
            InitializeComponent();
        }

        private void Start()
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            //videoCaptureDevice.DesiredFrameRate = 20;
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Bitmap sss = new Bitmap(1000,700);
            //Rectangle cloneRect = new Rectangle(0, 0, 1000, 700);
            //PixelFormat format = new sss.PixelFormat();
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            
            Dedect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in filterInfoCollection)
                cboCamera.Items.Add(Device.Name);
            cboCamera.SelectedIndex = 0;
            videoCaptureDevice = new VideoCaptureDevice();
            Start();
        }  

        private void Dedect()
        {
            var ConfigurationDetector = new ConfigurationDetector();
            var config = ConfigurationDetector.Detect();
            var yolo = new YoloWrapper(config);
            var memoryStream = new MemoryStream();
            pictureBox1.Image.Save(memoryStream, ImageFormat.Png);
            var _items = yolo.Detect(memoryStream.ToArray()).ToList();
            AddDetailsToPicture(pictureBox1, _items); 
        }

        private void AddDetailsToPicture(PictureBox pictureBox1, List<YoloItem> items)
        {
            var img = pictureBox1.Image;
            var font = new Font("Arial", 18, FontStyle.Bold);
            var brush = new SolidBrush(Color.Red);
            var graphics = Graphics.FromImage(img);
            int i = 0;
            foreach (var item in items)
            {
                var x = items[i].X;
                var y = items[i].Y;
                var width = item.Width;
                var hight = item.Height;
                var tung = item.Type;
                var rect = new Rectangle(x, y, width, hight);
                var pen = new Pen(Color.LightGreen, 6);
                var Point = new Point(x, y);

                graphics.DrawRectangle(pen, rect);
                graphics.DrawString(item.Type, font, brush, Point);
                i++;
            }
            pictureBox1.Image = img;
        }

    }
}
