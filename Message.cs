using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    public class Message
    {

            public double time;
            public String speaker;
            public String text;
            public String imgFile;

            public Message(double time, String speaker,String text, String imgFile)
            {
                this.time = time;
                this.speaker = speaker;
                this.text = text;
                this.imgFile = imgFile;
            }
    }
}
