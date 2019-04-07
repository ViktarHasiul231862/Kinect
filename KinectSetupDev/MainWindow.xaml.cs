using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace KinectSetupDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor = null;

        private ColorFrameReader colorFrameReader = null;

        private WriteableBitmap colorBitmap = null;

        // new Variable
        private WriteableBitmap skeletonBitmap = null;

        private string statusText = null;
 
        public MainWindow()
        {
           
            this.kinectSensor = KinectSensor.GetDefault();

            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            this.kinectSensor.Open();

            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;
            this.DataContext = this;

            this.InitializeComponent();
         
            switchSideCombobox.SelectedIndex = 2;
            this.MinWidth = 1200;
            this.MinHeight = 750;
            this.MaxWidth = 1200;
            this.MaxHeight = 750;

            movieGrid.Visibility = Visibility.Hidden;
            liveGrid.Visibility = this.kinectSensor.IsAvailable ? Visibility.Visible : Visibility.Hidden;
            statusGrid.Visibility = this.kinectSensor.IsAvailable ? Visibility.Hidden : Visibility.Visible;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource humanSource
        {
            get
            {
                return this.colorBitmap;
            }
        }

        //added by Victor
        public ImageSource skeletonSource
        {
            get
            {
                // create this variable
                return this.skeletonBitmap;
            }
        }
        //
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.colorFrameReader != null)
            {
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();
                    }
                }
            }
        }

        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            if (this.kinectSensor.IsAvailable)
            {
                this.StatusText = Properties.Resources.RunningStatusText;
                movieGrid.Visibility = Visibility.Hidden;
                liveGrid.Visibility = Visibility.Visible;
                statusGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                this.StatusText = Properties.Resources.SensorNotAvailableStatusText;
                movieGrid.Visibility = Visibility.Hidden;
                liveGrid.Visibility = Visibility.Hidden;
                statusGrid.Visibility = Visibility.Visible;
            }
        }

        private void startRecordingButton_click(object sender, RoutedEventArgs e)
        {
         //   manager.ToggleRecord();
        }

      //  private void stopRecordingButton_Click(object sender, RoutedEventArgs e)
      //  {
         //   manager.ToggleRecord();
      //  }

        private void ComboBoxItem_Selected_Left(object sender, RoutedEventArgs e)
        {
            skeletonViewBox.Visibility = Visibility.Collapsed;
            humanViewBox.Visibility = Visibility.Visible;
            humanViewBox.Margin = new Thickness(393, 42, 392, 180);
            skeletonViewBox.Margin = new Thickness(679, 42, 140, 180);
        }

        private void ComboBoxItem_Selected_Right(object sender, RoutedEventArgs e)
        {
            humanViewBox.Visibility = Visibility.Collapsed;
            skeletonViewBox.Visibility = Visibility.Visible;
            humanViewBox.Margin = new Thickness(171, 42, 614, 180);
            skeletonViewBox.Margin = new Thickness(393, 42, 392, 180);
        }

        private void ComboBoxItem_Selected_Both(object sender, RoutedEventArgs e)
        {
            humanViewBox.Visibility = Visibility.Visible;
            skeletonViewBox.Visibility = Visibility.Visible;
            humanViewBox.Margin = new Thickness(135, 42, 650, 180);
            skeletonViewBox.Margin = new Thickness(677, 42, 142, 180);
        }

        private void uploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            movieGrid.Visibility = Visibility.Visible;
            liveGrid.Visibility = Visibility.Hidden;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            movieGrid.Visibility = Visibility.Hidden;
            liveGrid.Visibility = Visibility.Visible;
        }
    }
}
