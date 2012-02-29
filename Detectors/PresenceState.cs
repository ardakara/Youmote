using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{


    public class PresenceState : ScenarioState
{

        public enum GetsUpAndLeavesPosition { SIT, STAND, ABSENT };

        public static PresenceState ABSENT_STATE = new PresenceState(GetsUpAndLeavesPosition.ABSENT, DateTime.Now, DateTime.Now);
        public static PresenceState SIT_STATE = new PresenceState(GetsUpAndLeavesPosition.SIT, DateTime.Now, DateTime.Now);
        public static PresenceState STAND_STATE = new PresenceState(GetsUpAndLeavesPosition.STAND, DateTime.Now, DateTime.Now);

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

        public PresenceState(GetsUpAndLeavesPosition pos, DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
            this._pos = pos;
        }

        public PresenceState(PresenceState gualState)
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
                return this.Pos == ((PresenceState)ss).Pos;
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

        public ScenarioState mergeEqualStates(ScenarioState next)
        {
            PresenceState gualState = new PresenceState(this);
            // make sure this is same state 
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    PresenceState nextState = (PresenceState)next;

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
            PresenceState gualState = new PresenceState(this);
            // make sure this is same state 
            if (!this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    PresenceState nextState = (PresenceState)next;

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