using System;
using System.Collections.Generic;
using System.Linq;


namespace KinectSetupDev
{
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.IO;
    using System.IO.Compression;
    using System.Windows.Forms;
    using System.ComponentModel;
    using Microsoft.Kinect;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor = null;

        private ColorFrameReader colorFrameReader = null;

        private WriteableBitmap colorBitmap = null;

        // status kinecta
        private string statusText = null;

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

        private List<Tuple<JointType, JointType>> bones;

        private int displayWidth;

        private int displayHeight;

        // lista  na kolory dla każdego kośćca
        private List<Pen> bodyColors;

        bool movie1IsPlaying = false;
        bool movie2IsPlaying = false;

        public MainWindow()
        {
                      
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

            this.DataContext = this;

            this.InitializeComponent();

            this.statusLabel.Content = this.kinectSensor.IsAvailable ? "Kinect działa"
              : "Kinect nie dziala";

            this.statusLabel.Background = this.kinectSensor.IsAvailable ? Brushes.Green
              : Brushes.Red;

            switchSideCombobox.SelectedIndex = 2;
            this.MinWidth = 1200;
            this.MinHeight = 750;
            this.MaxWidth = 1200;
            this.MaxHeight = 750;

            movieGrid.Visibility = Visibility.Hidden;
            liveGrid.Visibility = Visibility.Visible;

            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += timer_Tick1;
            timer1.Start();

            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += timer_Tick2;
            timer2.Start();
        }

        void timer_Tick1(object sender, EventArgs e)
        {
            if (kosciecVideo1.Source != null)
            {
                if (kosciecVideo1.NaturalDuration.HasTimeSpan)
                    labelKosciec1.Content = String.Format("{0} / {1}", kosciecVideo1.Position.ToString(@"mm\:ss"), kosciecVideo1.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                labelKosciec1.Content = "No file selected...";
        }

        void timer_Tick2(object sender, EventArgs e)
        {
            if (kosciecVideo2.Source != null)
            {
                if (kosciecVideo2.NaturalDuration.HasTimeSpan)
                    labelKosciec2.Content = String.Format("{0} / {1}", kosciecVideo2.Position.ToString(@"mm\:ss"), kosciecVideo2.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                labelKosciec2.Content = "No file selected...";
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

                    int penIndex = 0;
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

                            this.DrawBody(joints, jointPoints, dc, drawPen);

                            // możliwość dodania znaczników na jakiś punktach np. na rękach
                            /*this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);*/
                        }
                    }
                    // ochrona rysowania poza polem renderowania
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

        // rysuj znaczniki na rękach (opcjonalne)
        /*private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }*/

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
         //   manager.ToggleRecord();
        }

        private void stopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
         //   manager.ToggleRecord();
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

        private void uploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            bool file1LoadedCorrectly = false;
            bool file2LoadedCorrectly = false;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            //    openFileDialog.Filter = "(*.avi)";

            if (openFileDialog1.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog1.FileName != "")
                {
                    kosciecVideo1.Source = new Uri(openFileDialog1.FileName, UriKind.Absolute);
                    if (openFileDialog1.FileName.EndsWith(".avi"))
                    {
                        labelKosciec1.Content = "Nacisnij start";
                        startKosciec1.IsEnabled = true;
                        stopKosciec1.IsEnabled = true;
                        pauseKosciec1.IsEnabled = true;
                        file1LoadedCorrectly = true;
                    }
                    else
                    {
                        labelKosciec1.Content = "Niepoprawny format pliku";
                        startKosciec1.IsEnabled = false;
                        stopKosciec1.IsEnabled = false;
                        pauseKosciec1.IsEnabled = false;
                        file1LoadedCorrectly = false;
                    }
                }
            }
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.InitialDirectory = "C:\\";
            //    openFileDialog.Filter = "(*.avi)";

            if (openFileDialog2.ShowDialog() != 0)
            {
                //Get the path of specified file
                if (openFileDialog2.FileName != "")
                {
                    kosciecVideo2.Source = new Uri(openFileDialog2.FileName, UriKind.Absolute);
                    if (openFileDialog2.FileName.EndsWith(".avi"))
                    {
                        labelKosciec2.Content = "Nacisnij start";
                        startKosciec2.IsEnabled = true;
                        stopKosciec2.IsEnabled = true;
                        pauseKosciec2.IsEnabled = true;
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
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            movieGrid.Visibility = Visibility.Hidden;
            liveGrid.Visibility = Visibility.Visible;
            if (movie1IsPlaying) kosciecVideo1.Stop();
            if (movie2IsPlaying) kosciecVideo2.Stop();
        }

        private void startKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (!movie1IsPlaying)
            {
                kosciecVideo1.Play();
                movie1IsPlaying = true;
            }
        }

        private void startKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (!movie2IsPlaying)
            {
                kosciecVideo2.Play();
                movie2IsPlaying = true;
            }
        }

        private void startMovieAll_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideo1.Play();
            kosciecVideo2.Play();
            movie1IsPlaying = true;
            movie2IsPlaying = true;
        }

        private void pauseKosciec1_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideo1.Pause();
            movie1IsPlaying = false;
        }

        private void pauseKosciec2_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideo2.Pause();
            movie2IsPlaying = false;
        }

        private void pauseAll_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideo1.Pause();
            kosciecVideo2.Pause();
            movie1IsPlaying = false;
            movie2IsPlaying = false;
        }

        private void stopKosciec1_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideo1.Stop();
            movie1IsPlaying = false;
        }

        private void stopKosciec2_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideo2.Stop();
            movie2IsPlaying = false;
        }

        private void stopAll_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideo1.Stop();
            movie1IsPlaying = false;
            kosciecVideo2.Stop();
            movie2IsPlaying = false;
        }
    }
}

