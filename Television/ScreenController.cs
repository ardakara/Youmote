using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

namespace SkeletalTracking.Television
{
    /// <summary>
    /// Class that controllers what is happening on the users screen.. Paused video, moving videos etc.
    /// </summary>
    public class ScreenController
    {

        private static double SCREEN_CHANGE_DURATION = 1;
        private static int SCREEN_X = 0;
        private static int SCREEN_Y = 0;
        private static int SCREEN_WIDTH = 100;
        private static int SCREEN_HEIGHT = 100;
        private MainWindow _window;
        private Media _currentMedia = Media.NULL_MEDIA;
        private Media _swapMedia = Media.NULL_MEDIA;
        private MediaElement _currentMediaElement = new MediaElement();
        private MediaElement _swapMediaElement = new MediaElement();

        private Storyboard _curLeavingLeftAnimation = new Storyboard();
        private Storyboard _curLeavingRightAnimation = new Storyboard();
        private Storyboard _swapEnteringLeftAnimation = new Storyboard();
        private Storyboard _swapEnteringRightAnimation = new Storyboard();

        private DoubleAnimation _curLeavingLeftDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _curLeavingRightDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _swapEnteringLeftDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _swapEnteringRightDoubleAnimation = new DoubleAnimation();


        public ScreenController(MainWindow window)
        {
            this._window = window;


        }

        private void initializeStoryElements()
        {

            this._curLeavingLeftDoubleAnimation.From = ScreenController.SCREEN_X;
            this._curLeavingLeftDoubleAnimation.To = ScreenController.SCREEN_X - ScreenController.SCREEN_WIDTH;
            this._curLeavingLeftDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._curLeavingLeftDoubleAnimation.FillBehavior = FillBehavior.Stop;
            this._curLeavingLeftDoubleAnimation.Completed += handleAnimationCompleted;
            this._curLeavingLeftAnimation.Children.Add(this._curLeavingLeftDoubleAnimation);

            this._curLeavingRightDoubleAnimation.From = ScreenController.SCREEN_X;
            this._curLeavingRightDoubleAnimation.To = ScreenController.SCREEN_X + ScreenController.SCREEN_WIDTH;
            this._curLeavingRightDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._curLeavingRightDoubleAnimation.FillBehavior = FillBehavior.Stop;
            this._curLeavingRightDoubleAnimation.Completed += handleAnimationCompleted;
            this._curLeavingRightAnimation.Children.Add(this._curLeavingRightDoubleAnimation);

            this._swapEnteringLeftDoubleAnimation.From = ScreenController.SCREEN_X - ScreenController.SCREEN_WIDTH;
            this._swapEnteringLeftDoubleAnimation.To = ScreenController.SCREEN_X;
            this._swapEnteringLeftDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._swapEnteringLeftDoubleAnimation.FillBehavior = FillBehavior.Stop;
            this._swapEnteringLeftDoubleAnimation.Completed += handleAnimationCompleted;
            this._swapEnteringLeftAnimation.Children.Add(this._swapEnteringLeftDoubleAnimation);

            this._swapEnteringRightDoubleAnimation.From = ScreenController.SCREEN_X + ScreenController.SCREEN_WIDTH;
            this._swapEnteringRightDoubleAnimation.To = ScreenController.SCREEN_X;
            this._swapEnteringRightDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._swapEnteringRightDoubleAnimation.FillBehavior = FillBehavior.Stop;
            this._swapEnteringRightDoubleAnimation.Completed += handleAnimationCompleted;
            this._swapEnteringRightAnimation.Children.Add(this._swapEnteringRightDoubleAnimation);
        }

        private void handleAnimationCompleted(object sender, EventArgs e)
        {
            Media tempMedia = this._currentMedia;
            MediaElement tempMediaElement = this._currentMediaElement;
            this._currentMediaElement = this._swapMediaElement;
            this._currentMedia = this._swapMedia;

            this._swapMedia = Media.NULL_MEDIA;
            this._swapMediaElement = tempMediaElement;
            this._swapMediaElement.Opacity = 0;
        }
        private void initializeMediaElements()
        {
            this._currentMediaElement.Height = SCREEN_HEIGHT;
            this._currentMediaElement.Width = SCREEN_WIDTH;
            this._currentMediaElement.Opacity = 0;
            this._currentMediaElement.Name = "CURRENT_MEDIA_ELEMENT";
            this._window.MainCanvas.Children.Add(this._currentMediaElement);

            this._swapMediaElement.Height = SCREEN_HEIGHT;
            this._swapMediaElement.Width = SCREEN_WIDTH;
            this._swapMediaElement.Opacity = 0;
            this._swapMediaElement.Name = "SWAP_MEDIA_ELEMENT";
            this._window.MainCanvas.Children.Add(this._swapMediaElement);
        }

        public void pauseCurrentMedia()
        {
            this._currentMediaElement.Pause();
        }

        public void playCurrentMedia()
        {
            this._currentMediaElement.Play();
        }

        public void fadeOutCurrentMedia()
        {
        }

        public void fadeInCurrentMedia()
        {
        }

        public void turnOn(Media media)
        {
            // fate in current media
        }

        public void turnOff()
        {
            // fade out current media
        }

        public void swapInNewMediaFromLeftToRight(Media media)
        {
            this._swapMedia = media;
            this._swapMediaElement.Source = new System.Uri(this._swapMedia.File);
            this._swapMediaElement.Opacity = 1.0;
            Storyboard.SetTargetName(this._curLeavingRightDoubleAnimation, this._currentMediaElement.Name);
            Storyboard.SetTargetProperty(this._curLeavingRightDoubleAnimation, new PropertyPath(Canvas.LeftProperty));

            Storyboard.SetTargetName(this._swapEnteringLeftDoubleAnimation, this._swapMediaElement.Name);
            Storyboard.SetTargetProperty(this._swapEnteringLeftDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
            this._curLeavingRightAnimation.Begin();
            this._swapEnteringLeftAnimation.Begin();
        }


        public void swapInNewMediaFromRightToLeft(Media media)
        {
            this._swapMedia = media;
            this._swapMediaElement.Source = new System.Uri(this._swapMedia.File);
            this._swapMediaElement.Opacity = 1.0;

            Storyboard.SetTargetName(this._curLeavingLeftDoubleAnimation, this._currentMediaElement.Name);
            Storyboard.SetTargetProperty(this._curLeavingLeftDoubleAnimation, new PropertyPath(Canvas.LeftProperty));

            Storyboard.SetTargetName(this._swapEnteringRightDoubleAnimation, this._swapMediaElement.Name);
            Storyboard.SetTargetProperty(this._swapEnteringRightDoubleAnimation, new PropertyPath(Canvas.LeftProperty));

            this._curLeavingLeftAnimation.Begin();
            this._swapEnteringRightAnimation.Begin();
        }

    }
}
