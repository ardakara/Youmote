using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote.Detectors
{
    public class StandingDetector : PresenceDetector
    {
        private static double MIN_STAND_TIME_IN_SECONDS = 0.5;

        public override Boolean isScenarioDetected()
        {
            ScenarioState lastState = this._history.Peek();
            if (PresenceState.STAND_STATE.isSameState(lastState))
            {
                PresenceState pState = (PresenceState)lastState;
                double duration = pState.getDurationInSeconds();
                if (duration > StandingDetector.MIN_STAND_TIME_IN_SECONDS)
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
