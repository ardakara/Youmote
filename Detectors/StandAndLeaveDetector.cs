using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.States;

namespace YouMote.Detectors
{
    public class StandAndLeaveDetector : PresenceDetector
    {
        protected static double STAND_DURATION = 30;
        public override Boolean isScenarioDetected()
        {
            List<ScenarioState> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = false;

            for (int i = 0; i < recentStates.Count; i++)
            {

                ScenarioState state = recentStates[i];
                if (i == 0 && !PresenceState.ABSENT_STATE.isSameState(state))

                {
                    // if most recent state wasnt absent, then scenario not detected
                    isDetected = false;
                    break;
                }
                else if (i == 1)
                {
                    if (PresenceState.STAND_STATE.isSameState(state))
                    {
                        PresenceState gualState = (PresenceState)state;
                        double duration = gualState.getDurationInSeconds();
                        if (duration > STAND_DURATION)
                        {
                            isDetected = true;
                            break;
                        }
                        else
                        {
                            // well the person was standing before leaving, make sure they had been sitting before this
                            isDetected = true;
                            continue;
                        }
                    }
                    else
                    {
                        // scenario fail if the person wasn't standing before they left (this would probably be a buggy situations)
                        isDetected = false;
                        break;
                    }


                }
                else if (i == 2)
                {
                    if (PresenceState.SIT_STATE.isSameState(state))
                    {
                        isDetected = true;
                        break;
                    }
                }

            }
            return isDetected;
        }

    }
}
