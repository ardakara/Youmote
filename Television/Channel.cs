using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    public class Channel
    {
        public static Channel NULL_CHANNEL = new Channel(-1, "Null Channel", null);
        private int _id;
        public int Id
        {
            get
            {
                return this._id;
            }
        }
        private String _name;
        public String Name
        {
            get
            {
                return this._name;
            }
        }

        private Media _media;
        public Media Media
        {
            get
            {
                return this._media;
            }
        }

        public Channel(int id, String name, Media media)
        {
            this._id = id;
            this._name = name;
            this._media = media;
        }
    }
}
