/* Copyright (C) Politechnika Wroclawska
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * 
 * Sklad grupy:
 * Viktar Hasiul 231862
 * Tobiasz Rumian 226131
 * Łukasz Witowicz 211143
 * Piotr Pawelski 218370
 * Mateusz Mikuszewski 209980
 * 
 * Przedmiot: Projekt zespolowy
 * Termin: czwartek, 9-12
 * Prowadzacy: dr inż. Jan Nikodem
 * 
 * Czerwiec, 2019
 */

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;
using TestStack.White.WindowsAPI;

namespace Tests
{
    [TestClass]
    public class TestRunMovies
    {
        [TestMethod]
        public void openAndRunAvi1()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label = window.Get<Label>("labelKosciec1");

            Assert.IsTrue(label.Text == "");

            var uploadButton = window.Get<Button>("uploadAvi1_Button");
            uploadButton.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var startButton = window.Get<Button>("startKosciec1");
            var pauseButton = window.Get<Button>("pauseKosciec1");
            var stop = window.Get<Button>("stopKosciec1");


            Assert.AreEqual(label.Text, "Nacisnij start");
            startButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label.Text, "Nacisnij start");
            var time1 = label.Text;
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label.Text, time1);
            pauseButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time2);
            startButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time3 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label.Text, time3);
            stop.Click();
            System.Threading.Thread.Sleep(1000);
            var time4 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time4);

            app.Close();
        }

        [TestMethod]
        public void openAndRunAvi2()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label = window.Get<Label>("labelKosciec2");

            Assert.IsTrue(label.Text == "");

            var uploadButton = window.Get<Button>("uploadAvi2_Button");
            uploadButton.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var startButton = window.Get<Button>("startKosciec2");
            var pauseButton = window.Get<Button>("pauseKosciec2");
            var stop = window.Get<Button>("stopKosciec2");


            Assert.AreEqual(label.Text, "Nacisnij start");
            startButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label.Text, "Nacisnij start");
            var time1 = label.Text;
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label.Text, time1);
            pauseButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time2);
            startButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time3 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label.Text, time3);
            stop.Click();
            System.Threading.Thread.Sleep(1000);
            var time4 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time4);

            app.Close();
        }

        [TestMethod]
        public void openAndRunKosciec1()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label = window.Get<Label>("labelKosciec1");

            Assert.IsTrue(label.Text == "");

            var uploadButton = window.Get<Button>("uploadSkeleton1_Button");
            uploadButton.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var startButton = window.Get<Button>("startKosciec1");
            var pauseButton = window.Get<Button>("pauseKosciec1");
            var stop = window.Get<Button>("stopKosciec1");


            Assert.AreNotEqual(label.Text, "");
            startButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label.Text, "Nacisnij start");
            var time1 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label.Text, time1);
            pauseButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time2);
            startButton.Click();
            System.Threading.Thread.Sleep(500);
            var time3 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label.Text, time3);
            stop.Click();
            System.Threading.Thread.Sleep(1000);
            var time4 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time4);

            app.Close();
        }

        [TestMethod]
        public void openAndRunKosciec2()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label = window.Get<Label>("labelKosciec2");

            Assert.IsTrue(label.Text == "");

            var uploadButton = window.Get<Button>("uploadSkeleton2_Button");
            uploadButton.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var startButton = window.Get<Button>("startKosciec2");
            var pauseButton = window.Get<Button>("pauseKosciec2");
            var stop = window.Get<Button>("stopKosciec2");


            Assert.AreNotEqual(label.Text, "");
            startButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label.Text, "Nacisnij start");
            var time1 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label.Text, time1);
            pauseButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time2);
            startButton.Click();
            System.Threading.Thread.Sleep(500);
            var time3 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label.Text, time3);
            stop.Click();
            System.Threading.Thread.Sleep(1000);
            var time4 = label.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label.Text, time4);

            app.Close();
        }

        [TestMethod]
        public void openAndRunBothAviSimultaneously()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label1 = window.Get<Label>("labelKosciec1");
            var label2 = window.Get<Label>("labelKosciec2");

            Assert.IsTrue(label1.Text == "");
            Assert.IsTrue(label2.Text == "");


            var uploadButton1 = window.Get<Button>("uploadAvi1_Button");
            uploadButton1.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var uploadButton2 = window.Get<Button>("uploadAvi2_Button");
            uploadButton2.Click();

           tests_dir = Directory.GetCurrentDirectory();

           openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

           filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

           filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

  

            var startButton = window.Get<Button>("startMovieAll");
            var pauseButton = window.Get<Button>("pauseAll");
            var stop = window.Get<Button>("stopAll");


            Assert.AreNotEqual(label1.Text, "");
            Assert.AreNotEqual(label2.Text, "");

            startButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label1.Text, "Nacisnij start");
            Assert.AreNotEqual(label2.Text, "Nacisnij start");
            var time1_1 = label1.Text;
            var time1_2 = label2.Text;

            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time1_1);
            Assert.AreNotEqual(label2.Text, time1_2);
            pauseButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2_1 = label1.Text;
            var time2_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time2_1);
            Assert.AreEqual(label2.Text, time2_2);
            startButton.Click();
            System.Threading.Thread.Sleep(500);
            var time3_1 = label1.Text;
            var time3_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time3_1);
            Assert.AreNotEqual(label2.Text, time3_2);

            app.Close();
        }

        [TestMethod]
        public void openAndRunBothAviNotSimultaneously()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label1 = window.Get<Label>("labelKosciec1");
            var label2 = window.Get<Label>("labelKosciec2");

            Assert.IsTrue(label1.Text == "");
            Assert.IsTrue(label2.Text == "");


            var uploadButton1 = window.Get<Button>("uploadAvi1_Button");
            uploadButton1.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var uploadButton2 = window.Get<Button>("uploadAvi2_Button");
            uploadButton2.Click();

            tests_dir = Directory.GetCurrentDirectory();

            openModalWindow =
                 window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            filenameTextBox =
                 openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);



            var startAllButton = window.Get<Button>("startMovieAll");
            var pauseAllButton = window.Get<Button>("pauseAll");
            var stopAllButton = window.Get<Button>("stopAll");

            var startButton1 = window.Get<Button>("startKosciec1");
            var pauseButton1 = window.Get<Button>("pauseKosciec1");
            var stopButton1 = window.Get<Button>("stopKosciec1");

            var startButton2 = window.Get<Button>("startKosciec2");
            var pauseButton2 = window.Get<Button>("pauseKosciec2");
            var stopButton2 = window.Get<Button>("stopKosciec2");


            Assert.AreEqual(label1.Text, "Nacisnij start");
            Assert.AreEqual(label2.Text, "Nacisnij start");
            startAllButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label1.Text, "Nacisnij start");
            Assert.AreNotEqual(label2.Text, "Nacisnij start");
            var time1_1 = label1.Text;
            var time1_2 = label2.Text;

            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time1_1);
            Assert.AreNotEqual(label2.Text, time1_2);
            pauseAllButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2_1 = label1.Text;
            var time2_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time2_1);
            Assert.AreEqual(label2.Text, time2_2);
            startButton1.Click();
            System.Threading.Thread.Sleep(1000);
            var time3_1 = label1.Text;
            var time3_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time3_1);
            Assert.AreEqual(label2.Text, time3_2);
            pauseButton1.Click();
            startButton2.Click();
            System.Threading.Thread.Sleep(1000);
            var time4_1 = label1.Text;
            var time4_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time4_1);
            Assert.AreNotEqual(label2.Text, time4_2);

            app.Close();
        }

        [TestMethod]
        public void openAndRunBothKosciecNotSimultaneously()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label1 = window.Get<Label>("labelKosciec1");
            var label2 = window.Get<Label>("labelKosciec2");

            Assert.IsTrue(label1.Text == "");
            Assert.IsTrue(label2.Text == "");


            var uploadButton1 = window.Get<Button>("uploadSkeleton1_Button");
            uploadButton1.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var uploadButton2 = window.Get<Button>("uploadSkeleton2_Button");
            uploadButton2.Click();

            tests_dir = Directory.GetCurrentDirectory();

            openModalWindow =
                 window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            filenameTextBox =
                 openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);



            var startAllButton = window.Get<Button>("startMovieAll");
            var pauseAllButton = window.Get<Button>("pauseAll");
            var stopAllButton = window.Get<Button>("stopAll");

            var startButton1 = window.Get<Button>("startKosciec1");
            var pauseButton1 = window.Get<Button>("pauseKosciec1");
            var stopButton1 = window.Get<Button>("stopKosciec1");

            var startButton2 = window.Get<Button>("startKosciec2");
            var pauseButton2 = window.Get<Button>("pauseKosciec2");
            var stopButton2 = window.Get<Button>("stopKosciec2");


            Assert.AreNotEqual(label1.Text, "");
            Assert.AreNotEqual(label2.Text, "");
            startAllButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label1.Text, "Nacisnij start");
            Assert.AreNotEqual(label2.Text, "Nacisnij start");
            var time1_1 = label1.Text;
            var time1_2 = label2.Text;

            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time1_1);
            Assert.AreNotEqual(label2.Text, time1_2);
            pauseAllButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2_1 = label1.Text;
            var time2_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time2_1);
            Assert.AreEqual(label2.Text, time2_2);
            startButton1.Click();
            System.Threading.Thread.Sleep(1000);
            var time3_1 = label1.Text;
            var time3_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time3_1);
            Assert.AreEqual(label2.Text, time3_2);
            pauseButton1.Click();
            startButton2.Click();
            System.Threading.Thread.Sleep(1000);
            var time4_1 = label1.Text;
            var time4_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time4_1);
            Assert.AreNotEqual(label2.Text, time4_2);
            stopAllButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time5_1 = label1.Text;
            var time5_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time5_1);
            Assert.AreEqual(label2.Text, time5_2);

            app.Close();
        }

        [TestMethod]
        public void openAndRunBothKosciecSimultaneously()
        {
            var app = Application.Launch("KinectWin7.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label1 = window.Get<Label>("labelKosciec1");
            var label2 = window.Get<Label>("labelKosciec2");

            Assert.IsTrue(label1.Text == "");
            Assert.IsTrue(label2.Text == "");


            var uploadButton1 = window.Get<Button>("uploadSkeleton1_Button");
            uploadButton1.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            var uploadButton2 = window.Get<Button>("uploadSkeleton2_Button");
            uploadButton2.Click();

            tests_dir = Directory.GetCurrentDirectory();

            openModalWindow =
                 window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            filenameTextBox =
                 openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);



            var startButton = window.Get<Button>("startMovieAll");
            var pauseButton = window.Get<Button>("pauseAll");
            var stopAllButton = window.Get<Button>("stopAll");


            Assert.AreNotEqual(label1.Text, "");
            Assert.AreNotEqual(label2.Text, "");
            startButton.Click();
            System.Threading.Thread.Sleep(2000);
            Assert.AreNotEqual(label1.Text, "Nacisnij start");
            Assert.AreNotEqual(label2.Text, "Nacisnij start");
            var time1_1 = label1.Text;
            var time1_2 = label2.Text;

            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time1_1);
            Assert.AreNotEqual(label2.Text, time1_2);
            pauseButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time2_1 = label1.Text;
            var time2_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time2_1);
            Assert.AreEqual(label2.Text, time2_2);
            startButton.Click();
            System.Threading.Thread.Sleep(500);
            var time3_1 = label1.Text;
            var time3_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual(label1.Text, time3_1);
            Assert.AreNotEqual(label2.Text, time3_2);
            stopAllButton.Click();
            System.Threading.Thread.Sleep(1000);
            var time4_1 = label1.Text;
            var time4_2 = label2.Text;
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(label1.Text, time4_1);
            Assert.AreEqual(label2.Text, time4_2);

            app.Close();
        }
    }
}