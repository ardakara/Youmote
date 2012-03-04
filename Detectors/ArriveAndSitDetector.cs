using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.States;

namespace YouMote.Detectors
{
    class ArriveAndSitDetector : PresenceDetector
    {


        public override Boolean isScenarioDetected()
        {
            List<ScenarioState> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = false;

            for (int i = 0; i < recentStates.Count; i++)
            {
                ScenarioState state = recentStates[i];
            }
            return isDetected;
        }

    }
}
