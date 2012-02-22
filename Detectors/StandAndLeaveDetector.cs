﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking.Detectors
{
    class StandAndLeaveDetector : PresenceDetector
    {
        protected static double STAND_DURATION = 30;
        public Boolean isScenarioDetected()
        {
            List<ScenarioStateIMPL> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = false;

            Console.WriteLine("********* CHECKING RECENT STATES *************");
            for (int i = 0; i < recentStates.Count; i++)
            {
                ScenarioStateIMPL state = recentStates[i];
                Console.WriteLine(i + ":" + state.ToString());
                if (i == 0 && !PresenceState.ABSENT_STATE.Equals(state))
                {
                    // if most recent state wasnt absent, then scenario not detected
                    isDetected = false;
                    break;
                }
                else if (i == 1)
                {
                    if (PresenceState.STAND_STATE.Equals(state))
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
                    if (PresenceState.SIT_STATE.Equals(state))
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