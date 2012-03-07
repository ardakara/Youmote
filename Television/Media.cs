using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote
{
    public class Media
    {
        public static Media NULL_MEDIA = new Media(-1, -1, -1, "NULL MEDIA", "");
        private int _id;
        public int Id
        {
            get
            {
                return this._id;
            }
        }
        private double _duration;
        public double Duration
        {
            get
            {
                return this._duration;
            }
        }
        private double _currentTime;

        public double CurrentTime
        {
            get
            {
                return this._currentTime;
            }
            set
            {
                this._currentTime = value;
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

        private Uri _fileUri;
        public Uri FileUri
        {
            get
            {
                return this._fileUri;
            }
        }

        private Boolean _isPlaying;
        public Boolean IsPlaying
        {
            get
            {
                return this._isPlaying;
            }
            set
            {
                this._isPlaying = value;
            }

        }

        public Media(int id, double duration, double currentTime, String name, String file)
        {
            this._id = id;
            this._duration = duration;
            this._currentTime = currentTime;
            this._name = name;
            if (!file.Equals(""))
            {
                String currentPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                this._fileUri = new Uri(currentPath + "\\" + file);
            }
        }
    }
}
