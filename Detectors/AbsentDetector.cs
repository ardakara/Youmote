using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking.Detectors
{
    public class AbsentDetector : PresenceDetector
    {
        private static double MIN_ABSENT_TIME_IN_SECONDS = 0.5;

        public override Boolean isScenarioDetected()
        {
            ScenarioStateIMPL lastState = this._history.Peek();
            if (PresenceState.ABSENT_STATE.isSameState(lastState))
            {
                PresenceState pState = (PresenceState)lastState;
                double duration = pState.getDurationInSeconds();
                if (duration > AbsentDetector.MIN_ABSENT_TIME_IN_SECONDS)
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
