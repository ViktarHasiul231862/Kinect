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

        private List<Tuple<CustomJointType, CustomJointType>> bones;

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

        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        private const double JointThickness = 3;

        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.InitializeComponent();
            this.bones = new List<Tuple<CustomJointType, CustomJointType>>();
            // tors
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.Head, CustomJointType.Neck));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.Neck, CustomJointType.SpineShoulder));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.SpineShoulder, CustomJointType.SpineMid));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.SpineMid, CustomJointType.SpineBase));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.SpineShoulder, CustomJointType.ShoulderRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.SpineShoulder, CustomJointType.ShoulderLeft));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.SpineBase, CustomJointType.HipRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.SpineBase, CustomJointType.HipLeft));

            // prawa reka
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.ShoulderRight, CustomJointType.ElbowRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.ElbowRight, CustomJointType.WristRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.WristRight, CustomJointType.HandRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.HandRight, CustomJointType.HandTipRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.WristRight, CustomJointType.ThumbRight));

            // lewa reka
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.ShoulderLeft, CustomJointType.ElbowLeft));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.ElbowLeft, CustomJointType.WristLeft));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.WristLeft, CustomJointType.HandLeft));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.HandLeft, CustomJointType.HandTipLeft));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.WristLeft, CustomJointType.ThumbLeft));

            // prawa noga
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.HipRight, CustomJointType.KneeRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.KneeRight, CustomJointType.AnkleRight));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.AnkleRight, CustomJointType.FootRight));

            // lewa noga
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.HipLeft, CustomJointType.KneeLeft));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.KneeLeft, CustomJointType.AnkleLeft));
            this.bones.Add(new Tuple<CustomJointType, CustomJointType>(CustomJointType.AnkleLeft, CustomJointType.FootLeft));

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
            if (kosciecVideoAvi1.Source != null && isMovieAvi1)
            {
                if (kosciecVideoAvi1.NaturalDuration.HasTimeSpan)
                {
                    labelKosciec1.Content = String.Format("{0} / {1}", kosciecVideoAvi1.Position.ToString(@"mm\:ss"), kosciecVideoAvi1.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    sliderKosciec1.Value = kosciecVideoAvi1.Position.Seconds / kosciecVideoAvi1.NaturalDuration.TimeSpan.TotalSeconds * 100;
                }
            }
        }

        void timer_Tick2(object sender, EventArgs e)
        {
            if (kosciecVideoAvi2.Source != null && isMovieAvi2)
            {
                if (kosciecVideoAvi2.NaturalDuration.HasTimeSpan)
                {
                    labelKosciec2.Content = String.Format("{0} / {1}", kosciecVideoAvi2.Position.ToString(@"mm\:ss"), kosciecVideoAvi2.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    sliderKosciec2.Value = kosciecVideoAvi2.Position.Seconds / kosciecVideoAvi2.NaturalDuration.TimeSpan.TotalSeconds * 100;
                }
            }
        }

        void timer_Tick3(object sender, EventArgs e)
        {
            if (!isMovieAvi1)
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

                        this.drawingGroup1.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, kosciecVideoKosciec1.Width, kosciecVideoKosciec1.Height));

                        framesKosciec1.Content = currentFrameKosciec1 + 1;
                        sliderKosciec1.Value = currentFrameKosciec1;

                        labelKosciec1.Content = String.Format("{0} / {1}", string.Format("{0:F1}", (double)currentFrameKosciec1 / allFrames1.Count * (double)allFrames1.Count / framesPerSecond1),
                                string.Format("{0:F1}", (double)allFrames1.Count / framesPerSecond1));
                        if(playKosciecMovie1)
                            currentFrameKosciec1++;
                    }
                    kosciecVideoKosciec1.Source = skeletonMovie1;
                }
                if (currentFrameKosciec1 == allFrames1.Count)
                    playKosciecMovie1 = false;
            }
        }

        void timer_Tick4(object sender, EventArgs e)
        {
            if (!isMovieAvi2)
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

                        this.drawingGroup2.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, kosciecVideoKosciec2.Width, kosciecVideoKosciec2.Height));

                        framesKosciec2.Content = currentFrameKosciec2+1;
                        sliderKosciec2.Value = currentFrameKosciec2;

                        labelKosciec2.Content = String.Format("{0} / {1}", string.Format("{0:F1}", (double)currentFrameKosciec2 / allFrames2.Count * (double)allFrames2.Count / framesPerSecond2),
                            string.Format("{0:F1}", (double)allFrames2.Count / framesPerSecond2));
                        if(playKosciecMovie2)
                                currentFrameKosciec2++;
                    }
                }
                if (currentFrameKosciec2 == allFrames2.Count)
                    playKosciecMovie2 = false;
            }
        }
      
        private void startKosciec1_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                if (!movie1IsPlaying)
                {
                    kosciecVideoAvi1.Play();
                    movie1IsPlaying = true;
                    sliderKosciec1.IsEnabled = false;
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
                    sliderKosciec2.IsEnabled = false;
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
                sliderKosciec1.IsEnabled = false;
            }
            else
            {
                playKosciecMovie1 = true;
            }
            if(isMovieAvi2)
            {
                kosciecVideoAvi2.Play();
                movie2IsPlaying = true;
                sliderKosciec2.IsEnabled = false;
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
                sliderKosciec1.IsEnabled = true;
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
                sliderKosciec2.IsEnabled = true;
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
                sliderKosciec1.IsEnabled = true;
            }
            else
            {
                playKosciecMovie1 = false;
            }
            if(isMovieAvi2)
            {
                kosciecVideoAvi2.Pause();
                movie2IsPlaying = false;
                sliderKosciec2.IsEnabled = true;
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
                sliderKosciec1.IsEnabled = true;
            }
            else
            {
                playKosciecMovie1 = false;
                currentFrameKosciec1 = 0;
                sliderKosciec1.Value = 0;
                labelKosciec1.Content = String.Format("{0} / {1}", string.Format("{0:F1}", 0),
                                string.Format("{0:F1}", (double)allFrames1.Count / framesPerSecond1));
            }
        }

        private void stopKosciec2_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi2)
            {
                kosciecVideoAvi2.Stop();
                movie2IsPlaying = false;
                sliderKosciec2.IsEnabled = true;
            }
            else
            {
                playKosciecMovie2 = false;
                currentFrameKosciec2 = 0;
                sliderKosciec2.Value = 0;
                labelKosciec2.Content = String.Format("{0} / {1}", string.Format("{0:F1}", 0),
                                string.Format("{0:F1}", (double)allFrames2.Count / framesPerSecond2));
            }
        }

        private void stopAll_Click(object sender, RoutedEventArgs e)
        {
            if (isMovieAvi1)
            {
                kosciecVideoAvi1.Stop();
                movie1IsPlaying = false;
                sliderKosciec1.IsEnabled = true;
            }
            else
            {
                playKosciecMovie1 = false;
                currentFrameKosciec1 = 0;
                sliderKosciec1.Value = 0;
                labelKosciec1.Content = String.Format("{0} / {1}", string.Format("{0:F1}", 0),
                                string.Format("{0:F1}", (double)allFrames1.Count / framesPerSecond1));
            }
            if(isMovieAvi2)
            {
                kosciecVideoAvi2.Stop();
                movie2IsPlaying = false;
                sliderKosciec2.IsEnabled = true;
            }
            else
            {
                playKosciecMovie2 = false;
                currentFrameKosciec2 = 0;
                sliderKosciec2.Value = 0;
                labelKosciec2.Content = String.Format("{0} / {1}", string.Format("{0:F1}", 0),
                                string.Format("{0:F1}", (double)allFrames2.Count / framesPerSecond2));
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
                skeletonToRecord.addJointToPerson(0, 1, 150, 90, 1);
                skeletonToRecord.addJointToPerson(0, 2, 135, 30, 1);
                skeletonToRecord.addJointToPerson(0, 3, 125, 3, 1);
                skeletonToRecord.addJointToPerson(0, 4, 180, 60, 2);
                skeletonToRecord.addJointToPerson(0, 5, 195, 105, 2);
                skeletonToRecord.addJointToPerson(0, 6, 210, 150, 2);
                skeletonToRecord.addJointToPerson(0, 7, 215, 170, 2);
                skeletonToRecord.addJointToPerson(0, 8, 105, 60, 2);
                skeletonToRecord.addJointToPerson(0, 9, 90, 110, 2);
                skeletonToRecord.addJointToPerson(0, 10, 85, 175, 1);
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

                if (i > 250)
                {
                    skeletonToRecord.addPerson(1, new Dictionary<int, CustomJoint>());

                    skeletonToRecord.addJointToPerson(1, 0, 160, 150, 2);
                    skeletonToRecord.addJointToPerson(1, 1, 160, 90, 2);
                    skeletonToRecord.addJointToPerson(1, 2, 145, 30, 2);
                    skeletonToRecord.addJointToPerson(1, 3, 135, 3, 2);
                    skeletonToRecord.addJointToPerson(1, 4, 190, 60, 2);
                    skeletonToRecord.addJointToPerson(1, 5, 205, 105, 2);
                    skeletonToRecord.addJointToPerson(1, 6, 220, 150, 2);
                    skeletonToRecord.addJointToPerson(1, 7, 225, 170, 2);
                    skeletonToRecord.addJointToPerson(1, 8, 115, 60, 2);
                    skeletonToRecord.addJointToPerson(1, 9, 100, 110, 2);
                    skeletonToRecord.addJointToPerson(1, 10, 95, 175, 2);
                    skeletonToRecord.addJointToPerson(1, 11, 90, 195, 2);
                    skeletonToRecord.addJointToPerson(1, 12, 190, 165, 2);
                    skeletonToRecord.addJointToPerson(1, 13, 180, 240, 2);
                    skeletonToRecord.addJointToPerson(1, 14, 175, 315, 2);
                    skeletonToRecord.addJointToPerson(1, 15, 190, 320, 2);
                    skeletonToRecord.addJointToPerson(1, 16, 120, 165, 2);
                    skeletonToRecord.addJointToPerson(1, 17, 120, 240, 2);
                    skeletonToRecord.addJointToPerson(1, 18, 130, 315, 2);
                    skeletonToRecord.addJointToPerson(1, 19, 137, 325, 2);
                    skeletonToRecord.addJointToPerson(1, 20, 150, 45, 2);
                    skeletonToRecord.addJointToPerson(1, 21, 235, 180, 2);
                    skeletonToRecord.addJointToPerson(1, 22, 220, 180, 2);
                    skeletonToRecord.addJointToPerson(1, 23, 100, 200, 2);
                    skeletonToRecord.addJointToPerson(1, 24, 85, 200, 2);
                }

                double temp1;
                double temp2;

                double temp3;
                double temp4; 
                for (int bone = 0; bone < 25; ++bone)
                {
                    if(bone==5 || bone ==6 || bone ==9 || bone ==10)
                    {
                        temp1 = rand.Next(30)-15;
                        temp2 = rand.Next(30)-15;
                        temp3 = rand.Next(30) - 15;
                        temp4 = rand.Next(30) - 15;
                    }
                    else
                    {
                        temp1 = rand.Next(10)-5;
                        temp2 = rand.Next(10)-5;
                        temp3 = rand.Next(30) - 15;
                        temp4 = rand.Next(30) - 15;
                    }
                    skeletonToRecord.getJoint(0, bone).setX(skeletonToRecord.getJoint(0, bone).getX() + temp1);
                    skeletonToRecord.getJoint(0, bone).setY(skeletonToRecord.getJoint(0, bone).getY() + temp2);

                    if (i > 250)
                    {
                        skeletonToRecord.getJoint(1, bone).setX(skeletonToRecord.getJoint(1, bone).getX() + temp3);
                        skeletonToRecord.getJoint(1, bone).setY(skeletonToRecord.getJoint(1, bone).getY() + temp4);
                    }
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
                        labelOfFrame1.Visibility = Visibility.Hidden;
                        framesKosciec1.Visibility = Visibility.Hidden;
                        sliderKosciec1.Value = 0;
                        sliderKosciec1.IsEnabled = true;
                        sliderKosciec1.Maximum = 100;
                        sliderKosciec1.IsEnabled = false;
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
                        labelOfFrame1.Visibility = Visibility.Visible;
                        framesKosciec1.Visibility = Visibility.Visible;
                        sliderKosciec1.Value = 0;
                        sliderKosciec1.Maximum = allFrames1.Count;
                        sliderKosciec1.TickFrequency = framesPerSecond1;
                        sliderKosciec1.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight;
                        sliderKosciec1.IsEnabled = true;
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
                    if (openFileDialog.FileName.EndsWith(".avi") || openFileDialog.FileName.EndsWith(".mp4"))
                    {
                        labelKosciec2.Content = "Nacisnij start";
                        startKosciec2.IsEnabled = true;
                        stopKosciec2.IsEnabled = true;
                        pauseKosciec2.IsEnabled = true;
                        speedMovie2.IsEnabled = true;
                        file2LoadedCorrectly = true;
                        labelOfFrame2.Visibility = Visibility.Hidden;
                        framesKosciec2.Visibility = Visibility.Hidden;
                        sliderKosciec2.Value = 0;
                        sliderKosciec2.Maximum = 100;
                        sliderKosciec2.IsEnabled = false;
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
                        labelOfFrame2.Visibility = Visibility.Visible;
                        framesKosciec2.Visibility = Visibility.Visible;
                        sliderKosciec2.Value = 0;
                        sliderKosciec2.Maximum = allFrames2.Count;
                        sliderKosciec2.TickFrequency = framesPerSecond2;
                        sliderKosciec2.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight;
                        sliderKosciec2.IsEnabled = true;
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

        private void sliderKosciec1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isMovieAvi1)
            {
                framesKosciec1.Content = (int)sliderKosciec1.Value;
                currentFrameKosciec1 = (int)sliderKosciec1.Value;
                labelKosciec1.Content = String.Format("{0} / {1}", string.Format("{0:F1}", (double)currentFrameKosciec1 / allFrames1.Count * (double)allFrames1.Count / framesPerSecond1),
                                string.Format("{0:F1}", (double)allFrames1.Count / framesPerSecond1));
            }
            else
            {
                if(!movie1IsPlaying)
                 kosciecVideoAvi1.Position = new TimeSpan(0, 0, 0, 0, (int)(kosciecVideoAvi1.NaturalDuration.TimeSpan.TotalMilliseconds / 100 * sliderKosciec1.Value));
            }
        }

        private void sliderKosciec2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isMovieAvi2)
            {
                framesKosciec2.Content = (int)sliderKosciec2.Value;
                currentFrameKosciec2 = (int)sliderKosciec2.Value;
                labelKosciec2.Content = String.Format("{0} / {1}", string.Format("{0:F1}", (double)currentFrameKosciec2 / allFrames2.Count * (double)allFrames2.Count / framesPerSecond2),
                                string.Format("{0:F1}", (double)allFrames2.Count / framesPerSecond2));
            }
            else
            {
                if (!movie2IsPlaying)
                    kosciecVideoAvi2.Position = new TimeSpan(0, 0, 0, 0, (int)(kosciecVideoAvi2.NaturalDuration.TimeSpan.TotalMilliseconds / 100 * sliderKosciec2.Value));
            }
        }
    }
}

