using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote
{
    public class AmbidextrousResumeDetector : ResumeDetector
    {
        protected static double MAX_Resume_DURATION = 1;

        private Boolean isResumeDetected(ScenarioStateHistory history)
        {
            List<ScenarioState> recentStates = history.getLastNStates(3);

            double Resume_duration = 0;
            for (int i = 0; i < recentStates.Count; i++)
            {
                Resume_duration += recentStates[i].getDurationInMilliseconds();
                if (recentStates.Count == 3)
                {
                    //Console.WriteLine("\t RS 0: " + recentStates[0].ToString() + ", RS 1: " + recentStates[1].ToString() + ", RS 2: " + recentStates[2].ToString());
                }
            }
            if (Resume_duration < 3000 && recentStates.Count >= 3)
            {
                if (ResumeState.HAND_FRONT.isSameState(recentStates[0]) && ResumeState.HAND_BACK.isSameState(recentStates[1]) && ResumeState.HAND_FRONT.isSameState(recentStates[2]))
                {
                    return true;
                }
                else if (ResumeState.HAND_BACK.isSameState(recentStates[0]) && ResumeState.HAND_FRONT.isSameState(recentStates[1]) && ResumeState.HAND_BACK.isSameState(recentStates[2]))
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

        public override Boolean isScenarioDetected()
        {
            Boolean rightHandResumed = isResumeDetected(this._rightHandHistory);
            Boolean leftHandResumed = isResumeDetected(this._leftHandHistory);
            return (rightHandResumed || leftHandResumed);
        }

    }
}
