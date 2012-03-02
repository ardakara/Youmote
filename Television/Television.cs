using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using YouMote;

namespace Youmote.Television
{
    public class Television
    {
        // Blakes Path For Quick Copying:  "C:\\Users\\Blake\\Documents\\CS247\\P4\\Youmote\\Video\\";
        private static String VIDEO_PATH = "C:\\Users\\Blake\\Documents\\CS247\\P4\\Youmote\\Video\\";
        private Boolean _isOn = false;
        public Boolean IsOn
        {
            get
            {
                return this._isOn;
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
        public void moveMediaToRight()
        {

            this.updateChannelListings();
            if (this.CurrentChannelIndex >= 0)
            {
                this.CurrentChannelIndex = this.CurrentChannelIndex - 1;
                if (this.CurrentChannelIndex == CACHE_CHANNEL_ID)
                {
                    Media nextMedia = this._cachedMedia;
                    this._screenController.moveMediaToRight(nextMedia);
                }
                else
                {
                    Channel nextChannel = this._channels[this.CurrentChannelIndex];
                    Media nextMedia = nextChannel.Media;
                    this._screenController.moveMediaToRight(nextMedia);
                }
            }
            else
            {
                // do a little bob animation to show that there are no more channels
            }

        }

        public void moveMediaToLeft()
        {
            this.updateChannelListings();
            if (this.CurrentChannelIndex + 1 < this._channels.Count)
            {
                if (this.CurrentChannelIndex == CACHE_CHANNEL_ID)
                {
                    // currently watching cache.. pause the cache
                    this.pause();
                }
                this.CurrentChannelIndex++;
                Channel nextChannel = this._channels[this.CurrentChannelIndex];
                Media nextMedia = nextChannel.Media;
                this._screenController.moveMediaToLeft(nextMedia);
            }
            else
            {
                // do a little bob animation to show that there are no more channels
            }
        }

        /// <summary>
        ///  turns on and plays content
        /// </summary>

        public void turnOn()
        {
            this._isOn = true;
            if (!this._cachedMedia.Equals(Media.NULL_MEDIA))
            {
                this._screenController.turnOn(this._cachedMedia);
            }
            else
            {
                Channel onChannel = this.Channels.First();
                Media onMedia = onChannel.Media;
                this._screenController.turnOn(onMedia);
            }
        }

        public void turnOff()
        {
            this.pause();
            this._isOn = false;
            this._screenController.turnOff();
        }

        /// <summary>
        ///  pauses the media and media element
        ///  WARNING: PAUSING A CHANNEL INSTANTLY TURNS THAT INTO THE CACHE
        /// </summary>
        public void pause()
        {
            double position = this._screenController.pause();
            Channel curChannel = this.getCurrentChannel();
            this._cachedMedia = curChannel.Media;
            this._cachedMedia.CurrentTime = position;
            this._currentChannelIndex = CACHE_CHANNEL_ID;
        }

        public void play()
        {
            this._screenController.play();
        }


        /// <summary>
        /// Talks to server and updates current channels
        /// </summary>
        private void updateChannelListings()
        {
            // generate fake media
            Media m1 = new Media(1, 100, 0, "test", VIDEO_PATH + "batman.avi");
            Media m2 = new Media(1, 100, 0, "test", VIDEO_PATH + "pixar_short.avi");
            Media m3 = new Media(1, 100, 0, "test", VIDEO_PATH + "spiderman.avi");
            Media m4 = new Media(1, 100, 0, "test", VIDEO_PATH + "hobbit.avi");
            Channel c1 = new Channel(0, "channel 1", m1);
            Channel c2 = new Channel(0, "channel 2", m2);
            this._channels.Clear();
            this._channels.Add(c1);
            this._channels.Add(c2);

        }
        public void fakeTVRun()
        {
            Media m0 = new Media(1, 100, 0, "test", VIDEO_PATH + "pixar_short.avi");
            this._cachedMedia = m0;


            // worker that creates a channel change worker that turns the tv on

            this.simulateFakeAction(this.turnOn, 2);
//            this.simulateFakeAction(this.moveMediaToLeft, 7);
//            this.simulateFakeAction(this.moveMediaToRight, 10);
//            this.simulateFakeAction(this.moveMediaToLeft, 13);
//            this.simulateFakeAction(this.moveMediaToLeft, 15);
//            this.simulateFakeAction(this.moveMediaToRight, 17);
//            this.simulateFakeAction(this.pause, 20);
//            this.simulateFakeAction(this.moveMediaToLeft, 23);
//            this.simulateFakeAction(this.turnOff, 28);
        }

        private delegate void FakeActionDelegate();
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
