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

using Kinect.Toolbox.Record;

using Microsoft.Kinect;

namespace KinectSetupDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // KinectRecordManager manager;
        public MainWindow()
        {
            InitializeComponent();
            movieGrid.Visibility = Visibility.Hidden;
            liveGrid.Visibility = Visibility.Visible;
            switchSideCombobox.SelectedIndex = 2;
            this.MinWidth = 1200;
            this.MinHeight = 750;
            this.MaxWidth = 1200;
            this.MaxHeight = 750;

            //  manager = new KinectRecordManager();
        }

        private void startRecordingButton_click(object sender, RoutedEventArgs e)
        {
         //   manager.ToggleRecord();
        }

        private void stopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
         //   manager.ToggleRecord();
        }

        private void ComboBoxItem_Selected_Left(object sender, RoutedEventArgs e)
        {
            skeleton_png.Visibility = Visibility.Collapsed;
            human_jpg.Visibility = Visibility.Visible;
            human_jpg.Margin = new Thickness(393, 138, 392, 84);
            skeleton_png.Margin = new Thickness(679, 138, 140, 84);
        }

        private void ComboBoxItem_Selected_Right(object sender, RoutedEventArgs e)
        {
            human_jpg.Visibility = Visibility.Collapsed;
            skeleton_png.Visibility = Visibility.Visible;
            human_jpg.Margin = new Thickness(171, 138, 614, 84);
            skeleton_png.Margin = new Thickness(393, 138, 392, 84);
        }

        private void ComboBoxItem_Selected_Both(object sender, RoutedEventArgs e)
        {
            human_jpg.Visibility = Visibility.Visible;
            skeleton_png.Visibility = Visibility.Visible;
            human_jpg.Margin = new Thickness(171, 138, 614, 84);
            skeleton_png.Margin = new Thickness(679, 138, 140, 84);
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

    class KinectRecordManager
    {
        #region Fields

        private BufferedStream ColorRecordStream;
        private BufferedStream DepthRecordStream;
        private KinectRecorder KinectColorRecorder;
        private KinectRecorder KinectDepthRecorder;
        private KinectSensor KinectSensor;
        private KinectRecorder KinectSkeletonRecorder;
        private BufferedStream SkeletonRecordStream;

        #endregion Fields

        #region Constructors

        public KinectRecordManager()
        {
            Recording = false;
            RecordPath = "";
            RecordFileName = "";
            KinectSensor = KinectSensor.KinectSensors.FirstOrDefault(e => e.Status == KinectStatus.Connected);
            KinectSensor.AllFramesReady += this.OnAllFramesReady;
        }

        #endregion Constructors

        #region Properties

        public String RecordFileName
        {
            get; set;
        }

        public bool Recording
        {
            get; private set;
        }

        public String RecordPath
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void StartRecording()
        {
            if (!Recording)
            {
                System.Diagnostics.Debug.WriteLine("Start recording...");
                string logFile = Path.Combine(RecordPath, RecordFileName + ".log");
                string skeletonFile = Path.Combine(RecordPath, RecordFileName + "_skeleton.data");
                string colorFile = Path.Combine(RecordPath, RecordFileName + "_color.data");
                string depthFile = Path.Combine(RecordPath, RecordFileName + "_depth.data");
                LogStartTime(logFile);
                SkeletonRecordStream = new BufferedStream(new FileStream(skeletonFile, FileMode.Create));
                ColorRecordStream = new BufferedStream(new FileStream(colorFile, FileMode.Create));
                DepthRecordStream = new BufferedStream(new FileStream(depthFile, FileMode.Create));

                KinectSkeletonRecorder = new KinectRecorder(KinectRecordOptions.Skeletons, SkeletonRecordStream);
                KinectColorRecorder = new KinectRecorder(KinectRecordOptions.Color, ColorRecordStream);
                KinectDepthRecorder = new KinectRecorder(KinectRecordOptions.Depth, DepthRecordStream);
                Recording = true;
            }
        }

        public void StopRecording()
        {
            if (Recording)
            {
                System.Diagnostics.Debug.WriteLine("Stop recording...");
                SkeletonRecordStream.Flush();
                ColorRecordStream.Flush();
                DepthRecordStream.Flush();

                KinectSkeletonRecorder.Stop();
                KinectColorRecorder.Stop();
                KinectDepthRecorder.Stop();
                Recording = false;
            }
        }

        public void ToggleRecord()
        {
            if (!Recording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        private void LogStartTime(string logFile)
        {
            File.WriteAllText(logFile, "startTime: " + System.DateTime.Now.Ticks + "\n");
        }

        private void OnAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (Recording)
            {
                using (SkeletonFrame SkeletonFrame = e.OpenSkeletonFrame())
                {
                    if (SkeletonFrame != null)
                        KinectSkeletonRecorder.Record(SkeletonFrame);
                }
                using (ColorImageFrame ColorImageFrame = e.OpenColorImageFrame())
                {
                    if (ColorImageFrame != null)
                        KinectColorRecorder.Record(ColorImageFrame);
                }
                using (DepthImageFrame DepthImageFrame = e.OpenDepthImageFrame())
                {
                    if (DepthImageFrame != null)
                        KinectDepthRecorder.Record(DepthImageFrame);
                }
            }
        }

        #endregion Methods
    }
}
