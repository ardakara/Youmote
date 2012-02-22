﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking.Detectors
{
    class ArriveAndSitDetector : PresenceDetector
    {


        public Boolean isScenarioDetected()
        {
            List<ScenarioStateIMPL> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = false;

            Console.WriteLine("********* CHECKING RECENT STATES *************");
            for (int i = 0; i < recentStates.Count; i++)
            {
                ScenarioStateIMPL state = recentStates[i];
                Console.WriteLine(i + ":" + state.ToString());
            }
            return isDetected;
        }

    }
}