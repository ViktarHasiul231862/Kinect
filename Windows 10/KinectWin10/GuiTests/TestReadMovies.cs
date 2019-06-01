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
    public class TestReadMovies
    {
        [TestMethod]
        public void openingCorrectFormatAviInWindow1()
        {
            var app = Application.Launch("KinectWin10.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label = window.Get<Label>("labelKosciec1");

            Assert.IsTrue(label.Text == "");

            var uploadAvi1Button = window.Get<Button>("uploadAvi1_Button");
            uploadAvi1Button.Click();

            string tests_dir = Directory.GetCurrentDirectory();

            var openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            var filePath = Path.Combine(tests_dir, "..", "..","..", "..", "clipKosciec.avi");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            Assert.IsTrue(label.Text == "Nacisnij start");
            Assert.IsTrue(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);


            app.Close();
        }

        [TestMethod]
        public void openingCorrectFormatAviInWindow2()
        {
            var app = Application.Launch("KinectWin10.exe");

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

            Assert.IsTrue(label.Text == "Nacisnij start");
            Assert.IsFalse(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

        [TestMethod]
        public void openingIncorrectAviInWindow1()
        {
            var app = Application.Launch("KinectWin10.exe");

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

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            Assert.IsTrue(label.Text == "Niepoprawny format pliku");
            Assert.IsFalse(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

        [TestMethod]
        public void openingIncorrectAviInWindow2()
        {
            var app = Application.Launch("KinectWin10.exe");

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

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            Assert.IsTrue(label.Text == "Niepoprawny format pliku");
            Assert.IsFalse(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

        [TestMethod]
        public void openingCorrectKosciecInWindow1()
        {
            var app = Application.Launch("KinectWin10.exe");

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

            Assert.IsTrue(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }


        [TestMethod]
        public void openingCorrectKosciecInWindow2()
        {
            var app = Application.Launch("KinectWin10.exe");

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

            Assert.IsFalse(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

        [TestMethod]
        public void openingIncorrectKosciecInWindow1()
        {
            var app = Application.Launch("KinectWin10.exe");

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

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            Assert.IsTrue(label.Text == "Niepoprawny format pliku");
            Assert.IsFalse(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

        [TestMethod]
        public void openingIncorrectKosciecInWindow2()
        {
            var app = Application.Launch("KinectWin10.exe");

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

            var filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            var filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            Assert.IsTrue(label.Text == "Niepoprawny format pliku");
            Assert.IsFalse(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsFalse(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsFalse(window.Get<Button>("pauseAll").Enabled);
            Assert.IsFalse(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

        [TestMethod]
        public void openingBothCorrectAviFiles()
        {
            var app = Application.Launch("KinectWin10.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label1 = window.Get<Label>("labelKosciec1");
            var label2 = window.Get<Label>("labelKosciec2");


            Assert.IsTrue(label1.Text == "");
            Assert.IsTrue(label2.Text == "");


            var uploadButton1 = window.Get<Button>("uploadAvi1_Button");
            var uploadButton2 = window.Get<Button>("uploadAvi2_Button");


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

            uploadButton2.Click();

            openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "clipKosciec.avi");

            filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            Assert.IsTrue(label1.Text == "Nacisnij start");
            Assert.IsTrue(label2.Text == "Nacisnij start");
            Assert.IsTrue(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseAll").Enabled);
            Assert.IsTrue(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

        [TestMethod]
        public void openingBothCorrectKosciecFiles()
        {
            var app = Application.Launch("KinectWin10.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);

            var label1 = window.Get<Label>("labelKosciec1");
            var label2 = window.Get<Label>("labelKosciec2");


            Assert.IsTrue(label1.Text == "");
            Assert.IsTrue(label2.Text == "");


            var uploadButton1 = window.Get<Button>("uploadSkeleton1_Button");
            var uploadButton2 = window.Get<Button>("uploadSkeleton2_Button");


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

            uploadButton2.Click();

            openModalWindow =
                window.ModalWindow("Open", InitializeOption.NoCache);

            window.WaitWhileBusy();
            Assert.IsNotNull(openModalWindow);

            filePath = Path.Combine(tests_dir, "..", "..", "..", "..", "Kosciec.kosciec");

            filenameTextBox =
                openModalWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1148"));
            filenameTextBox.SetValue(filePath);

            openModalWindow.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            Assert.AreNotEqual(label1.Text, "");
            Assert.AreNotEqual(label2.Text, "");
            Assert.IsTrue(window.Get<Button>("startKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec1").Enabled);
            Assert.IsTrue(window.Get<Button>("startKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("stopKosciec2").Enabled);
            Assert.IsTrue(window.Get<Button>("startMovieAll").Enabled);
            Assert.IsTrue(window.Get<Button>("pauseAll").Enabled);
            Assert.IsTrue(window.Get<Button>("stopAll").Enabled);

            app.Close();
        }

    }
}