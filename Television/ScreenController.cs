using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Shapes;
using YouMote;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YouMote.Television
{
    /// <summary>
    /// Class that controllers what is happening on the users screen.. Paused video, moving videos etc.
    /// </summary>
    public class ScreenController
    {
        public enum PauseReason { STANDUP, PHONE, LEAVE };

        private static String PAUSE_FILE =   "Images\\icons\\icon-solid-pause.png";
        private static String PLAY_FILE =    "Images\\icons\\icon-solid-play.png";
        private static String STANDUP_FILE = "Images\\icons\\icon-solid-standup.png";
        private static String LEAVE_FILE =   "Images\\icons\\icon-solid-leave.png";
        private static String PHONE_FILE =   "Images\\icons\\icon-solid-phone.png";

        private static double PAUSE_FADE_OUT_DURATION = 3.0;
        private static double PLAY_FADE_IN_DURATION = 1.0;
        private static double OFF_FADE_OUT_DURATION = 5.0;
        private static double FADE_IN_DURATION = 3.0;
        private static double SCREEN_CHANGE_DURATION = 1.0;
        private double screenX = 0;
        //private double screenY = 0;
        private double screenWidth;
        private double screenHeight;
        private Image _centerIcon;
        private Image _cornerIcon;
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
            }
        }

        private MediaElement _currentMediaElement;
        private MediaElement _onPointMediaElement;
        private Canvas _currentContainer;
        private Canvas _onPointContainer;

        public ScreenController(MainWindow window)
        {
            this._window = window;
            this.screenWidth = window.MainCanvas.Width;
            this.screenHeight = window.MainCanvas.Height;
            this.initializeMediaElements();

        }


        private DoubleAnimation generateDoubleAnimation(double from, double to, double duration)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(duration));
            da.FillBehavior = FillBehavior.HoldEnd;
            return da;
        }
        /// <summary>
        /// Function called once the swaps have been moved into place.  This cleanup function then makes the swap media the current media.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void initializeMediaElements()
        {
            this._cornerIcon = this._window.CornerIcon;
            this._cornerIcon.Opacity = 0.0;

            this._centerIcon = this._window.CenterIcon;

            this._currentContainer = this._window.MediaContainer1;
            this._currentContainer.Height = this.screenHeight;
            this._currentContainer.Width = this.screenWidth;
            this._currentContainer.Opacity = 0.0;
            this._currentContainer.Visibility = Visibility.Visible;

            this._currentMediaElement = this._window.mediaElement1;
            this._currentMediaElement.Height = this.screenHeight;
            this._currentMediaElement.Width = this.screenWidth;
            this._currentMediaElement.Visibility = Visibility.Visible;
            this._currentMediaElement.Name = "CURRENT_MEDIA_ELEMENT";
            this._currentMediaElement.MediaEnded += this.handleCurrentMediaEnded;

            this._onPointContainer = this._window.MediaContainer2;
            this._onPointContainer.Visibility = Visibility.Visible;
            this._onPointContainer.Height = this.screenHeight;
            this._onPointContainer.Width = this.screenWidth;
            this._onPointContainer.Opacity = 0.0;

            this._onPointMediaElement = this._window.mediaElement2;
            this._onPointMediaElement.Height = this.screenHeight;
            this._onPointMediaElement.Width = this.screenWidth;
            this._onPointMediaElement.Visibility = Visibility.Visible;
            this._onPointMediaElement.Name = "SWAP_MEDIA_ELEMENT";
            this._onPointMediaElement.MediaEnded += this.handleCurrentMediaEnded;
        }

        public void handleCurrentMediaEnded(object sender, EventArgs e)
        {
            this._currentMediaElement.Position = TimeSpan.FromSeconds(0);
            this._currentMediaElement.Play();

        }

        public void setCurrentMediaVolume(double volume)
        {
            if (volume < 0)
            {
                this._currentMediaElement.Volume = 0;
            }
            else if (volume > 1.0)
            {
                this._currentMediaElement.Volume = 0;
            }
            else
            {
                this._currentMediaElement.Volume = volume;
            }
        }

        public void turnOn(Media m)
        {
            this.CurrentMedia = m;
            this._currentMediaElement.Source = this._currentMedia.FileUri;
            this._currentMediaElement.Play();
            this._currentMediaElement.Position = TimeSpan.FromSeconds(this._currentMedia.CurrentTime);
            double startOpacity = this._currentContainer.Opacity;
            this._currentContainer.BeginAnimation(Canvas.OpacityProperty, this.generateDoubleAnimation(startOpacity, 1, ScreenController.FADE_IN_DURATION));

        }

        public void play()
        {
            this._currentMediaElement.Play();
            this._currentMediaElement.Position = TimeSpan.FromSeconds(this._currentMedia.CurrentTime);
            this._centerIcon.Source = this.generateImage(ScreenController.PLAY_FILE);
            double startOpacity = this._currentContainer.Opacity;
            this._currentContainer.BeginAnimation(Canvas.OpacityProperty, this.generateDoubleAnimation(startOpacity, 1, ScreenController.PLAY_FADE_IN_DURATION));
            this._centerIcon.BeginAnimation(Canvas.OpacityProperty, this.generateDoubleAnimation(1.0, 0, ScreenController.PLAY_FADE_IN_DURATION));
            this._cornerIcon.Opacity = 0.0;
        }

        private BitmapImage generateImage(String relativeFilename)
        {
            BitmapImage im = new BitmapImage();
            im.BeginInit();

            String currentPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            im.UriSource = new Uri(currentPath + "\\" + relativeFilename);

            im.EndInit();
            return im;
        }

        public double pause(PauseReason pr)
        {
            this._currentMediaElement.Pause();
            double currentPosition = this._currentMediaElement.Position.Seconds;
            this._currentMedia.CurrentTime = currentPosition;
            this._centerIcon.Source = this.generateImage(ScreenController.PAUSE_FILE);
            this._centerIcon.Opacity = 1.0;
            double startOpacity = this._currentContainer.Opacity;

            this._currentContainer.BeginAnimation(Canvas.OpacityProperty, this.generateDoubleAnimation(startOpacity, 0.3, ScreenController.PAUSE_FADE_OUT_DURATION));
            this._centerIcon.BeginAnimation(Canvas.OpacityProperty, this.generateDoubleAnimation(1, 0, ScreenController.PAUSE_FADE_OUT_DURATION));
            this._cornerIcon.Opacity = 1.0;
            this._cornerIcon.Source = this.generateImage(this.getIconPathForPauseReason(pr));


            return currentPosition;
        }

        public double getCurrentMediaPosition()
        {
            double currentPosition = this._currentMediaElement.Position.Seconds;
            return currentPosition;
        }

        private String getIconPathForPauseReason(PauseReason pr)
        {
            if (pr.Equals(PauseReason.STANDUP))
            {
                return ScreenController.STANDUP_FILE;
            }
            else if (pr.Equals(PauseReason.PHONE))
            {
                return ScreenController.PHONE_FILE;
            }
            else if (pr.Equals(PauseReason.LEAVE))
            {
                return ScreenController.LEAVE_FILE;
            }
            else
            {
                return ScreenController.PAUSE_FILE;
            }
        }

        public double turnOff()
        {
            double position = this.pause(PauseReason.LEAVE);
            double startOpacity = this._currentContainer.Opacity;
            this._currentContainer.BeginAnimation(Canvas.OpacityProperty, this.generateDoubleAnimation(startOpacity, 0, ScreenController.OFF_FADE_OUT_DURATION));
            return position;
        }

        public void moveMediaToRight(Media media)
        {
            this.moveMedia(media, false);
        }


        public void moveMediaToLeft(Media media)
        {
            this.moveMedia(media, true);
        }

        private void moveMedia(Media media, Boolean isLeft)
        {

            if (this._cornerIcon.Opacity > 0)
            {
                this._cornerIcon.Opacity = 0.0;
                this._centerIcon.Source = this.generateImage(ScreenController.PLAY_FILE);
                this._centerIcon.BeginAnimation(Canvas.OpacityProperty, this.generateDoubleAnimation(1.0, 0, ScreenController.PLAY_FADE_IN_DURATION));
            }


            this._currentMediaElement.Pause();
            double currentPosition = this._currentMediaElement.Position.Seconds;
            this._currentMedia.CurrentTime = currentPosition;

            this._onPointMediaElement.Source = media.FileUri;
            this._onPointMediaElement.Volume = this._currentMediaElement.Volume;
            this._onPointMediaElement.Play();
            this._onPointMediaElement.Position = TimeSpan.FromSeconds(media.CurrentTime);

            this._currentContainer.Opacity = 1.0;
            this._onPointContainer.Opacity = 1.0;

            DoubleAnimation curAnimation = this.generateDoubleAnimation(this.screenX, this.screenX + this.screenWidth, ScreenController.SCREEN_CHANGE_DURATION);
            DoubleAnimation onPointAnimation = this.generateDoubleAnimation(this.screenX - this.screenWidth, this.screenX, ScreenController.SCREEN_CHANGE_DURATION);
            if (isLeft)
            {
                curAnimation = this.generateDoubleAnimation(this.screenX, this.screenX - this.screenWidth, ScreenController.SCREEN_CHANGE_DURATION);
                onPointAnimation = this.generateDoubleAnimation(this.screenX + this.screenWidth, this.screenX, ScreenController.SCREEN_CHANGE_DURATION);
            }

            this._currentContainer.BeginAnimation(Canvas.LeftProperty, curAnimation);
            this._onPointContainer.BeginAnimation(Canvas.LeftProperty, onPointAnimation);

            Canvas tempContainer = this._currentContainer;
            MediaElement tempMediaElement = this._currentMediaElement;

            this._currentContainer = this._onPointContainer;
            this._currentMediaElement = this._onPointMediaElement;
            this._currentMedia = media;

            this._onPointContainer = tempContainer;
            this._onPointMediaElement = tempMediaElement;
        }
    }
}
