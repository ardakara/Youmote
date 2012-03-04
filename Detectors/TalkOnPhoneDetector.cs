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
    class TalkOnPhoneDetector : ScenarioDetector
    {
        private static double MIN_HAND_ON_FACE_TIME_IN_SECONDS = 0.5;

        public Boolean isScenarioDetected()
        {
            return false;

        }
        public void processSkeleton(Skeleton skeleton)
        {
            //check if hand is on face
        }
    }
}
