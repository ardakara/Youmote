using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Indicators;
using YouMote.States;

namespace YouMote
{
    public class AmbidextrousStiffSwipeLeftDetector : StiffSwipeLeftDetector
    {
        private Boolean isSwipeLeft(ScenarioStateHistory history)
        {
            List<ScenarioState> recentStates = history.getLastNStates(3);
            if (recentStates.Count == 3)
            {
Console.WriteLine("\t Swipe Left: RS 0: " + recentStates[0].ToString() + ", RS 1: " + recentStates[1].ToString() + ", RS 2: " + recentStates[2].ToString());
            }
            double swipe_duration = 0;
            double finish_duration = 0;
            if (recentStates.Count >= 3)
            {
                if (SwipeState.SWIPE_FINISHED.isSameState(recentStates[0]) && SwipeState.MOVING_LEFT.isSameState(recentStates[1]) && SwipeState.SWIPE_STARTED.isSameState(recentStates[2]))
                {
                    swipe_duration = recentStates[1].getDurationInMilliseconds();
                    finish_duration = recentStates[0].getDurationInMilliseconds();
                    if (swipe_duration < MAX_SWIPE_DURATION)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        
        public override Boolean isScenarioDetected()
        {
            Boolean rh_isSwipeLeft = this.isSwipeLeft(this._rightHandHistory);
            Boolean lh_isSwipeLeft = this.isSwipeLeft(this._leftHandHistory);

            if (rh_isSwipeLeft || lh_isSwipeLeft)
            {
                return true;
            }
            return false;
        }

    }
}
