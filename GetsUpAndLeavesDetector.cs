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
            return true;
        }

        void processSkeleton(SkeletonData skeleton)
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
                    GetsUpAndLeavesState state = new GetsUpAndLeavesState(GetsUpAndLeavesState.GetsUpAndLeavesPosition.ABSENT, DateTime.Now, DateTime.Now);
                    this._history.addState(state);
                }
                else if (isSitting)
                {
                    GetsUpAndLeavesState state = new GetsUpAndLeavesState(GetsUpAndLeavesState.GetsUpAndLeavesPosition.ABSENT, DateTime.Now, DateTime.Now);
                    this._history.addState(state);
                }

            }

        }



        public class GetsUpAndLeavesState : ScenarioState
        {
            public enum GetsUpAndLeavesPosition { NONE, SIT, STAND, ABSENT };
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


            Boolean isSameState(ScenarioState ss)
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

            ScenarioState mergeEqualStates(ScenarioState ss)
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

            ScenarioState finishState(ScenarioState next)
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
