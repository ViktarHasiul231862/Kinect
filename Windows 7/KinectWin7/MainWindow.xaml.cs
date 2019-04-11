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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool movie1IsPlaying = false;
        bool movie2IsPlaying = false;

        public MainWindow()
        {
            this.InitializeComponent();

            this.MinWidth = 1200;
            this.MinHeight = 750;
            this.MaxWidth = 1200;
            this.MaxHeight = 750;

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
                labelKosciec1.Content = "Nie wybrano pliku...";
        }

        void timer_Tick2(object sender, EventArgs e)
        {
            if (kosciecVideo2.Source != null)
            {
                if (kosciecVideo2.NaturalDuration.HasTimeSpan)
                    labelKosciec2.Content = String.Format("{0} / {1}", kosciecVideo2.Position.ToString(@"mm\:ss"), kosciecVideo2.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                labelKosciec2.Content = "Nie wybrano pliku...";
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
                if (openFileDialog1.FileName!="")
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

