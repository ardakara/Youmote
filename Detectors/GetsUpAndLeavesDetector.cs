using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    class GetsUpAndLeavesDetector : ScenarioDetectorIMPL
    {
        private static double STAND_DURATION = 30;
        private ScenarioStateHistory _history;
        private StandingIndicator _standingDetector = new StandingIndicator();
        private SittingIndicator _sittingDetector = new SittingIndicator();
        private AbsentDetector _absentDetector = new AbsentDetector();
        public GetsUpAndLeavesDetector()
        {
            this._history = new ScenarioStateHistory();
        }

        public Boolean isScenarioDetected()
        {
            List<ScenarioState> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = false;

                Console.WriteLine("********* CHECKING RECENT STATES *************");
            for (int i = 0; i < recentStates.Count; i++)
            {
                ScenarioState state = recentStates[i];
                Console.WriteLine(i +":" + state.ToString());
                if (i == 0 && !GetsUpAndLeavesState.ABSENT_STATE.Equals(state))
                {
                    // if most recent state wasnt absent, then scenario not detected
                    isDetected = false;
                    break;
                }else if (i == 1)
                {
                    if (GetsUpAndLeavesState.STAND_STATE.Equals(state))
                    {
                        GetsUpAndLeavesState gualState = (GetsUpAndLeavesState)state;
                        double duration = gualState.getDurationInSeconds();
                        if (duration > GetsUpAndLeavesDetector.STAND_DURATION)
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


                }else if (i == 2)
                {
                    if (GetsUpAndLeavesState.SIT_STATE.Equals(state))
                    {
                        isDetected = true;
                        break;
                    }
                }

            }
            return isDetected;
        }

        public void processSkeleton(SkeletonData skeleton)
        {
            Boolean isAbsent = this._absentDetector.isPositionDetected(skeleton);
            if (isAbsent)
            {
                GetsUpAndLeavesState state = new GetsUpAndLeavesState(GetsUpAndLeavesState.GetsUpAndLeavesPosition.ABSENT, DateTime.Now, DateTime.Now);
                this._history.addState(state);
            }
            else
            {
                Boolean isStanding = this._standingDetector.isPositionDetected(skeleton);
                Boolean isSitting = this._sittingDetector.isPositionDetected(skeleton);
                if (isStanding)
                {
                    GetsUpAndLeavesState state = new GetsUpAndLeavesState(GetsUpAndLeavesState.GetsUpAndLeavesPosition.STAND, DateTime.Now, DateTime.Now);
                    this._history.addState(state);
                }
                else if (isSitting)
                {
                    GetsUpAndLeavesState state = new GetsUpAndLeavesState(GetsUpAndLeavesState.GetsUpAndLeavesPosition.SIT, DateTime.Now, DateTime.Now);
                    this._history.addState(state);
                }

            }

        }

    }
}
