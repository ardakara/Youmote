using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    class GetsUpAndLeavesDetector : ScenarioDetector
    {
        private static double STAND_DURATION = 30;
        private ScenarioStateHistory _history;
        private StandingDetector _standingDetector = new StandingDetector();
        private SittingDetector _sittingDetector = new SittingDetector();
        private AbsentDetector _absentDetector = new AbsentDetector();
        public GetsUpAndLeavesDetector()
        {
            this._history = new ScenarioStateHistory();
        }

        public Boolean isScenarioDetected()
        {
            List<ScenarioState> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = false;
            for (int i = 0; i < recentStates.Count; i++)
            {
                ScenarioState state = recentStates[i];
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



        public class GetsUpAndLeavesState : ScenarioState
        {

            public enum GetsUpAndLeavesPosition { SIT, STAND, ABSENT };

            public static GetsUpAndLeavesState ABSENT_STATE = new GetsUpAndLeavesState(GetsUpAndLeavesPosition.ABSENT, DateTime.Now, DateTime.Now);
            public static GetsUpAndLeavesState SIT_STATE = new GetsUpAndLeavesState(GetsUpAndLeavesPosition.SIT, DateTime.Now, DateTime.Now);
            public static GetsUpAndLeavesState STAND_STATE = new GetsUpAndLeavesState(GetsUpAndLeavesPosition.STAND, DateTime.Now, DateTime.Now);

            private DateTime _start;
            private DateTime _end;
            private GetsUpAndLeavesPosition _pos;

            GetsUpAndLeavesPosition Pos
            {
                get
                {
                    return this._pos;
                }
            }

            public GetsUpAndLeavesState(GetsUpAndLeavesPosition pos, DateTime start, DateTime end)
            {
                this._start = start;
                this._end = end;
                this._pos = pos;
            }

            public GetsUpAndLeavesState(GetsUpAndLeavesState gualState)
            {
                this._start = gualState._start;
                this._end = gualState._end;
                this._pos = gualState._pos;
            }


            public Boolean isSameState(ScenarioState ss)
            {
                if (!ss.GetType().Equals(this.GetType()))
                {
                    return false;
                }
                else
                {
                    return this.Pos == ((GetsUpAndLeavesState)ss).Pos;
                }
            }
            /// <summary>
            /// returns the duration that this state took place in minutes;
            /// </summary>
            /// <returns></returns>
            public double getDurationInMinutes()
            {
                return this._end.Subtract(this._start).TotalMinutes;

            }

            /// <summary>
            /// returns the duration that this state took place in seconds;
            /// </summary>
            /// <returns></returns>
            public double getDurationInSeconds()
            {
                return this._end.Subtract(this._start).TotalSeconds;

            }


            public ScenarioState mergeEqualStates(ScenarioState ss)
            {
                GetsUpAndLeavesState gualState = new GetsUpAndLeavesState(this);
                // make sure this is same state 
                if (this.isSameState(ss))
                {
                    if (ss.GetType().Equals(this.GetType()))
                    {
                        GetsUpAndLeavesState nextState = new GetsUpAndLeavesState(this);

                        // make gualState's start be the earlier of the two
                        if (nextState._start.CompareTo(gualState._start) < 0)
                        {
                            gualState._start = nextState._start;
                        }

                        // make gualState's start be the later of the two
                        if (nextState._end.CompareTo(gualState._end) > 0)
                        {
                            gualState._end = nextState._end;
                        }

                    }
                }
                return gualState;
            }

            public ScenarioState finishState(ScenarioState next)
            {
                GetsUpAndLeavesState gualState = new GetsUpAndLeavesState(this);
                // make sure this is same state 
                if (!this.isSameState(next))
                {
                    if (next.GetType().Equals(this.GetType()))
                    {
                        GetsUpAndLeavesState nextState = new GetsUpAndLeavesState(this);

                        // make gualState's start be the earlier of the two
                        if (nextState._start.CompareTo(gualState._start) < 0)
                        {
                            gualState._start = nextState._start;
                        }
                        // make gualState's start be the later of the two
                        if (nextState._end.CompareTo(gualState._end) > 0)
                        {
                            gualState._end = nextState._end;
                        }
                    }
                }
                return gualState;
            }
        }
    }
}
