using System;
using System.Collections.Generic;
using System.Linq;


namespace KinectSetupDev
{
    using System.Windows;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Media;
    using System;
    using System.Collections.Generic;
    using System.Windows.Threading;

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

        bool isMovieAvi1 = true;
        bool isMovieAvi2 = true;


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

        bool playKosciecMovie1 = false;
        bool playKosciecMovie2 = false;
        int currentFrameKosciec1 = 0;
        int currentFrameKosciec2 = 0;

        int framesPerSecond1 = 20;
        int framesPerSecond2 = 20;

        DispatcherTimer timer1;
        DispatcherTimer timer2;
        DispatcherTimer timer3;
        DispatcherTimer timer4;

        bool file1LoadedCorrectly = false;
        bool file2LoadedCorrectly = false;


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

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += timer_Tick1;
            timer1.Start();

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += timer_Tick2;
            timer2.Start();

            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(1000/framesPerSecond1);
            timer3.Tick += timer_Tick3;
            timer3.Start();

            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromMilliseconds(1000/framesPerSecond2);
            timer4.Tick += timer_Tick4;
            timer4.Start();
        }

        void timer_Tick1(object sender, EventArgs e)
        {
            if (kosciecVideoAvi1.Source != null)
            {
                if (kosciecVideoAvi1.NaturalDuration.HasTimeSpan)
                    labelKosciec1.Content = String.Format("{0} / {1}", kosciecVideoAvi1.Position.ToString(@"mm\:ss"), kosciecVideoAvi1.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            //else
                //labelKosciec1.Content = "Nie wybrano pliku...";
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

        void timer_Tick3(object sender, EventArgs e)
        {
            if (!isMovieAvi1 && playKosciecMovie1)
            {
                if (currentFrameKosciec1 < allFrames1.Count)
                {
                    SkeletonToRecord frame = allFrames1[currentFrameKosciec1];
                    using (DrawingContext dc = this.drawingGroup1.Open())
                    {
                        // czarne tło
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, kosciecVideoKosciec1.Width, kosciecVideoKosciec1.Height));

                        for (int body = 0; body < frame.frameOfPeople.Count; ++body)
                        {
                            foreach (var bone in this.bones)
                            {
                                Pen drawPen = this.bodyColors[body];

                                dc.DrawLine(drawPen, new Point(frame.frameOfPeople[body][(int)bone.Item1].X * 1000 + 100, frame.frameOfPeople[body][(int)bone.Item1].Y * 1000 + 100),
                                    new Point(frame.frameOfPeople[body][(int)bone.Item2].X * 1000 + 100, frame.frameOfPeople[body][(int)bone.Item2].Y * 1000 + 100));
                                kosciecVideoKosciec1.Source = skeletonMovie1;
                            }
                        }
                        labelKosciec1.Content = String.Format("{0} / {1}", string.Format("{0:F1}",(double)currentFrameKosciec1/allFrames1.Count* (double)allFrames1.Count / framesPerSecond1),
                             string.Format("{0:F1}", (double)allFrames1.Count / framesPerSecond1));
                        currentFrameKosciec1++;
                    }
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
                    SkeletonToRecord frame = allFrames2[currentFrameKosciec2];
                    using (DrawingContext dc = this.drawingGroup2.Open())
                    {
                        // czarne tło
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, kosciecVideoKosciec2.Width, kosciecVideoKosciec2.Height));

                        for (int body = 0; body < frame.frameOfPeople.Count; ++body)
                        {
                            foreach (var bone in this.bones)
                            {
                                Pen drawPen = this.bodyColors[body];

                                dc.DrawLine(drawPen, new Point(frame.frameOfPeople[body][(int)bone.Item1].X * 1000 + 100, frame.frameOfPeople[body][(int)bone.Item1].Y * 1000 + 100),
                                    new Point(frame.frameOfPeople[body][(int)bone.Item2].X * 1000 + 100, frame.frameOfPeople[body][(int)bone.Item2].Y * 1000 + 100));
                                kosciecVideoKosciec2.Source = skeletonMovie2;
                            }
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
            if(isMovieAvi2)
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
            if(isMovieAvi2)
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
            if(isMovieAvi2)
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

        private void speedKosciec1_TextChanged(object sender, RoutedEventArgs e)
        {
            int speed1 = 100;
            if (Int32.TryParse(speedKosciec1.Text, out speed1))
            {
                if (speed1 < 20)
                {
                    speed1 = 20;
                    speedKosciec1.Text = speed1.ToString();
                }
                if (speed1 > 200)
                {
                    speed1 = 200;
                    speedKosciec1.Text = speed1.ToString();
                }
                framesPerSecond1 = 20 * speed1 / 100;
                if (timer3 != null)
                    timer3.Stop();
                timer3 = new DispatcherTimer();
                timer3.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond1);
                timer3.Tick += timer_Tick3;
                timer3.Start();
            }
        }

        private void speedKosciec2_TextChanged(object sender, RoutedEventArgs ee)
        {
            int speed2 = 100;
            if (Int32.TryParse(speedKosciec2.Text, out speed2))
            {
                if (speed2 < 20)
                {
                    speed2 = 20;
                    speedKosciec2.Text = speed2.ToString();
                }
                if (speed2 > 200)
                {
                    speed2 = 200;
                    speedKosciec2.Text = speed2.ToString();
                }
                framesPerSecond2 = 20 * speed2 / 100;
                if (timer4 != null)
                    timer4.Stop();
                timer4 = new DispatcherTimer();
                timer4.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond2);
                timer4.Tick += timer_Tick4;
                timer4.Start();
            }
        }

        private void uploadAvi1_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideoAvi1.Visibility = Visibility.Visible;
            kosciecVideoKosciec1.Visibility = Visibility.Hidden;
            file1LoadedCorrectly = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            //    openFileDialog.Filter = "(*.avi)";

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
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi1 = true;
            speedLabel2.Visibility = Visibility.Hidden;
            speedKosciec2.Visibility = Visibility.Hidden;
        }

        private void uploadSkeleton1_Click(object sender, RoutedEventArgs e)
        {
            allFrames1.Clear();
            kosciecVideoAvi1.Visibility = Visibility.Hidden;
            kosciecVideoKosciec1.Visibility = Visibility.Visible;

            string path = "";
            file1LoadedCorrectly = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            //    openFileDialog.Filter = "(*.avi)";

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
                    using (Stream stream1 = File.Open(path, FileMode.Open))
                    {
                        while (stream1.Position < stream1.Length)
                        {
                            SkeletonToRecord object1 = ReadFromBinaryFile<SkeletonToRecord>(stream1, path);
                            if (object1 != null)
                                allFrames1.Add(object1);
                        }
                    }
                }
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi1 = false;
            speedLabel1.Visibility = Visibility.Visible;
            speedKosciec1.Visibility = Visibility.Visible;
        }

        private void uploadAvi2_Click(object sender, RoutedEventArgs e)
        {
            kosciecVideoAvi2.Visibility = Visibility.Visible;
            kosciecVideoKosciec2.Visibility = Visibility.Hidden;
            file2LoadedCorrectly = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            //    openFileDialog.Filter = "(*.avi)";

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
            speedLabel2.Visibility = Visibility.Hidden;
            speedKosciec2.Visibility = Visibility.Hidden;
        }

        private void uploadSkeleton2_Click(object sender, RoutedEventArgs e)
        {
            allFrames2.Clear();
            kosciecVideoAvi2.Visibility = Visibility.Hidden;
            kosciecVideoKosciec2.Visibility = Visibility.Visible;

            string path = "";
            file2LoadedCorrectly = false;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            //    openFileDialog.Filter = "(*.avi)";

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
                    using (Stream stream2 = File.Open(path, FileMode.Open))
                    {
                        while (stream2.Position < stream2.Length)
                        {
                            SkeletonToRecord object2 = ReadFromBinaryFile<SkeletonToRecord>(stream2, path);
                            if (object2 != null)
                                allFrames2.Add(object2);
                        }
                    }
                }
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi2 = false;
            speedLabel2.Visibility = Visibility.Visible;
            speedKosciec2.Visibility = Visibility.Visible;
        }
    }
}

