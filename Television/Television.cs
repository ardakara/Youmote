using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote.Television
{
    class Television
    {
        private Media _cachedMedia = Media.NULL_MEDIA;

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

        public Television()
        {
            this.CurrentChannelIndex = -1;
        }

        /// <summary>
        /// Returns the channel currently being viewed
        /// </summary>
        /// <returns></returns>
        public Channel getCurrentChannel()
        {
            int currentChannelIndex = this.CurrentChannelIndex;
            if (currentChannelIndex > 0 && currentChannelIndex < this.Channels.Count)
            {
                return this.Channels[currentChannelIndex];

            }
            else
            {
                return Channel.NULL_CHANNEL;
            }

        }

        public void moveToLeftChannel()
        {
        }

        public void moveToRightChannel()
        {
        }

        public void turnOn()
        {
        }

        public void turnOff()
        {
        }

        public void pause()
        {
        }

        public void play()
        {
        }

        public void updateCurrentChannels()
        {
        }

    }
}
