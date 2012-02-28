using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking.Detectors
{
    class ArriveAndSitDetector : PresenceDetector
    {


        public override Boolean isScenarioDetected()
        {
            List<ScenarioStateIMPL> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = false;

            for (int i = 0; i < recentStates.Count; i++)
            {
                ScenarioStateIMPL state = recentStates[i];
            }
            return isDetected;
        }

    }
}
