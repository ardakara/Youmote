using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Speech;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace YouMote.Detectors
{
    class SpeechStartOverrideDetector : ScenarioDetector
    {
        MainWindow window;
        private SpeechRecognizer speechRecognizer;


        public SpeechStartOverrideDetector(MainWindow win)
        {
            window = win;
            this.speechRecognizer = win.speechRecognizer;
        }

        public Boolean isScenarioDetected()
        {
            String wordsSaid = speechRecognizer.Word;
            if (wordsSaid == null) return false;
            if (wordsSaid.Equals("TV start"))
            {
                return true;
            }
            return false;

        }

        public void processSkeleton(Skeleton skeleton)
        {

        }



    }
}
