using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SkeletalTracking.Television
{
    /// <summary>
    /// Class that controllers what is happening on the users screen.. Paused video, moving videos etc.
    /// </summary>
    public class ScreenController
    {
        private static int SCREEN_WIDTH = 100;
        private static int SCREEN_HEIGHT = 100;
        private MainWindow _window;
        private Media _currentMedia = Media.NULL_MEDIA;
        private Media _swapMedia = Media.NULL_MEDIA;
        private MediaElement _currentMediaElement = new MediaElement();
        private MediaElement _swapMediaElement = new MediaElement();

        public ScreenController(MainWindow window)
        {
            this._window = window;

            this._window.MainCanvas.Children.Add(this._currentMediaElement);
            this._window.MainCanvas.Children.Add(this._swapMediaElement);
        }

        private void initializeMediaElements()
        {
            this._currentMediaElement.Height= SCREEN_HEIGHT;
            this._currentMediaElement.Width = SCREEN_WIDTH;
            this._currentMediaElement.Opacity = 0;
            this._swapMediaElement.Height= SCREEN_HEIGHT;
            this._swapMediaElement.Width = SCREEN_WIDTH;
            this._swapMediaElement.Opacity = 0;

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
        }

        public void turnOff()
        {
        }

        public void swapInNewMediaFromLeftToRight(Media media)
        {
        }


        public void swapInNewMediaFromRightToLeft(Media media)
        {
        }

    }
}
