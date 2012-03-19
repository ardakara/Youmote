using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Speech;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;
using YouMote.States;

namespace YouMote.Detectors
{
    class MakeoutDetector : ScenarioDetector
    {
        protected ScenarioStateHistory _history;
        protected MakeoutIndicator _makeoutIndicator = new MakeoutIndicator();
        private MainWindow window;
        private HashSet<String> greeting = new HashSet<String>();
        private Boolean hasMadeOut = false;

        public Boolean isScenarioDetected()
        {
            return hasMadeOut;
        }
        public void processTwoSkeletons(Skeleton skel1, Skeleton skel2)
        {
            if (_makeoutIndicator.isMakeoutDetected(skel1, skel2)) {
                hasMadeOut = true;
            } else {
                hasMadeOut = false;
            }
        }
    }
}
