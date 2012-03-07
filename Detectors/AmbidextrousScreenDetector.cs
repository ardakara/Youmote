using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Indicators;
using YouMote.States;

namespace YouMote
{
    public class AmbidextrousScreenDetector : PullDownScreenDetector
    {
        protected static double MAX_PULL_DURATION = 3000;
        protected static double MAX_ARM_DOWN_DURATION = 60;

        private Boolean isPullDownDetected(ScenarioStateHistory history)
        {
            List<ScenarioState> recentStates = history.getLastNStates(3);
            if (recentStates.Count == 3)
            {
                //Console.WriteLine("\t RS 0: " + recentStates[0].ToString() + ", RS 1: " + recentStates[1].ToString() + ", RS 2: " + recentStates[2].ToString());
            }
            double pull_duration = 0;
            double arm_down_duration = 0;
            if (pull_duration < MAX_PULL_DURATION && recentStates.Count >= 3)
            {
                if (ScreenState.ARM_STRAIGHTUP.isSameState(recentStates[2]) && ScreenState.ARM_MOVINGDOWN.isSameState(recentStates[1]) && ScreenState.ARM_DOWN.isSameState(recentStates[0]))
                {
                    pull_duration = recentStates[1].getDurationInMilliseconds();
                    arm_down_duration = recentStates[0].getDurationInMilliseconds();
                    if ( (pull_duration < MAX_PULL_DURATION) && (arm_down_duration < MAX_ARM_DOWN_DURATION) )
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
            Boolean isPulledDown = this.isPullDownDetected(this._rightHandHistory);
            if (isPulledDown)
            {
                return true;
            }
            return false;
        }

    }
}
