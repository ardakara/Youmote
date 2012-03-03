using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Shapes;
using YouMote;

namespace Youmote.Television
{
    /// <summary>
    /// Class that controllers what is happening on the users screen.. Paused video, moving videos etc.
    /// </summary>
    public class ScreenController
    {

        private static double FADE_OUT_DURATION = 5.0;
        private static double FADE_IN_DURATION = 3.0;
        private static double SCREEN_CHANGE_DURATION = 1.0;
        private double screenX = 0;
        private double screenY = 0;
        private double screenWidth;
        private double screenHeight;
        private MainWindow _window;
        private Media _currentMedia = Media.NULL_MEDIA;
        public Media CurrentMedia
        {
            get
            {
                return this._currentMedia;
            }
            set
            {
                this._currentMedia = value;

                this._currentMediaElement.Source = new System.Uri(this._currentMedia.File);
                this._currentMediaElement.Position = TimeSpan.FromSeconds(this._currentMedia.CurrentTime);
            }
        }
        private Media _swapMedia = Media.NULL_MEDIA;
        public Media SwapMedia
        {
            get
            {
                return this._swapMedia;
            }
            set
            {
                this._swapMedia = value;
                this._swapMediaElement.Source = new System.Uri(this._swapMedia.File);
                this._swapMediaElement.Position = TimeSpan.FromSeconds(this._swapMedia.CurrentTime);
            }
        }

        private MediaElement _currentMediaElement;
        private MediaElement _swapMediaElement;
        private Canvas _currentContainer;
        private Canvas _swapContainer;

        private Storyboard _fadeOutStoryboard = new Storyboard();
        private Storyboard _fadeInStoryboard = new Storyboard();
        private Storyboard _curMoveLeftStoryboard = new Storyboard();
        private Storyboard _curMoveRightStoryboard = new Storyboard();
        private Storyboard _swapMoveRightStoryboard = new Storyboard();
        private Storyboard _swapMoveLeftStoryboard = new Storyboard();

        private DoubleAnimation _fadeInDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _fadeOutDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _curMoveLeftDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _curMoveRightDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _swapMoveRightDoubleAnimation = new DoubleAnimation();
        private DoubleAnimation _swapMoveLeftDoubleAnimation = new DoubleAnimation();


        public ScreenController(MainWindow window)
        {
            this._window = window;
            this.screenWidth = window.MainCanvas.Width;
            this.screenHeight = window.MainCanvas.Height;
            this.initializeMediaElements();
            this.initializeStoryElements();
        }

        private void initializeStoryElements()
        {
            this._fadeInDoubleAnimation.From = 0.0;
            this._fadeInDoubleAnimation.To = 1.0;
            this._fadeInDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.FADE_IN_DURATION));
            this._fadeInDoubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            this._fadeInStoryboard.Children.Add(this._fadeInDoubleAnimation);

            this._fadeOutDoubleAnimation.From = 1.0;
            this._fadeOutDoubleAnimation.To = 0.0;
            this._fadeOutDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.FADE_OUT_DURATION));
            this._fadeOutDoubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            this._fadeOutStoryboard.Children.Add(this._fadeOutDoubleAnimation);

            this._curMoveLeftDoubleAnimation.From = this.screenX;
            this._curMoveLeftDoubleAnimation.To = this.screenX - this.screenWidth;
            this._curMoveLeftDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._curMoveLeftDoubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            this._curMoveLeftStoryboard.Children.Add(this._curMoveLeftDoubleAnimation);

            this._curMoveRightDoubleAnimation.From = this.screenX;
            this._curMoveRightDoubleAnimation.To = this.screenX + this.screenWidth;
            this._curMoveRightDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._curMoveRightDoubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            this._curMoveRightStoryboard.Children.Add(this._curMoveRightDoubleAnimation);

            this._swapMoveRightDoubleAnimation.From = this.screenX - this.screenWidth;
            this._swapMoveRightDoubleAnimation.To = this.screenX;
            this._swapMoveRightDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._swapMoveRightDoubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            this._swapMoveRightDoubleAnimation.Completed += handleAnimationCompleted;
            this._swapMoveRightStoryboard.Children.Add(this._swapMoveRightDoubleAnimation);

            this._swapMoveLeftDoubleAnimation.From = this.screenX + this.screenWidth;
            this._swapMoveLeftDoubleAnimation.To = this.screenX;
            this._swapMoveLeftDoubleAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(ScreenController.SCREEN_CHANGE_DURATION));
            this._swapMoveLeftDoubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            this._swapMoveLeftDoubleAnimation.Completed += handleAnimationCompleted;
            this._swapMoveLeftStoryboard.Children.Add(this._swapMoveLeftDoubleAnimation);

        }

        /// <summary>
        /// Function called once the swaps have been moved into place.  This cleanup function then makes the swap media the current media.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleAnimationCompleted(object sender, EventArgs e)
        {
            Media tempMedia = this._currentMedia;
            MediaElement tempMediaElement = this._currentMediaElement;
            Canvas tempContainer = this._currentContainer;

            this._currentMediaElement = this._swapMediaElement;
            this._currentMedia = this._swapMedia;
            this._currentContainer = this._swapContainer;

            this._swapMedia = Media.NULL_MEDIA;
            this._swapMediaElement = tempMediaElement;
            this._swapContainer = tempContainer;
        }
        private void initializeMediaElements()
        {
            this._currentContainer = this._window.MediaContainer1;
            this._currentContainer.Height = this.screenHeight;
            this._currentContainer.Width = this.screenWidth;
            this._currentContainer.Visibility = Visibility.Visible;

            this._currentMediaElement = this._window.mediaElement1;
            this._currentMediaElement.Height = this.screenHeight;
            this._currentMediaElement.Width = this.screenWidth;
            this._currentMediaElement.Visibility = Visibility.Visible;
            this._currentMediaElement.Name = "CURRENT_MEDIA_ELEMENT";
            this._currentMediaElement.MediaEnded += this.handleCurrentMediaEnded;
            this._swapContainer = this._window.MediaContainer2;
            this._swapContainer.Visibility = Visibility.Visible;
            this._swapContainer.Height = this.screenHeight;
            this._swapContainer.Width = this.screenWidth;

            this._swapMediaElement = this._window.mediaElement2;
            this._swapMediaElement.Height = this.screenHeight;
            this._swapMediaElement.Width = this.screenWidth;
            this._swapMediaElement.Visibility = Visibility.Visible;
            this._swapMediaElement.Name = "SWAP_MEDIA_ELEMENT";
        }

        public void handleCurrentMediaEnded(object sender, EventArgs e)
        {
            this._currentMediaElement.Position = TimeSpan.FromSeconds(0);
            this._currentMediaElement.Play();

        }

        public void fadeOut()
        {
            this._fadeInStoryboard.SkipToFill();
            Storyboard.SetTargetName(this._fadeOutDoubleAnimation, this._currentContainer.Name);
            Storyboard.SetTargetProperty(this._fadeOutDoubleAnimation, new PropertyPath(Canvas.OpacityProperty));
            this._fadeOutStoryboard.Begin(this._window, true);
        }

        public void fadeIn()
        {
            this._fadeInStoryboard.SkipToFill();
            Storyboard.SetTargetName(this._fadeInDoubleAnimation, this._currentContainer.Name);
            Storyboard.SetTargetProperty(this._fadeInDoubleAnimation, new PropertyPath(Canvas.OpacityProperty));
            this._fadeInStoryboard.Begin(this._window, true);
        }

        public void turnOn(Media m)
        {
            this.CurrentMedia = m;
            this.play();
            this.fadeIn();
        }

        public void play()
        {
            this.skipAllStoryboardsToFill();
            this._currentMediaElement.Play();
            this._currentMediaElement.Position = TimeSpan.FromSeconds(this._currentMedia.CurrentTime);
        }

        public double pause()
        {
            this._currentMediaElement.Pause();
            return this._currentMediaElement.Position.Seconds;
        }

        public double turnOff()
        {
            double position = this.pause();
            this.fadeOut();
            return position;
        }

        private void skipAllStoryboardsToFill()
        {
            this._curMoveLeftStoryboard.SkipToFill();
            this._curMoveLeftStoryboard.SkipToFill();
            this._swapMoveRightStoryboard.SkipToFill();
            this._swapMoveLeftStoryboard.SkipToFill();
            this._fadeInStoryboard.SkipToFill();
            this._fadeOutStoryboard.SkipToFill();
        }
        public void moveMediaToRight(Media media)
        {
            this.skipAllStoryboardsToFill();

            this.pause();
            Storyboard.SetTargetName(this._curMoveRightDoubleAnimation, this._currentContainer.Name);
            Storyboard.SetTargetProperty(this._curMoveRightDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
            this._curMoveRightStoryboard.Begin(this._window, true);

            this.SwapMedia = media;
            this._swapMediaElement.Play();
            this._swapMediaElement.Position = TimeSpan.FromSeconds(this.SwapMedia.CurrentTime);
            Storyboard.SetTargetName(this._swapMoveRightDoubleAnimation, this._swapContainer.Name);
            Storyboard.SetTargetProperty(this._swapMoveRightDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
            this._swapMoveRightStoryboard.Begin(this._window, true);
        }


        public void moveMediaToLeft(Media media)
        {
            this._fadeInStoryboard.SkipToFill();

            this.pause();
            Storyboard.SetTargetName(this._curMoveLeftDoubleAnimation, this._currentContainer.Name);
            Storyboard.SetTargetProperty(this._curMoveLeftDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
            this._curMoveLeftStoryboard.Begin(this._window, true);

            this.SwapMedia = media;
            this._swapMediaElement.Play();
            this._swapMediaElement.Position = TimeSpan.FromSeconds(this.SwapMedia.CurrentTime);
            Storyboard.SetTargetName(this._swapMoveLeftDoubleAnimation, this._swapContainer.Name);
            Storyboard.SetTargetProperty(this._swapMoveLeftDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
            this._swapMoveLeftStoryboard.Begin(this._window, true);
        }

    }
}
