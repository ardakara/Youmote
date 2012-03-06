using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using YouMote;

namespace YouMote.Television
{
    public class Television
    {
        // Blake's Path For Quick Copying:  "C:\\Users\\Blake\\Documents\\CS247\\P4\\Youmote\\Video\\";
        // Charlton's path for quick copying: "C:\\Users\\Charlton Soesanto\\Desktop\\Youmote\\Video\\"
        // Kenny's path for quick copying: "C:\\Users\\Kenny\\CS247\\Youmote\\Video\\"
        // Arda's path for quick copyinh: "C:\\Users\\ardakara\\Documents\\CS247\\Youmote\\Video\\"
        private static String VIDEO_PATH = "C:\\Users\\Blake\\Documents\\CS247\\P4\\Youmote\\Video\\";
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
        private Media _cachedMedia = Media.NULL_MEDIA;
        private static int CACHE_CHANNEL_ID = -1;
        private static String CACHE_CHANNEL_NAME = "CACHED_CHANNEL";
        private ScreenController _screenController;
        private List<Channel> _channels = new List<Channel>();
        public List<Channel> Channels
        {
            get
            {
                return this._channels;
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
            this.CurrentChannelIndex = CACHE_CHANNEL_ID;
        }

        private Channel generateCachedChannel()
        {
            Channel cachedChannel = new Channel(CACHE_CHANNEL_ID, CACHE_CHANNEL_NAME, this._cachedMedia);
            return cachedChannel;
        }

        /// <summary>
        /// Returns the channel currently being viewed
        /// </summary>
        /// <returns></returns>
        public Channel getCurrentChannel()
        {
            int currentChannelIndex = this.CurrentChannelIndex;
            if (currentChannelIndex >= 0 && currentChannelIndex < this.Channels.Count)
            {
                return this.Channels[currentChannelIndex];
            }
            else
            {
                return this.generateCachedChannel();
            }

        }

        /// <summary>
        /// Imagine the channels are a film strip, with each frame being a channel.  
        /// We are viewing 1 frame at a time and can movie the striip to left or right.
        /// This is how to imagine changing channels
        /// </summary>
        public Boolean moveMediaToRight()
        {

            this.updateChannelListings();
            if (this.CurrentChannelIndex >= 0)
            {

                if (!this._cachedMedia.Equals(Media.NULL_MEDIA) && this.CurrentChannelIndex == 0)
                {
                    // if current channel is 0, and the cache media isnt null
                    // show the cached media
                    this.CurrentChannelIndex = Television.CACHE_CHANNEL_ID;

                    Media nextMedia = this._cachedMedia;
                    this._screenController.moveMediaToRight(nextMedia);
                    return true;
                }
                else if (this._currentChannelIndex - 1 >= 0 && this._currentChannelIndex - 1 < this._channels.Count)
                {
                    // otherwise show the next media if it is still in range
                    this.CurrentChannelIndex = this.CurrentChannelIndex - 1;
                    Channel nextChannel = this._channels[this.CurrentChannelIndex];
                    Media nextMedia = nextChannel.Media;
                    this._screenController.moveMediaToRight(nextMedia);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                // do a little bob animation to show that there are no more channels
                return false;
            }

        }

        public Boolean moveMediaToLeft()
        {
            this.updateChannelListings();
            if (this.CurrentChannelIndex + 1 < this._channels.Count || this._currentChannelIndex == CACHE_CHANNEL_ID)
            {
                if (this.CurrentChannelIndex == CACHE_CHANNEL_ID)
                {
                    // if we are currently in cached channel
                    // save where we are and make next channel be the beginning
                    double position = this._screenController.getCurrentMediaPosition();
                    this._cachedMedia.CurrentTime = position;
                    if (this._channels.Count > 0)
                    {
                        this._currentChannelIndex = 0;
                    }
                    else
                    {
                        // no channels available
                        return false;
                    }
                }
                else
                {
                    this.CurrentChannelIndex++;
                }

                Channel nextChannel = this._channels[this.CurrentChannelIndex];
                Media nextMedia = nextChannel.Media;
                this._screenController.moveMediaToLeft(nextMedia);
                return true;
            }
            else
            {
                // do a little bob animation to show that there are no more channels
                return false;
            }

        }

        /// <summary>
        ///  turns on and plays content
        /// </summary>

        public Boolean turnOn()
        {
            if (!this.IsOn)
            {
                this._isOn = true;
                this.updateChannelListings();
                this.IsPaused = false;
                if (!this._cachedMedia.Equals(Media.NULL_MEDIA))
                {
                    this._currentChannelIndex = CACHE_CHANNEL_ID;
                    this._screenController.turnOn(this._cachedMedia);
                }
                else
                {
                    this._currentChannelIndex = 0;
                    Channel onChannel = this.Channels.First();
                    Media onMedia = onChannel.Media;
                    this._screenController.turnOn(onMedia);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean turnOff()
        {
            if (this.IsOn)
            {
                this.pause();
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
                this._currentChannelIndex = CACHE_CHANNEL_ID;
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
            if (!this.IsPaused)
            {
                this.IsPaused = true;
                double position = this._screenController.pause(pr);
                Channel curChannel = this.getCurrentChannel();
                this._cachedMedia = curChannel.Media;
                this._cachedMedia.CurrentTime = position;
                this._currentChannelIndex = CACHE_CHANNEL_ID;
                // fade off the pause button

                return true;
            }
            else
            {
                return false;
            }
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
            Media m2 = new Media(1, 100, 15, "test", VIDEO_PATH + "batman.avi");
            Media m1 = new Media(1, 100, 0, "test", VIDEO_PATH + "pixar_short.avi");
            Media m3 = new Media(1, 100, 0, "test", VIDEO_PATH + "spiderman.avi");
            Media m4 = new Media(1, 100, 0, "test", VIDEO_PATH + "hobbit.avi");
            Channel c1 = new Channel(0, "channel 1", m1);
            Channel c2 = new Channel(0, "channel 2", m2);
            Channel c3 = new Channel(0, "channel 3", m3);
            Channel c4 = new Channel(0, "channel 4", m4);
            this._channels.Clear();
            this._channels.Add(c1);
            this._channels.Add(c2);
            this._channels.Add(c3);
            this._channels.Add(c4);

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
