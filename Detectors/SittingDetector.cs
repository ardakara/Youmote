using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking.Detectors
{
    public class SittingDetector : PresenceDetector
    {
        private static double MIN_SIT_TIME_IN_SECONDS = 0.5;

        public override Boolean isScenarioDetected()
        {
            ScenarioStateIMPL lastState = this._history.Peek();
            if (PresenceState.SIT_STATE.isSameState(lastState))
            {
                PresenceState pState = (PresenceState)lastState;
                double duration = pState.getDurationInSeconds();
                if (duration > SittingDetector.MIN_SIT_TIME_IN_SECONDS)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
