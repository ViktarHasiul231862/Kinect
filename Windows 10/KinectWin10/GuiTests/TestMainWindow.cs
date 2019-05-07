
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.Factory;

namespace Tests
{
    [TestClass]
    public class TestMainWindow
    {
        [TestMethod]
        public void visibilityAfterLaunch()
        {
            var app = Application.Launch("KinectWin10.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);
            var depthOrColorCombobox = window.Get<ComboBox>("colorOrDepthCombobox");
            var leftOrRightOrBothCombobox = window.Get<ComboBox>("switchSideCombobox");
            var startRecordingButton = window.Get<Button>("startRecordingButton");
            var stopRecordingButton = window.Get<Button>("stopRecordingButton1");
            var statusLabel = window.Get<Label>("statusLabel");
            var goToMovieView = window.Get<Button>("uploadFilesButton");

            var humanSource = window.Get<ListBox>("humanViewBox");
            var depthSource = window.Get<ListBox>("depthViewBox");
            var skeletonSource = window.Get<ListBox>("skeletonViewBox");

            Assert.IsTrue(goToMovieView.Visible);
            Assert.IsTrue(depthOrColorCombobox.Visible);
            Assert.IsTrue(leftOrRightOrBothCombobox.Visible);
            Assert.IsTrue(startRecordingButton.Visible);
            Assert.IsTrue(stopRecordingButton.Visible);
            Assert.IsTrue(statusLabel.Visible);
            Assert.IsTrue(humanSource.Visible);
            Assert.IsTrue(skeletonSource.Visible);
            Assert.IsFalse(depthSource.Visible);

            Assert.IsTrue(window.Get<ListView>("liveGrid").Visible);
            Assert.IsFalse(window.Get<ListView>("movieGrid").Visible);

            Assert.IsFalse(window.Get<Button>("startKosciec1").Visible);
            Assert.IsFalse(window.Get<Button>("stopKosciec1").Visible);
            Assert.IsFalse(window.Get<Button>("pauseKosciec1").Visible);
            Assert.IsFalse(window.Get<Button>("startKosciec2").Visible);
            Assert.IsFalse(window.Get<Button>("stopKosciec2").Visible);
            Assert.IsFalse(window.Get<Button>("pauseKosciec2").Visible);
            Assert.IsFalse(window.Get<Button>("startMovieAll").Visible);
            Assert.IsFalse(window.Get<Button>("stopAll").Visible);
            Assert.IsFalse(window.Get<Button>("pauseAll").Visible);
            Assert.IsFalse(window.Get<Button>("backButton").Visible);
            Assert.IsFalse(window.Get<Image>("kosciecVideoKosciec1").Visible);
            Assert.IsFalse(window.Get<Image>("kosciecVideoKosciec2").Visible);
            Assert.IsFalse(window.Get<Button>("uploadAvi1_Button").Visible);
            Assert.IsFalse(window.Get<Button>("uploadAvi2_Button").Visible);
            Assert.IsFalse(window.Get<Button>("uploadSkeleton1_Button").Visible);
            Assert.IsFalse(window.Get<Button>("uploadSkeleton2_Button").Visible);
           // window.Get<UIItem>("kosciecVideoAvi1");

            app.Close();
        }

        [TestMethod]
        public void depthOrLiveComboboxChange()
        {
            var app = Application.Launch("KinectWin10.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);
            var depthOrColorCombobox = window.Get<ComboBox>("colorOrDepthCombobox");

            var humanSource = window.Get<ListBox>("humanViewBox");
            var depthSource = window.Get<ListBox>("depthViewBox");
            var skeletonSource = window.Get<ListBox>("skeletonViewBox");

            depthOrColorCombobox.SetValue("Kolorowy obraz");

            Assert.IsTrue(humanSource.Visible);
            Assert.IsFalse(depthSource.Visible);
            Assert.IsTrue(skeletonSource.Visible);

            depthOrColorCombobox.SetValue("Mapa głębokości");

            Assert.IsFalse(humanSource.Visible);
            Assert.IsTrue(depthSource.Visible);
            Assert.IsTrue(skeletonSource.Visible);

            app.Close();
        }

        [TestMethod]
        public void sideViewComboboxChanged()
        {
            var app = Application.Launch("KinectWin10.exe");

            var window = app.GetWindow("Kinect Skeleton Analyst", InitializeOption.NoCache);
            var switchSideCombobox = window.Get<ComboBox>("switchSideCombobox");
            var depthOrColorCombobox = window.Get<ComboBox>("colorOrDepthCombobox");


            var humanSource = window.Get<ListBox>("humanViewBox");
            var depthSource = window.Get<ListBox>("depthViewBox");
            var skeletonSource = window.Get<ListBox>("skeletonViewBox");

            depthOrColorCombobox.SetValue("Kolorowy obraz");
            switchSideCombobox.SetValue("Obie strony");

         //   Assert.IsTrue(humanSource.VerticalSpan, { 0,0,0,0 });

            app.Close();
        }
    }
}