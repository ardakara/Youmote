﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace YouMote
{
    public class Message
    {

        public double time;
        public double duration;
        public String speaker;
        public String text;
        public String imgFile;
        private Stopwatch sw = new Stopwatch();
        public Message(double time, double duration, String speaker, String text, String imgFile)
        {
            this.time = time;
            this.duration = duration;
            this.speaker = speaker;
            this.text = text;
            this.imgFile = imgFile;
        }
        public void startMessageTimer()
        {
            sw.Start();
        }

        public Boolean isFinished(){
            if (this.sw.Elapsed.TotalSeconds > this.duration)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void stopMessageTimer()
        {
            this.sw.Stop();
        }
    }
}
