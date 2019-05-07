using SkeletonFrameManager;
using ReadLoadManager;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace KinectSetupDev
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool movie1IsPlaying = false;
        bool movie2IsPlaying = false;

        bool isMovieAvi1 = true;
        bool isMovieAvi2 = true;

        string recordingPath = "";

        private DrawingGroup drawingGroup1;
        private DrawingGroup drawingGroup2;

        private DrawingImage skeletonMovie1;
        private DrawingImage skeletonMovie2;

        List<SkeletonFrame> allFrames1 = new List<SkeletonFrame>();
        List<SkeletonFrame> allFrames2 = new List<SkeletonFrame>();

        private List<Tuple<JointType, JointType>> bones;

        private List<Pen> bodyColors;

        bool playKosciecMovie1 = false;
        bool playKosciecMovie2 = false;
        int currentFrameKosciec1 = 0;
        int currentFrameKosciec2 = 0;

        const int defaultFramesPerSecond1 = 35;
        const int defaultFramesPerSecond2 = 35;

        int framesPerSecond1 = defaultFramesPerSecond1;
        int framesPerSecond2 = defaultFramesPerSecond2;

        DispatcherTimer timer1;
        DispatcherTimer timer2;
        DispatcherTimer timer3;
        DispatcherTimer timer4;

        bool file1LoadedCorrectly = false;
        bool file2LoadedCorrectly = false;


        private KinectSensor kinectSensor = null;

        private ColorFrameReader colorFrameReader = null;

        private WriteableBitmap colorBitmap = null;

        /*// promień znaczników rąk
        private const double HandSize = 30;*/

        // grubość połączeń
        private const double JointThickness = 3;

        // grubość krawędzi określających wyjście spoza obszaru kamery
        private const double ClipBoundsThickness = 10;

        private const float InferredZPositionClamp = 0.1f;

        //figury do rysowania znacznika rąk
        // private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        // private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        // private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        // rysowania śledzonych punktow przecięcia
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        private readonly Brush inferredJointBrush = Brushes.Yellow;

        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        // grupa rysujaca do renderowania obrazu kośćca na wyjściu
        private DrawingGroup drawingGroup;

        // to co się wyświetli
        private DrawingImage skeletonSource;

        // mapowanie wspolrzednych
        private CoordinateMapper coordinateMapper = null;

        // odczyt ramek kośćca
        private BodyFrameReader bodyFrameReader = null;

        // tablica na kośćce 
        private Body[] bodies = null;

        private const int MapDepthToByte = 8000 / 256;

        private DepthFrameReader depthFrameReader = null;

        private FrameDescription depthFrameDescription = null;

        private WriteableBitmap depthBitmap = null;

        private byte[] depthPixels = null;

        private int displayWidth;

        private int displayHeight;

        // lista  na kolory dla każdego kośćca

        bool recording = false;

        int penIndex = 0;

        SkeletonFrame skeletonToRecord;

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.kinectSensor = KinectSensor.GetDefault();

            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            this.bones = new List<Tuple<JointType, JointType>>();

            // tors
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // prawa reka
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // lewa reka
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // prawa noga
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // lewa noga
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            // kolory dla kośćców max 6
            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            this.kinectSensor.Open();

            this.drawingGroup = new DrawingGroup();

            this.skeletonSource = new DrawingImage(this.drawingGroup);

            this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();

            this.depthFrameReader.FrameArrived += this.DepthReader_FrameArrived;

            this.depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            this.depthPixels = new byte[this.depthFrameDescription.Width * this.depthFrameDescription.Height];

            this.depthBitmap = new WriteableBitmap(this.depthFrameDescription.Width, this.depthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);

            this.DataContext = this;

            this.InitializeComponent();

            this.statusLabel.Content = this.kinectSensor.IsAvailable ? "Kinect działa"
              : "Kinect nie dziala";

            this.statusLabel.Background = this.kinectSensor.IsAvailable ? Brushes.Green
              : Brushes.Red;

            switchSideCombobox.SelectedIndex = 2;
            colorOrDepthCombobox.SelectedIndex = 0;
            this.MinWidth = 1200;
            this.MinHeight = 750;
            this.MaxWidth = 1200;
            this.MaxHeight = 750;

            movieGrid.Visibility = Visibility.Hidden;
            liveGrid.Visibility = Visibility.Visible;

            this.drawingGroup1 = new DrawingGroup();
            this.skeletonMovie1 = new DrawingImage(this.drawingGroup1);

            this.drawingGroup2 = new DrawingGroup();
            this.skeletonMovie2 = new DrawingImage(this.drawingGroup2);

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += timer_Tick1;
            timer1.Start();

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += timer_Tick2;
            timer2.Start();

            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond1);
            timer3.Tick += timer_Tick3;
            timer3.Start();

            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond2);
            timer4.Tick += timer_Tick4;
            timer4.Start();
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource humanSource
        {
            get
            {
                return this.colorBitmap;
            }
        }

        public ImageSource SkeletonSource
        {
            get
            {
                return this.skeletonSource;
            }
        }

        public ImageSource DepthSource
        {
            get
            {
                return this.depthBitmap;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.colorFrameReader != null)
            {
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }
            if (this.depthFrameReader != null)
            {
                this.depthFrameReader.Dispose();
                this.depthFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        // odczyt ramek kośca
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }
                    // pierwsze wywołania funkcji GetAndRefreshBodyData doda nowy kościeć do tablicy bodies
                    // kośćce będą w niej dopóki nie znikną z pola widzenia sensora, czyli wyniosą null
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // czarne tło
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                    penIndex = 0;
                    skeletonToRecord = new SkeletonFrame();
                    foreach (Body body in this.bodies)
                    {
                        Pen drawPen = this.bodyColors[penIndex++];

                        if (body.IsTracked)
                        {
                            this.DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            foreach (JointType jointType in joints.Keys)
                            {
                                CameraSpacePoint position = joints[jointType].Position;
                                if (position.Z < 0)
                                {
                                    position.Z = InferredZPositionClamp;
                                }

                                DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }

                            if (recording)
                            {
                                skeletonToRecord.addPerson(penIndex, new Dictionary<int, CustomJoint>());
                            }

                            this.DrawBody(joints, jointPoints, dc, drawPen);
                        }
                    }
                    // ochrona rysowania poza polem renderowania
                    if(recording)
                    {
                        RecordManager.WriteToBinaryFile<SkeletonFrame>(recordingPath, skeletonToRecord, true);

                    }
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                }
            }
        }

        // rysowanie torsu
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // rysuj kości
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // rysuj punkty połączeń
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }

                if(recording)
                {
                        skeletonToRecord.addJointToPerson(penIndex, (int)jointType, jointPoints[(JointType)jointType].X,
                           jointPoints[(JointType)jointType].Y, (int)joints[(JointType)jointType].TrackingState);
                }
            }
        }

        // rysuj kości od punktu łączącego joint do innego punktu lączącego
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        // rysuj krawędzie pokazujące wyjście spoza obszaru sensora
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }

        private void DepthReader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            bool depthFrameProcessed = false;

            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    using (Microsoft.Kinect.KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        if (((this.depthFrameDescription.Width * this.depthFrameDescription.Height) == (depthBuffer.Size / this.depthFrameDescription.BytesPerPixel)) &&
                            (this.depthFrameDescription.Width == this.depthBitmap.PixelWidth) && (this.depthFrameDescription.Height == this.depthBitmap.PixelHeight))
                        {
                            ushort maxDepth = ushort.MaxValue;

                            this.ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, depthFrame.DepthMinReliableDistance, maxDepth);
                            depthFrameProcessed = true;
                        }
                    }
                }
            }

            if (depthFrameProcessed)
            {
                this.RenderDepthPixels();
            }
        }

        private unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {
            ushort* frameData = (ushort*)depthFrameData;

            for (int i = 0; i < (int)(depthFrameDataSize / this.depthFrameDescription.BytesPerPixel); ++i)
            {
                ushort depth = frameData[i];

                this.depthPixels[i] = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MapDepthToByte) : 0);
            }
        }

        private void RenderDepthPixels()
        {
            this.depthBitmap.WritePixels(
                new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                this.depthPixels,
                this.depthBitmap.PixelWidth,
                0);
        }

        // odczyt kolorowego obrazu
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

        // czy sensor jest podłączony/dostępny
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            if (this.kinectSensor.IsAvailable)
            {
                this.statusLabel.Content = "Kinect podłączony";
                this.statusLabel.Background = Brushes.Green;
                humanViewBox.Visibility = Visibility.Visible;
                skeletonViewBox.Visibility = Visibility.Visible;
                startRecordingButton.IsEnabled = true;
                stopRecordingButton1.IsEnabled = true;

            }
            else
            {
                this.statusLabel.Content = "Kinect nie podłączony";
                this.statusLabel.Background = Brushes.Red;
                humanViewBox.Visibility = Visibility.Hidden;
                skeletonViewBox.Visibility = Visibility.Hidden;
                startRecordingButton.IsEnabled = false;
                stopRecordingButton1.IsEnabled = false;
            }
        }

        private void startRecordingButton_click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.FileName = "Kosciec1.kosciec";
            if (openFileDialog1.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog1.FileName != "")
                {
                    if (File.Exists(openFileDialog1.FileName))
                    {
                        File.Delete(openFileDialog1.FileName);
                    }
                    recordingPath = openFileDialog1.FileName;
                }
            }
            recording = true;

            if (humanViewBox.Visibility == Visibility.Visible)
            {
                if (this.depthFrameReader != null)
                {
                    this.depthFrameReader.Dispose();
                    this.depthFrameReader = null;
                }
            }
            else if (depthViewBox.Visibility == Visibility.Visible)
            {
                if (this.colorFrameReader != null)
                {
                    this.colorFrameReader.Dispose();
                    this.colorFrameReader = null;
                }
            }
        }

        private void stopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            recording = false;
            if (humanViewBox.Visibility == Visibility.Visible)
            {
                this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();
                this.depthFrameReader.FrameArrived += this.DepthReader_FrameArrived;
            }
            else if(depthViewBox.Visibility == Visibility.Visible)
            {
                this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();
                this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;
            }
        }

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

        private void uploadFilesButton_Click(object sender, RoutedEventArgs e)
        {
            liveGrid.Visibility = Visibility.Hidden;
            movieGrid.Visibility = Visibility.Visible;
            if(this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            movieGrid.Visibility = Visibility.Hidden;
            liveGrid.Visibility = Visibility.Visible;
            humanViewBox.Visibility = Visibility.Visible;
            depthViewBox.Visibility = Visibility.Hidden;
            colorOrDepthCombobox.SelectedItem = 0;
            if (movie1IsPlaying) kosciecVideoAvi1.Stop();
            if (movie2IsPlaying) kosciecVideoAvi2.Stop();
            this.kinectSensor = KinectSensor.GetDefault();
            this.kinectSensor.Open();
        }

        void timer_Tick1(object sender, EventArgs e)
        {
            if (kosciecVideoAvi1.Source != null && isMovieAvi1)
            {
                if (kosciecVideoAvi1.NaturalDuration.HasTimeSpan)
                    labelKosciec1.Content = String.Format("{0} / {1}", kosciecVideoAvi1.Position.ToString(@"mm\:ss"), kosciecVideoAvi1.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            //else
            //labelKosciec1.Content = "Nie wybrano pliku...";
        }

        void timer_Tick2(object sender, EventArgs e)
        {
            if (kosciecVideoAvi2.Source != null && isMovieAvi2)
            {
                if (kosciecVideoAvi2.NaturalDuration.HasTimeSpan)
                    labelKosciec2.Content = String.Format("{0} / {1}", kosciecVideoAvi2.Position.ToString(@"mm\:ss"), kosciecVideoAvi2.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            //  else
            //       labelKosciec2.Content = "Nie wybrano pliku...";
        }

        void timer_Tick3(object sender, EventArgs e)
        {
            if (!isMovieAvi1 && playKosciecMovie1)
            {
                if (currentFrameKosciec1 < allFrames1.Count)
                {
                    SkeletonFrame frame = allFrames1[currentFrameKosciec1];
                    using (DrawingContext dc = this.drawingGroup1.Open())
                    {
                        // czarne tło
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, kosciecVideoKosciec1.Width, kosciecVideoKosciec1.Height));

                        int bodyIndex = 0;
                        foreach (int body in frame.getBodyIds())
                        {
                            Pen drawPen = this.bodyColors[bodyIndex];
                            foreach (var bone in this.bones)
                            {
                                drawPen = this.bodyColors[bodyIndex];
                                if ((frame.getJoint(body, (int)bone.Item1).getTrackingState() != (int)CustomJoint.TrackingState.Tracked) || (frame.getJoint(body, (int)bone.Item2).getTrackingState() != (int)CustomJoint.TrackingState.Tracked))
                                {
                                    drawPen = this.inferredBonePen;
                                }

                                dc.DrawLine(drawPen, new Point(frame.getJoint(body, (int)bone.Item1).getX(), frame.getJoint(body, (int)bone.Item1).getY()),
                                    new Point(frame.getJoint(body, (int)bone.Item2).getX(), frame.getJoint(body, (int)bone.Item2).getY()));

                            }

                            for (int CustomJointType = 0; CustomJointType < 25; ++CustomJointType)
                            {
                                Brush drawBrush = null;

                                int trackingState = allFrames1[currentFrameKosciec1].getJoint(body, CustomJointType).getTrackingState();

                                if (trackingState == (int)CustomJoint.TrackingState.Tracked)
                                {
                                    drawBrush = this.trackedJointBrush;
                                }
                                else if (trackingState == (int)CustomJoint.TrackingState.Inferred)
                                {
                                    drawBrush = this.inferredJointBrush;
                                }

                                if (drawBrush != null)
                                {
                                    dc.DrawEllipse(drawBrush, null, new Point(frame.getJoint(body, CustomJointType).getX(), frame.getJoint(body, CustomJointType).getY()),
                                        JointThickness, JointThickness);
                                }
                            }
                            ++bodyIndex;
                        }
                        labelKosciec1.Content = String.Format("{0} / {1}", string.Format("{0:F1}", (double)currentFrameKosciec1 / allFrames1.Count * (double)allFrames1.Count / framesPerSecond1),
                             string.Format("{0:F1}", (double)allFrames1.Count / framesPerSecond1));
                        currentFrameKosciec1++;
                    }
                    kosciecVideoKosciec1.Source = skeletonMovie1;
                }
            }
            //  else
            //       labelKosciec2.Content = "Nie wybrano pliku...";
        }

        void timer_Tick4(object sender, EventArgs e)
        {
            if (!isMovieAvi2 && playKosciecMovie2)
            {
                if (currentFrameKosciec2 < allFrames2.Count)
                {
                    SkeletonFrame frame = allFrames2[currentFrameKosciec2];
                    using (DrawingContext dc = this.drawingGroup2.Open())
                    {
                        // czarne tło
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, kosciecVideoKosciec2.Width, kosciecVideoKosciec2.Height));

                        int bodyIndex = 0;

                        foreach (int body in frame.getBodyIds())
                        {
                            Pen drawPen = this.bodyColors[bodyIndex];
                            foreach (var bone in this.bones)
                            {
                                drawPen = this.bodyColors[bodyIndex];
                                if ((frame.getJoint(body, (int)bone.Item1).getTrackingState() != (int)CustomJoint.TrackingState.Tracked) || (frame.getJoint(body, (int)bone.Item2).getTrackingState() != (int)CustomJoint.TrackingState.Tracked))
                                {
                                    drawPen = this.inferredBonePen;
                                }

                                dc.DrawLine(drawPen, new Point(frame.getJoint(body, (int)bone.Item1).getX(), frame.getJoint(body, (int)bone.Item1).getY()),
                                  new Point(frame.getJoint(body, (int)bone.Item2).getX(), frame.getJoint(body, (int)bone.Item2).getY()));
                                kosciecVideoKosciec2.Source = skeletonMovie2;
                            }
                            for (int CustomJointType = 0; CustomJointType < 25; ++CustomJointType)
                            {
                                Brush drawBrush = null;

                                int trackingState = allFrames2[currentFrameKosciec2].getJoint(body, CustomJointType).getTrackingState();

                                if (trackingState == (int)CustomJoint.TrackingState.Tracked)
                                {
                                    drawBrush = this.trackedJointBrush;
                                }
                                else if (trackingState == (int)CustomJoint.TrackingState.Inferred)
                                {
                                    drawBrush = this.inferredJointBrush;
                                }

                                if (drawBrush != null)
                                {
                                    dc.DrawEllipse(drawBrush, null, new Point(frame.getJoint(body, CustomJointType).getX(), frame.getJoint(body, CustomJointType).getY()),
                                        JointThickness, JointThickness);
                                }
                            }
                            ++bodyIndex;
                        }
                        labelKosciec2.Content = String.Format("{0} / {1}", string.Format("{0:F1}", (double)currentFrameKosciec2 / allFrames2.Count * (double)allFrames2.Count / framesPerSecond2),
                           string.Format("{0:F1}", (double)allFrames2.Count / framesPerSecond2));
                        currentFrameKosciec2++;
                    }
                }
            }
            //  else
            //       labelKosciec2.Content = "Nie wybrano pliku...";
        }

        private void startKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                if (!movie1IsPlaying)
                {
                    kosciecVideoAvi1.Play();
                    movie1IsPlaying = true;
                }
            }
            else
            {
                playKosciecMovie1 = true;
            }
        }

        private void startKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi2)
            {
                if (!movie2IsPlaying)
                {
                    kosciecVideoAvi2.Play();
                    movie2IsPlaying = true;
                }
            }
            else
            {
                playKosciecMovie2 = true;
            }
        }

        private void startMovieAll_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                kosciecVideoAvi1.Play();
                movie1IsPlaying = true;
            }
            else
            {
                playKosciecMovie1 = true;
            }
            if (isMovieAvi2)
            {
                kosciecVideoAvi2.Play();
                movie2IsPlaying = true;
            }
            else
            {
                playKosciecMovie2 = true;
            }
        }

        private void pauseKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                kosciecVideoAvi1.Pause();
                movie1IsPlaying = false;
            }
            else
            {
                playKosciecMovie1 = false;
            }
        }

        private void pauseKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi2)
            {
                kosciecVideoAvi2.Pause();
                movie2IsPlaying = false;
            }
            else
            {
                playKosciecMovie2 = false;
            }
        }

        private void pauseAll_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                kosciecVideoAvi1.Pause();
                movie1IsPlaying = false;
            }
            else
            {
                playKosciecMovie1 = false;
            }
            if (isMovieAvi2)
            {
                kosciecVideoAvi2.Pause();
                movie2IsPlaying = false;
            }
            else
            {
                playKosciecMovie2 = false;
            }
        }

        private void stopKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                kosciecVideoAvi1.Stop();
                movie1IsPlaying = false;
            }
            else
            {
                playKosciecMovie1 = false;
                currentFrameKosciec1 = 0;
            }
        }

        private void stopKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi2)
            {
                kosciecVideoAvi2.Stop();
                movie2IsPlaying = false;
            }
            else
            {
                playKosciecMovie2 = false;
                currentFrameKosciec2 = 0;
            }
        }

        private void stopAll_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                kosciecVideoAvi1.Stop();
                movie1IsPlaying = false;
            }
            else
            {
                playKosciecMovie1 = false;
                currentFrameKosciec1 = 0;
            }
            if (isMovieAvi2)
            {
                kosciecVideoAvi2.Stop();
                movie2IsPlaying = false;
            }
            else
            {
                playKosciecMovie2 = false;
                currentFrameKosciec2 = 0;
            }
        }

        private void recordFakeSkeleton_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.FileName = "Kosciec1.kosciec";
            if (openFileDialog1.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog1.FileName != "")
                {
                    if (File.Exists(openFileDialog1.FileName))
                    {
                        File.Delete(openFileDialog1.FileName);
                    }
                    recordingPath = openFileDialog1.FileName;
                }
            }
            for (int i = 0; i < 500; ++i)
            {
                SkeletonFrame skeletonToRecord = new SkeletonFrame();
                skeletonToRecord.addPerson(0, new Dictionary<int, CustomJoint>());

                skeletonToRecord.addJointToPerson(0, 0, 150, 150, 2);
                skeletonToRecord.addJointToPerson(0, 1, 150, 90, 2);
                skeletonToRecord.addJointToPerson(0, 2, 135, 30, 2);
                skeletonToRecord.addJointToPerson(0, 3, 125, 3, 2);
                skeletonToRecord.addJointToPerson(0, 4, 180, 60, 2);
                skeletonToRecord.addJointToPerson(0, 5, 195, 105, 2);
                skeletonToRecord.addJointToPerson(0, 6, 210, 150, 2);
                skeletonToRecord.addJointToPerson(0, 7, 215, 170, 2);
                skeletonToRecord.addJointToPerson(0, 8, 105, 60, 2);
                skeletonToRecord.addJointToPerson(0, 9, 90, 110, 2);
                skeletonToRecord.addJointToPerson(0, 10, 85, 175, 2);
                skeletonToRecord.addJointToPerson(0, 11, 80, 195, 2);
                skeletonToRecord.addJointToPerson(0, 12, 180, 165, 2);
                skeletonToRecord.addJointToPerson(0, 13, 170, 240, 2);
                skeletonToRecord.addJointToPerson(0, 14, 165, 315, 2);
                skeletonToRecord.addJointToPerson(0, 15, 180, 320, 2);
                skeletonToRecord.addJointToPerson(0, 16, 110, 165, 2);
                skeletonToRecord.addJointToPerson(0, 17, 110, 240, 2);
                skeletonToRecord.addJointToPerson(0, 18, 120, 315, 2);
                skeletonToRecord.addJointToPerson(0, 19, 127, 325, 2);
                skeletonToRecord.addJointToPerson(0, 20, 140, 45, 2);
                skeletonToRecord.addJointToPerson(0, 21, 225, 180, 2);
                skeletonToRecord.addJointToPerson(0, 22, 210, 180, 2);
                skeletonToRecord.addJointToPerson(0, 23, 90, 200, 2);
                skeletonToRecord.addJointToPerson(0, 24, 75, 200, 2);

                double temp1;
                double temp2;
                for (int bone = 0; bone < 25; ++bone)
                {
                    if (bone == 5 || bone == 6 || bone == 9 || bone == 10)
                    {
                        temp1 = rand.Next(30) - 15;
                        temp2 = rand.Next(30) - 15;
                    }
                    else
                    {
                        temp1 = rand.Next(10) - 5;
                        temp2 = rand.Next(10) - 5;
                    }
                    skeletonToRecord.getJoint(0, bone).setX(skeletonToRecord.getJoint(0, bone).getX() + temp1);
                    skeletonToRecord.getJoint(0, bone).setY(skeletonToRecord.getJoint(0, bone).getY() + temp1);
                }

                RecordManager.WriteToBinaryFile<SkeletonFrame>(recordingPath, skeletonToRecord, true);
            }
        }

        private void speedKosciec1_TextChanged(object sender, RoutedEventArgs e)
        {
            int speed1 = 100;
            if (Int32.TryParse(speedMovie1.Text, out speed1))
            {
                if (speed1 < 5)
                {
                    speed1 = 5;
                    speedMovie1.Text = speed1.ToString();
                }
                if (speed1 > 200)
                {
                    speed1 = 200;
                    speedMovie1.Text = speed1.ToString();
                }
                if (!isMovieAvi1)
                {
                    framesPerSecond1 = defaultFramesPerSecond1 * speed1 / 100;
                    if (timer3 != null)
                        timer3.Stop();
                    timer3 = new DispatcherTimer();
                    timer3.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond1);
                    timer3.Tick += timer_Tick3;
                    timer3.Start();
                }
                else
                {
                    kosciecVideoAvi1.SpeedRatio = (double)speed1 / 100;
                }
            }
        }

        private void speedKosciec2_TextChanged(object sender, RoutedEventArgs ee)
        {
            int speed2 = 100;
            if (Int32.TryParse(speedMovie2.Text, out speed2))
            {
                if (speed2 < 5)
                {
                    speed2 = 5;
                    speedMovie2.Text = speed2.ToString();
                }
                if (speed2 > 200)
                {
                    speed2 = 200;
                    speedMovie2.Text = speed2.ToString();
                }
                if (!isMovieAvi2)
                {
                    framesPerSecond2 = defaultFramesPerSecond2 * speed2 / 100;
                    if (timer4 != null)
                        timer4.Stop();
                    timer4 = new DispatcherTimer();
                    timer4.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond2);
                    timer4.Tick += timer_Tick4;
                    timer4.Start();
                }
                else
                {
                    kosciecVideoAvi2.SpeedRatio = (double)speed2 / 100;
                }
            }
        }

        private void uploadAvi1_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideoAvi1.Visibility = Visibility.Visible;
            kosciecVideoKosciec1.Visibility = Visibility.Hidden;
            file1LoadedCorrectly = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Avi Files (*.avi)|*.avi";

            if (openFileDialog.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog.FileName != "")
                {
                    kosciecVideoAvi1.Source = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    if (openFileDialog.FileName.EndsWith(".avi"))
                    {
                        labelKosciec1.Content = "Nacisnij start";
                        startKosciec1.IsEnabled = true;
                        stopKosciec1.IsEnabled = true;
                        pauseKosciec1.IsEnabled = true;
                        speedMovie1.IsEnabled = true;
                        file1LoadedCorrectly = true;
                    }
                    else
                    {
                        labelKosciec1.Content = "Niepoprawny format pliku";
                        startKosciec1.IsEnabled = false;
                        stopKosciec1.IsEnabled = false;
                        pauseKosciec1.IsEnabled = false;
                        speedMovie1.IsEnabled = false;
                        file1LoadedCorrectly = false;
                    }
                }
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi1 = true;
        }

        private void uploadSkeleton1_Click(object sender, RoutedEventArgs e)
        {
            allFrames1.Clear();
            kosciecVideoAvi1.Visibility = Visibility.Hidden;
            kosciecVideoKosciec1.Visibility = Visibility.Visible;
            playKosciecMovie1 = false;
            currentFrameKosciec1 = 0;

            string path = "";
            file1LoadedCorrectly = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Kosciec Files (*.kosciec)|*.kosciec";

            if (openFileDialog.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog.FileName != "")
                {
                    path = openFileDialog.FileName;

                    if (openFileDialog.FileName.EndsWith(".kosciec"))
                    {
                        labelKosciec1.Content = "Nacisnij start";
                        startKosciec1.IsEnabled = true;
                        stopKosciec1.IsEnabled = true;
                        pauseKosciec1.IsEnabled = true;
                        speedMovie1.IsEnabled = true;
                        file1LoadedCorrectly = true;
                        using (Stream stream1 = File.Open(path, FileMode.Open))
                        {
                            while (stream1.Position < stream1.Length)
                            {
                                SkeletonFrame object1 = RecordManager.ReadFromBinaryFile<SkeletonFrame>(stream1, path);
                                if (object1 != null)
                                    allFrames1.Add(object1);
                            }
                        }
                    }
                    else
                    {
                        labelKosciec1.Content = "Niepoprawny format pliku";
                        startKosciec1.IsEnabled = false;
                        stopKosciec1.IsEnabled = false;
                        pauseKosciec1.IsEnabled = false;
                        speedMovie1.IsEnabled = false;
                        file1LoadedCorrectly = false;
                    }
                }
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi1 = false;
        }

        private void uploadAvi2_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideoAvi2.Visibility = Visibility.Visible;
            kosciecVideoKosciec2.Visibility = Visibility.Hidden;
            file2LoadedCorrectly = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Avi Files (*.avi)|*.avi";

            if (openFileDialog.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog.FileName != "")
                {
                    kosciecVideoAvi2.Source = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    if (openFileDialog.FileName.EndsWith(".avi"))
                    {
                        labelKosciec2.Content = "Nacisnij start";
                        startKosciec2.IsEnabled = true;
                        stopKosciec2.IsEnabled = true;
                        pauseKosciec2.IsEnabled = true;
                        speedMovie2.IsEnabled = true;
                        file2LoadedCorrectly = true;
                    }
                    else
                    {
                        labelKosciec2.Content = "Niepoprawny format pliku";
                        startKosciec2.IsEnabled = false;
                        stopKosciec2.IsEnabled = false;
                        pauseKosciec2.IsEnabled = false;
                        file2LoadedCorrectly = false;
                    }
                }
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi2 = true;
        }

        private void uploadSkeleton2_Click(object sender, RoutedEventArgs e)
        {
            allFrames2.Clear();
            kosciecVideoAvi2.Visibility = Visibility.Hidden;
            kosciecVideoKosciec2.Visibility = Visibility.Visible;
            playKosciecMovie2 = false;
            currentFrameKosciec2 = 0;

            string path = "";
            file2LoadedCorrectly = false;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Kosciec Files (*.kosciec)|*.kosciec";

            if (openFileDialog.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog.FileName != "")
                {
                    path = openFileDialog.FileName;
                    if (openFileDialog.FileName.EndsWith(".kosciec"))
                    {
                        labelKosciec2.Content = "Nacisnij start";
                        startKosciec2.IsEnabled = true;
                        stopKosciec2.IsEnabled = true;
                        pauseKosciec2.IsEnabled = true;
                        speedMovie2.IsEnabled = true;
                        file2LoadedCorrectly = true;
                        using (Stream stream2 = File.Open(path, FileMode.Open))
                        {
                            while (stream2.Position < stream2.Length)
                            {
                                SkeletonFrame object2 = RecordManager.ReadFromBinaryFile<SkeletonFrame>(stream2, path);
                                if (object2 != null)
                                    allFrames2.Add(object2);
                            }
                        }
                    }
                    else
                    {
                        labelKosciec2.Content = "Niepoprawny format pliku";
                        startKosciec2.IsEnabled = false;
                        stopKosciec2.IsEnabled = false;
                        pauseKosciec2.IsEnabled = false;
                        speedMovie2.IsEnabled = false;
                        file2LoadedCorrectly = false;
                    }
                }
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi2 = false;
        }

        private void ComboBoxItem_Selected_Color(object sender, RoutedEventArgs e)
        {
            humanViewBox.Visibility = Visibility.Visible;
            depthViewBox.Visibility = Visibility.Hidden;
        }

        private void ComboBoxItem_Selected_Depth(object sender, RoutedEventArgs e)
        {
            humanViewBox.Visibility = Visibility.Hidden;
            depthViewBox.Visibility = Visibility.Visible;
        }
    }

}

