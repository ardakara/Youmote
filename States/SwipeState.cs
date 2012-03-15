using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote.States
{
    class SwipeState : ScenarioState
    {
        public enum SwipePosition { START, END, MOVING, NEUTRAL };
        private DateTime _start;
        private DateTime _end;
        public DateTime Start
        {
            get { return _start; }
        }
        public DateTime End
        {
            get { return _end; }
        }
        private SwipePosition _pos;
        private Point3D _loc;
        public Point3D Loc
        {
            get
            {
                return _loc;
            }
        }
        public SwipePosition Pos
        {
            get
            {
                return this._pos;
            }
        }

        public SwipeState(SwipePosition pos, DateTime start, DateTime end, Point3D loc)
        {
            this._start = start;
            this._end = end;
            this._pos = pos;
            this._loc = loc;
        }


        public SwipeState(SwipeState s)
        {
            this._start = s._start;
            this._end = s._end;
            this._pos = s.Pos;
            this._loc = s._loc;
        }

        public String toString()
        {
            if (this.Pos.Equals(SwipePosition.END))
            {
                return "END";
            }else if (this.Pos.Equals(SwipePosition.MOVING))
            {
                return "MOVING";
            }else if (this.Pos.Equals(SwipePosition.NEUTRAL))
            {
                return "NEUTRAL";
            }else
            {
                return "START";
            }
            
        }
        public Boolean isSameState(ScenarioState ss)
        {
            if (ss == null)
            {
                return false;
            }
            if (!ss.GetType().Equals(this.GetType()))
            {
                return false;
            }
            else
            {
                return this.Pos == ((SwipeState)ss).Pos;
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

        /// <summary>
        /// returns the duration that this state took place in milliseconds;
        /// </summary>
        /// <returns></returns>
        public double getDurationInMilliseconds()
        {
            return this._end.Subtract(this._start).TotalMilliseconds;
        }

        /// <summary>
        /// when swipes get merged, their start and end times get merged
        /// the most recent position becomes the poisiton
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>

        public ScenarioState mergeEqualStates(ScenarioState next)
        {
            SwipeState swipeState = new SwipeState(this);
            // make sure this is same state 
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    SwipeState nextState = (SwipeState)next;

                    // make gualState's start be the earlier of the two
                    if (nextState._start.CompareTo(swipeState._start) < 0)
                    {
                        swipeState._start = nextState._start;

                    }

                    // make gualState's end be the later of the two
                    // and make its loc the one of the later state
                    if (nextState._end.CompareTo(swipeState._end) > 0)
                    {
                        swipeState._end = nextState._end;
                        if (nextState.Pos.Equals(SwipePosition.MOVING))
                        {
                            // only merge state location if merging two moving states
                            swipeState._loc = nextState.Loc;
                        }
                    }
                }
            }

            return swipeState;
        }

        public override String ToString()
        {
            return this.Pos + "";
        }

        public ScenarioState finishState(ScenarioState next)
        {
            SwipeState swipeState = new SwipeState(this);
            // make sure these are the same type
            if (next.GetType().Equals(this.GetType()))
            {
                // make sure this is not state 
                if (!this.isSameState(next))
                {
                    SwipeState nextState = (SwipeState)next;

                    // make gualState's start be the earlier of the two
                    if (nextState._start.CompareTo(swipeState._start) < 0)
                    {
                        swipeState._start = nextState._start;
                    }
                    // make gualState's start be the later of the two
                    if (nextState._end.CompareTo(swipeState._end) > 0)
                    {
                        swipeState._end = nextState._end;
                    }

                    if (this.Pos.Equals(SwipePosition.START) && nextState.Pos.Equals(SwipePosition.MOVING))
                    {
                        // if going from start state to moving state
                        // make the capped start state have the same location as moving state
                        swipeState._loc = nextState._loc;
                    }

                }
            }
            return swipeState;
        }
    }

}
