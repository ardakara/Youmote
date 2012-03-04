using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;

namespace YouMote.States
{
    class TalkOnPhoneState : ScenarioState
    {
        public Boolean isSameState(ScenarioState ss)
        {
            return false;
        }

        public ScenarioState mergeEqualStates(ScenarioState ss)
        {
            return null;
        }

        public ScenarioState finishState(ScenarioState next)
        {
            return null;
        }

        public double getDurationInMinutes()
        {
            return 0.0;
        }

        public double getDurationInSeconds()
        {
            return 0.0;
        }

        public double getDurationInMilliseconds()
        {
            return 0.0;
        }
    }
}
