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
using YouMote.Detectors;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YouMote.Television
{
    /// <summary>
    /// Class that controllers what is happening on the users screen.. Paused video, moving videos etc.
    /// </summary>
    public class ScreenController
    {
        public enum PauseReason { STANDUP, PHONE, LEAVE, SPEECH };

        private static String PAUSE_FILE =   "Images\\icons\\icon-solid-pause.png";
        private static String PLAY_FILE =    "Images\\icons\\icon-solid-play.png";
        private static String STANDUP_FILE = "Images\\icons\\icon-solid-standup.png";
        private static String LEAVE_FILE =   "Images\\icons\\icon-solid-leave.png";
        private static String PHONE_FILE =   "Images\\icons\\icon-solid-phone.png";
        private static String SPEECH_FILE =   "Images\\icons\\icon-solid-speech.png";
        private static String OFF_FILE =    "Images\\icons\\icon-solid-off.png";

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
        private Image _swipeIcon;
        private ProgressBar _volumeBar;
        private MainWindow _window;
        private Media _currentMedia = Media.NULL_MEDIA;

        private double swipableWidth;
        private SwipeDirection lastSwipeDirection;
        public bool isInSwipe;

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
            this.swipableWidth = this.screenWidth - this._window.SwipeIcon.Width;
            this.lastSwipeDirection = SwipeDirection.CENTER;
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

            this._centerIcon = this._window.CenterIcon;
            Canvas.SetLeft(this._centerIcon, (this.screenWidth - this._centerIcon.Width) / 2.0);
            Canvas.SetTop(this._centerIcon, (this.screenHeight - this._centerIcon.Height) / 2.0);

            this._swipeIcon = this._window.SwipeIcon;

            this._volumeBar = this._window.VolumeBar;
            this._volumeBar.Height = this.screenHeight - 400;

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
            else if (pr.Equals(PauseReason.SPEECH))
            {
                return ScreenController.SPEECH_FILE;
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
            this._cornerIcon.Opacity = 1.0;
            return position;
        }

        // between 0 - 100
        public void setVolumeBar(int value)
        {
            this._volumeBar.Visibility = Visibility.Visible;
            this._volumeBar.Value = value;
            // TODO: have a timer, that resets to a certain amount every time volume is updated, when it runs out hide it again
            // TODO: set volume of media
        }

        public void startSwipe(SwipeDirection direction)
        {
            this.isInSwipe = true;
            this._swipeIcon.Visibility = Visibility.Visible;
            this.lastSwipeDirection = SwipeDirection.CENTER;
            if (direction == SwipeDirection.LEFT)
            {
                Canvas.SetLeft(this._swipeIcon, this.screenWidth - this._swipeIcon.Width);  
            }
            else if (direction == SwipeDirection.RIGHT) {
                Canvas.SetLeft(this._swipeIcon, 0);
            }
            Canvas.SetTop(this._swipeIcon, (this.screenHeight - this._swipeIcon.Height) / 2.0);
        }

        public void updateSwipe(double swipeProgress, SwipeDirection direction)
        {
            double swipeOffset = this.swipableWidth * swipeProgress;
            double newLeft = 0;
            if (direction == SwipeDirection.LEFT)
            {
                newLeft = this.screenWidth - this._swipeIcon.Width;
                newLeft -= swipeOffset;
                if (this.lastSwipeDirection != direction)
                {
                    this.lastSwipeDirection = direction;
                    this._swipeIcon.Source = new BitmapImage(
                        new Uri("../../Images/arrow-left.png", UriKind.Relative)
                    );
                }
            }
            else if (direction == SwipeDirection.RIGHT)
            {
                newLeft = 0;
                newLeft += swipeOffset;
                if (this.lastSwipeDirection != direction)
                {
                    this.lastSwipeDirection = direction;
                    this._swipeIcon.Source = new BitmapImage(
                        new Uri("../../Images/arrow-left.png", UriKind.Relative)
                    );
                }
            }

            Canvas.SetLeft(this._window.SwipeIcon, newLeft);

            if (swipeProgress >= 1.00)
            {
                this._swipeIcon.Visibility = Visibility.Hidden;
                this.isInSwipe = false;
                // TODO: call swipe here
            }
        }

        public void abortSwipe()
        {
            this.isInSwipe = false;
            this._swipeIcon.Visibility = Visibility.Hidden;
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
            this._currentMediaElement.Pause(); 
            this._currentMediaElement.Source = media.FileUri;
            this._currentMediaElement.Volume = this._currentMediaElement.Volume;
            this._currentMediaElement.Play();
            this._currentMediaElement.Position = TimeSpan.FromSeconds(media.CurrentTime);


            /*
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
            this._onPointMediaElement = tempMediaElement;*/
        }
    }
}
