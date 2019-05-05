
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

            Assert.IsTrue(window.Get<ListView>("liveGrid").Visible);
            Assert.IsFalse(window.Get<ListView>("movieGrid").Visible);

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

      //      depthOrColorCombobox.SetValue(depthOrColorCombobox.SetValue(depthOrColorCombobox.Items.ToArray()[0]));

            app.Close();
        }
    }
}