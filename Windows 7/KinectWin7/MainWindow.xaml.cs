using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using SkeletonFrameManager;
using ReadLoadManager;

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
                    SkeletonFrame frame = allFrames1[currentFrameKosciec1];
                    using (DrawingContext dc = this.drawingGroup1.Open())
                    {
                        // czarne tło
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, kosciecVideoKosciec1.Width, kosciecVideoKosciec1.Height));

                        for (int body = 0; body < frame.getNumberOfBodies(); ++body)
                        {
                            foreach (var bone in this.bones)
                            {
                                Pen drawPen = this.bodyColors[body];

                                dc.DrawLine(drawPen, new Point(frame.getJoint(body, (int)bone.Item1).getX(), frame.getJoint(body, (int)bone.Item1).getY()),
                                    new Point(frame.getJoint(body, (int)bone.Item2).getX(), frame.getJoint(body, (int)bone.Item2).getY()));
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
                    SkeletonFrame frame = allFrames2[currentFrameKosciec2];
                    using (DrawingContext dc = this.drawingGroup2.Open())
                    {
                        // czarne tło
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, kosciecVideoKosciec2.Width, kosciecVideoKosciec2.Height));

                        for (int body = 0; body < frame.getNumberOfBodies(); ++body)
                        {
                            foreach (var bone in this.bones)
                            {
                                Pen drawPen = this.bodyColors[body];

                                dc.DrawLine(drawPen, new Point(frame.getJoint(body, (int)bone.Item1).getX(), frame.getJoint(body, (int)bone.Item1).getY()),
                                  new Point(frame.getJoint(body, (int)bone.Item2).getX(), frame.getJoint(body, (int)bone.Item2).getY()));
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
            for(int i = 0; i<500; ++i)
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
                    if(bone==5 || bone ==6 || bone ==9 || bone ==10)
                    {
                        temp1 = rand.Next(30)-15;
                        temp2 = rand.Next(30)-15;
                    }
                    else
                    {
                        temp1 = rand.Next(10)-5;
                        temp2 = rand.Next(10)-5;
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
                    framesPerSecond1 = 20 * speed1 / 100;
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
                    framesPerSecond2 = 20 * speed2 / 100;
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
            }
            startMovieAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            stopAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            pauseAll.IsEnabled = file1LoadedCorrectly && file2LoadedCorrectly;
            isMovieAvi2 = false;
        }
    }
}

