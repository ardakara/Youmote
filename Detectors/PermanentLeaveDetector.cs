using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.States;

namespace YouMote.Detectors
{
    public class PermanentLeaveDetector : PresenceDetector
    {
        private static double MIN_ABSENT_TIME_IN_MINUTES = 10.0/60;

        public override Boolean isScenarioDetected()
        {
            ScenarioState lastState = this._history.Peek();
            if (PresenceState.ABSENT_STATE.isSameState(lastState))
            {
                PresenceState pState = (PresenceState)lastState;
                double duration = pState.getDurationInMinutes();
                if (duration > PermanentLeaveDetector.MIN_ABSENT_TIME_IN_MINUTES)
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
