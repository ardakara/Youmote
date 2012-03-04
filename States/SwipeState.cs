using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Indicators;

namespace YouMote.States
{


    public class SwipeState : ScenarioState
{

        public enum SwipePosition { SWIPE_STARTED, MOVING_LEFT, MOVING_RIGHT, SWIPE_FINISHED, SWIPE_CANCELLED };

        public static SwipeState SWIPE_STARTED = new SwipeState(SwipePosition.SWIPE_STARTED, DateTime.Now, DateTime.Now);
        public static SwipeState MOVING_LEFT = new SwipeState(SwipePosition.MOVING_LEFT, DateTime.Now, DateTime.Now);
        public static SwipeState MOVING_RIGHT = new SwipeState(SwipePosition.MOVING_RIGHT, DateTime.Now, DateTime.Now);
        public static SwipeState SWIPE_FINISHED = new SwipeState(SwipePosition.SWIPE_FINISHED, DateTime.Now, DateTime.Now);
        public static SwipeState SWIPE_CANCELLED = new SwipeState(SwipePosition.SWIPE_CANCELLED, DateTime.Now, DateTime.Now);

        private DateTime _start;
        private DateTime _end;
        private SwipePosition _pos;

        SwipePosition Pos
        {
            get
            {
                return this._pos;
            }
        }

        public SwipeState(SwipePosition pos, DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
            this._pos = pos;
        }

        public SwipeState(SwipeState goalState)
        {
            this._start = goalState._start;
            this._end = goalState._end;
            this._pos = goalState._pos;
        }


        public Boolean isSameState(ScenarioState ss)
        {
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
        /// returns the duration that this state took place in seconds;
        /// </summary>
        /// <returns></returns>
        public double getDurationInMilliseconds()
        {
            return this._end.Subtract(this._start).TotalMilliseconds;

        }

        public ScenarioState mergeEqualStates(ScenarioState next)
        {
            SwipeState gualState = new SwipeState(this);
            // make sure this is same state 
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    SwipeState nextState = (SwipeState)next;

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

        public override String ToString()
        {
            return this.Pos + "";
        }

        public ScenarioState finishState(ScenarioState next)
        {
            SwipeState gualState = new SwipeState(this);
            // make sure this is same state 
            if (!this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    SwipeState nextState = (SwipeState)next;

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