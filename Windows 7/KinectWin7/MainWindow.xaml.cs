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
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Threading;
    using System.Windows.Shapes;


    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Serializable]
        public class CustomJoint
        {
            public double X;
            public double Y;
            public TrackingState state;
            public enum TrackingState
            {
                NotTracked = 0,
                Inferred = 1,
                Tracked = 2
            }
            public CustomJoint(double X, double Y, TrackingState state)
            {
                this.X = X;
                this.Y = Y;
                this.state = state;
            }
        };
        [Serializable]
        public class SkeletonToRecord
        {

            public Dictionary<int, Dictionary<int, CustomJoint>> frameOfPeople = new Dictionary<int, Dictionary<int, CustomJoint>>();

            public void addJoints(Dictionary<int, CustomJoint> human, int type, double X, double Y, int state)
            {
                human.Add(type, new CustomJoint(X, Y, (CustomJoint.TrackingState)state));
            }

            public void addPerson(int person, Dictionary<int, CustomJoint> joints)
            {
                frameOfPeople.Add(person, joints);
            }
        };

        bool movie1IsPlaying = false;
        bool movie2IsPlaying = false;

        bool kosciec1IsPlaying = false;
        bool kosciec2IsPlaying = false;

        bool isMovieAvi = true;

        // bool recording = false;

        string recordingPath = "";

        private DrawingGroup drawingGroup1;
        private DrawingGroup drawingGroup2;

        private DrawingImage skeletonMovie1;
        private DrawingImage skeletonMovie2;


        List<SkeletonToRecord> allFrames1 = new List<SkeletonToRecord>();
        List<SkeletonToRecord> allFrames2 = new List<SkeletonToRecord>();

        public enum JointType
        {
            SpineBase = 0,
            SpineMid = 1,
            Neck = 2,
            Head = 3,
            ShoulderLeft = 4,
            ElbowLeft = 5,
            WristLeft = 6,
            HandLeft = 7,
            ShoulderRight = 8,
            ElbowRight = 9,
            WristRight = 10,
            HandRight = 11,
            HipLeft = 12,
            KneeLeft = 13,
            AnkleLeft = 14,
            FootLeft = 15,
            HipRight = 16,
            KneeRight = 17,
            AnkleRight = 18,
            FootRight = 19,
            SpineShoulder = 20,
            HandTipLeft = 21,
            ThumbLeft = 22,
            HandTipRight = 23,
            ThumbRight = 24
        }

        private List<Tuple<JointType, JointType>> bones;

        private List<Pen> bodyColors;

        public MainWindow()
        {
            this.InitializeComponent();
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

            this.MinWidth = 1200;
            this.MinHeight = 750;
            this.MaxWidth = 1200;
            this.MaxHeight = 750;

            this.drawingGroup1 = new DrawingGroup();
            this.skeletonMovie1 = new DrawingImage(this.drawingGroup1);

            this.drawingGroup2 = new DrawingGroup();
            this.skeletonMovie2 = new DrawingImage(this.drawingGroup2);

            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += timer_Tick1;
            timer1.Start();

            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += timer_Tick2;
            timer2.Start();
        }

        public ImageSource SkeletonSource1
        {
            get
            {
                return this.skeletonMovie1;
            }
        }

        public ImageSource SkeletonSource2
        {
            get
            {
                return this.skeletonMovie2;
            }
        }

        void timer_Tick1(object sender, EventArgs e)
        {
            if (kosciecVideoAvi1.Source != null)
            {
                if (kosciecVideoAvi1.NaturalDuration.HasTimeSpan)
                    labelKosciec1.Content = String.Format("{0} / {1}", kosciecVideoAvi1.Position.ToString(@"mm\:ss"), kosciecVideoAvi1.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            //   else
            //       labelKosciec1.Content = "Nie wybrano pliku...";
        }

        void timer_Tick2(object sender, EventArgs e)
        {
            if (kosciecVideoAvi2.Source != null)
            {
                if (kosciecVideoAvi2.NaturalDuration.HasTimeSpan)
                    labelKosciec2.Content = String.Format("{0} / {1}", kosciecVideoAvi2.Position.ToString(@"mm\:ss"), kosciecVideoAvi2.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            //  else
            //       labelKosciec2.Content = "Nie wybrano pliku...";
        }


        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static T ReadFromBinaryFile<T>(Stream stream, string filePath)
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }

        private void uploadAviFiles_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideoAvi1.Visibility = Visibility.Visible;
            kosciecVideoAvi2.Visibility = Visibility.Visible;
            kosciecVideoKosciec1.Visibility = Visibility.Hidden;
            kosciecVideoKosciec2.Visibility = Visibility.Hidden;
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
                    kosciecVideoAvi1.Source = new Uri(openFileDialog1.FileName, UriKind.Absolute);
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
                    kosciecVideoAvi2.Source = new Uri(openFileDialog2.FileName, UriKind.Absolute);
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
            isMovieAvi = true;
        }

        private void startKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                if (!movie1IsPlaying)
                {
                    kosciecVideoAvi1.Play();
                    movie1IsPlaying = true;
                }
            }
            else
            {
                System.Threading.Thread t = new System.Threading.Thread(() => playKosciec(kosciecVideoKosciec1, true));
                t.Start();
                // playKosciec(kosciecVideoKosciec1, true);
            }
        }

        private void startKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                if (!movie2IsPlaying)
                {
                    kosciecVideoAvi2.Play();
                    movie2IsPlaying = true;
                }
            }
            else
            {
                playKosciec(kosciecVideoKosciec2, false);
            }
        }

        private void startMovieAll_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                kosciecVideoAvi1.Play();
                kosciecVideoAvi2.Play();
                movie1IsPlaying = true;
                movie2IsPlaying = true;
            }
            else
            {

            }
        }

        private void pauseKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                kosciecVideoAvi1.Pause();
                movie1IsPlaying = false;
            }
            else
            {

            }
        }

        private void pauseKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                kosciecVideoAvi2.Pause();
                movie2IsPlaying = false;
            }
            else
            {

            }
        }

        private void pauseAll_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                kosciecVideoAvi1.Pause();
                kosciecVideoAvi2.Pause();
                movie1IsPlaying = false;
                movie2IsPlaying = false;
            }
            else
            {

            }
        }

        private void stopKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                kosciecVideoAvi1.Stop();
                movie1IsPlaying = false;
            }
            else
            {

            }
        }

        private void stopKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                kosciecVideoAvi2.Stop();
                movie2IsPlaying = false;
            }
        }

        private void stopAll_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi)
            {
                kosciecVideoAvi1.Stop();
                movie1IsPlaying = false;
                kosciecVideoAvi2.Stop();
                movie2IsPlaying = false;
            }
            else
            {

            }
        }

        private void DoUpdates(object sender, EventArgs e)
        {
            // Update canvas
        }

        private void playKosciec(System.Windows.Controls.Image image, bool firstKosciec)
        {
            this.Dispatcher.Invoke(() =>
            {
                int counter = 1;
                using (DrawingContext dc = firstKosciec ? this.drawingGroup1.Open() : this.drawingGroup2.Open())
                {
                    // czarne tło
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, kosciecVideoKosciec1.Width, kosciecVideoKosciec1.Height));
                    List<SkeletonToRecord> frames = firstKosciec ? allFrames1 : allFrames2;
                    foreach (var frame in frames)
                    {
                        for (int body = 0; body < frame.frameOfPeople.Count; ++body)
                        {
                            foreach (var bone in this.bones)
                            {
                                Pen drawPen = this.bodyColors[body];
                                //  dc.DrawLine(drawPen, new Point(frame.frameOfPeople[body][(int)bone.Item1].X*1000+100, frame.frameOfPeople[body][(int)bone.Item1].Y * 1000+100),
                                //     new Point(frame.frameOfPeople[body][(int)bone.Item2].X * 1000+100, frame.frameOfPeople[body][(int)bone.Item2].Y * 1000+100));

                                dc.DrawLine(drawPen, new Point(frame.frameOfPeople[body][(int)bone.Item1].X * 1000 + 100, frame.frameOfPeople[body][(int)bone.Item1].Y * 1000 + 100),
                                  new Point(frame.frameOfPeople[body][(int)bone.Item2].X * 1000 + 100, frame.frameOfPeople[body][(int)bone.Item2].Y * 1000 + 100));

                                image.Source = skeletonMovie1;
                                image.Refresh();
                            }
                        }
                        System.Threading.Thread.Sleep(80);
                        labelKosciec1.Content = counter;
                        labelKosciec1.Refresh();
                        counter++;
                    }
                }
            });
        }

        private void uploadSkeletonFiles_Click(object sender, RoutedEventArgs e)
        {
            allFrames1.Clear();
            allFrames2.Clear();
            kosciecVideoAvi1.Visibility = Visibility.Hidden;
            kosciecVideoAvi2.Visibility = Visibility.Hidden;
            kosciecVideoKosciec1.Visibility = Visibility.Visible;
            kosciecVideoKosciec2.Visibility = Visibility.Visible;

            string path1 = "";
            string path2 = "";
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
                    path1 = openFileDialog1.FileName;
                    
                    if (openFileDialog1.FileName.EndsWith(".kosciec"))
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
                    using (Stream stream1 = File.Open(path1, FileMode.Open))
                    {
                        while (stream1.Position < stream1.Length)
                        {
                            SkeletonToRecord object1 = ReadFromBinaryFile<SkeletonToRecord>(stream1, path1);
                            if (object1 != null)
                                allFrames1.Add(object1);
                        }
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
                    path2 = openFileDialog1.FileName;
                    if (openFileDialog2.FileName.EndsWith(".kosciec"))
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
                    using (Stream stream2 = File.Open(path2, FileMode.Open))
                    {
                        while (stream2.Position < stream2.Length)
                        {
                            SkeletonToRecord object2 = ReadFromBinaryFile<SkeletonToRecord>(stream2, path2);
                            if(object2!=null)
                                allFrames2.Add(object2);
                        }
                    }
                }
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi = false;
        }

        private void recordFakeSkeleton_Click(object sender, RoutedEventArgs e)
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
            for(int i = 0; i<50; ++i)
            {
                Random rand = new Random();
                double temp;
                temp = Convert.ToDouble(rand.Next(20)-10) / 100;
                SkeletonToRecord skeletonToRecord = new SkeletonToRecord();
                Dictionary<int, CustomJoint> recordedJoints = new Dictionary<int, CustomJoint>();
                for (int bone = 0; bone < 25; ++bone)
                {
                    skeletonToRecord.addJoints(recordedJoints, bone, Convert.ToDouble(rand.Next(20) - 10) / 100,
                       Convert.ToDouble(rand.Next(20) - 10) / 100, 2);
                }
                skeletonToRecord.addPerson(0, recordedJoints);
                WriteToBinaryFile<SkeletonToRecord>(recordingPath, skeletonToRecord, true);
            }
        }
    }
}

