using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote.States
{


    public class ResumeState : ScenarioState
{

        public enum ResumePosition { HAND_BACK, HAND_FRONT, HAND_AWAY};

        public static ResumeState HAND_BACK = new ResumeState(ResumePosition.HAND_BACK, DateTime.Now, DateTime.Now);
        public static ResumeState HAND_FRONT = new ResumeState(ResumePosition.HAND_FRONT, DateTime.Now, DateTime.Now);
        public static ResumeState HAND_AWAY = new ResumeState(ResumePosition.HAND_AWAY, DateTime.Now, DateTime.Now);

        private DateTime _start;
        private DateTime _end;
        private ResumePosition _pos;

        ResumePosition Pos
        {
            get
            {
                return this._pos;
            }
        }

        public ResumeState(ResumePosition pos, DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
            this._pos = pos;
        }

        public ResumeState(ResumeState goalState)
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
                return this.Pos == ((ResumeState)ss).Pos;
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
            ResumeState gualState = new ResumeState(this);
            // make sure this is same state 
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    ResumeState nextState = (ResumeState)next;

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
            ResumeState gualState = new ResumeState(this);
            // make sure this is same state 
            if (!this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    ResumeState nextState = (ResumeState)next;

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