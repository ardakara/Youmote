using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using YouMote;
using System.Diagnostics;

namespace YouMote.Television
{
    public class Television
    {
        private double _volume = 1.0;
        public double Volume
        {
            get
            {
                return this._volume;
            }

            set
            {
                this._volume = value;
                this._screenController.setCurrentMediaVolume(value);
            }
        }
        private Boolean _isOn = false;

        public Boolean IsOn
        {
            get
            {
                return this._isOn;
            }
        }
        public Boolean _isPaused = true;
        public Boolean IsPaused
        {
            get
            {
                return this._isPaused;
            }
            set
            {
                this._isPaused = value;
            }
        }

        private Stopwatch _stopWatch = new Stopwatch();
        private Media _cachedMedia = Media.NULL_MEDIA;
        private int _cachedMediaIndex = -1;
        private ScreenController _screenController;
        private List<Channel> _channels = new List<Channel>();
        public List<Channel> Channels
        {
            get
            {
                return this._channels;
            }

        }
        private int _lastChannelIndex;
        public int LastChannelIndex
        {
            get
            {
                return this._lastChannelIndex;
            }
            set
            {
                this._lastChannelIndex = value;
            }
        }
        private int _currentChannelIndex;
        public int CurrentChannelIndex
        {
            get
            {
                return this._currentChannelIndex;
            }
            set
            {
                this._currentChannelIndex = value;
            }
        }

        public Television(MainWindow window)
        {
            this._screenController = new ScreenController(window);
            this.CurrentChannelIndex = 0;
        }


        /// <summary>
        /// Returns the channel currently being viewed
        /// </summary>
        /// <returns></returns>
        public Channel getCurrentChannel()
        {
            int currentChannelIndex = this.CurrentChannelIndex;
            return this.Channels[currentChannelIndex];

        }

        /// <summary>
        /// Imagine the channels are a film strip, with each frame being a channel.  
        /// We are viewing 1 frame at a time and can movie the striip to left or right.
        /// This is how to imagine changing channels
        /// </summary>
        public Boolean moveMediaToRight()
        {
            this.updateChannelListings();
            this.CurrentChannelIndex = (this.CurrentChannelIndex - 1) % this.Channels.Count;
            Channel nextChannel = this._channels[this.CurrentChannelIndex];
            Media nextMedia = nextChannel.Media;
            this._screenController.moveMediaToRight(nextMedia);
            return true;
        }

        public Boolean moveMediaToLeft()
        {
            this.updateChannelListings();
            this.CurrentChannelIndex = (this.CurrentChannelIndex + 1) % this.Channels.Count;
            Channel nextChannel = this._channels[this.CurrentChannelIndex];
            Media nextMedia = nextChannel.Media;
            this._screenController.moveMediaToLeft(nextMedia);
            return true;

        }

        /// <summary>
        ///  turns on and plays content
        /// </summary>

        public Boolean turnOn()
        {
            if (!this.IsOn)
            {
                this._stopWatch.Start();
                this._isOn = true;
                this.updateChannelListings();
                this.IsPaused = false;
                Channel nextChannel = this._channels[this.CurrentChannelIndex];
                Media nextMedia = nextChannel.Media;
                this._screenController.turnOn(nextMedia);
                return true;
            }
            else
            {
                return false;
            }
        }

        private double computeMediaTime(Media m)
        {
            double elapsed = this._stopWatch.Elapsed.Seconds;
            double mediaDuration = m.Duration;
            double time = elapsed % mediaDuration;
            return time;
        }

        public Boolean turnOff()
        {
            if (this.IsOn)
            {
                this._stopWatch.Stop();
                ScreenController.PauseReason reason = ScreenController.PauseReason.STANDUP;
                this.pause(reason);
                this._isOn = false;
                this.IsPaused = true;
                this._screenController.turnOff();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  pauses the media and media element
        ///  WARNING: PAUSING A CHANNEL INSTANTLY TURNS THAT INTO THE CACHE
        /// </summary>
        public Boolean pause(ScreenController.PauseReason pr)
        {
            if (!this.IsPaused)
            {
                this.IsPaused = true;
                double position = this._screenController.pause(pr);
                Channel curChannel = this.getCurrentChannel();
                this._cachedMedia = curChannel.Media;
                this._cachedMedia.CurrentTime = position;
                this._cachedMediaIndex = this._currentChannelIndex;
                // fade off the pause button
                return true;
            }
            else
            {
                return false;
            }
        }


        public Boolean pause()
        {
            ScreenController.PauseReason pr = ScreenController.PauseReason.LEAVE;
            return this.pause(pr);
        }

        /// <summary>
        /// plays the current media.  Returns true if media went from stop to play.  False otherwise
        /// </summary>
        /// <returns></returns>

        public Boolean play()
        {
            if (this.IsPaused)
            {
                this.IsPaused = false;
                this._screenController.play();
                return true;
            }
            else
            {
                return false;
            }
        }


        public Boolean resume()
        {
            Boolean isPlay = this.play();
            if (isPlay)
            {
                // fade in the resume icon
            }
            return isPlay;
        }

        /// <summary>
        /// Talks to server and updates current channels
        /// </summary>
        private void updateChannelListings()
        {


            // generate fake media
            Media m1 = new Media(1, 4 * 60 + 40, 0, "test", "Video\\pixar_short.avi");
            Media m2 = new Media(1, 2 * 60 + 9, 15, "test", "Video\\batman.avi");
            Media m3 = new Media(1, 2 * 60 + 32, 0, "test", "Video\\spiderman.avi");
            Media m4 = new Media(1, 2 * 60 * 30, 0, "test", "Video\\hobbit.avi");
            Channel c1 = new Channel(0, "channel 1", m1);
            Channel c2 = new Channel(0, "channel 2", m2);
            Channel c3 = new Channel(0, "channel 3", m3);
            Channel c4 = new Channel(0, "channel 4", m4);
            this._channels.Clear();
            this._channels.Add(c1);
            this._channels.Add(c2);
            this._channels.Add(c3);
            this._channels.Add(c4);
            for (int i = 0; i < this._channels.Count; i++)
            {
                Channel c = this._channels[i];
                if (i == this._cachedMediaIndex)
                {
                    c.Media.CurrentTime = this._cachedMedia.CurrentTime;
                }
                else
                {
                    c.Media.CurrentTime = this.computeMediaTime(c.Media);
                }
            }
        }

        public void fakeTVRun()
        {
            // worker that creates a channel change worker that turns the tv on

            this.simulateFakeAction(this.turnOn, 1); // batman
            this.simulateFakeAction(this.moveMediaToLeft, 5); // to pixar
            this.simulateFakeAction(this.moveMediaToLeft, 10); // to pixar
            this.simulateFakeAction(this.moveMediaToLeft, 15); // to pixar
            this.simulateFakeAction(this.moveMediaToRight, 20); // to pixar
            this.simulateFakeAction(this.pause, 25); // batman
            this.simulateFakeAction(this.play, 30); // to pixar 
            this.simulateFakeAction(this.play, 35); // to pixar 
            this.simulateFakeAction(this.pause, 37); // batman
            this.simulateFakeAction(this.moveMediaToRight, 40); // to pixar
            this.simulateFakeAction(this.moveMediaToLeft, 45); // to pixar
            /*            this.simulateFakeAction(this.play, 11); // to pixar 
                        this.simulateFakeAction(this.moveMediaToLeft, 15); // to pixar
                        this.simulateFakeAction(this.moveMediaToRight, 25); // top batman
                        this.simulateFakeAction(this.moveMediaToLeft, 35); // to pixar
                        this.simulateFakeAction(this.moveMediaToLeft, 45); // to vat?
                        this.simulateFakeAction(this.pause, 60);
                        this.simulateFakeAction(this.turnOff, 65);
             */
        }

        private delegate Boolean FakeActionDelegate();
        private void simulateFakeAction(FakeActionDelegate fakeAction, double time)
        {
            BackgroundWorker workerInvoker = new BackgroundWorker();
            workerInvoker.DoWork += delegate
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(time));
                    };

            workerInvoker.RunWorkerCompleted += delegate
            {
                fakeAction();
            };
            workerInvoker.RunWorkerAsync();
        }
    }
}
