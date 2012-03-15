using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Indicators;

namespace YouMote.States
{


    public class ScreenState : ScenarioState
{

        public enum ScreenPosition { ARM_STRAIGHTUP, ARM_MOVINGDOWN, ARM_DOWN, ARM_ELSEWHERE};

        public static ScreenState ARM_STRAIGHTUP = new ScreenState(ScreenPosition.ARM_STRAIGHTUP, DateTime.Now, DateTime.Now);
        public static ScreenState ARM_MOVINGDOWN = new ScreenState(ScreenPosition.ARM_MOVINGDOWN, DateTime.Now, DateTime.Now);
        public static ScreenState ARM_DOWN = new ScreenState(ScreenPosition.ARM_DOWN, DateTime.Now, DateTime.Now);
        public static ScreenState ARM_ELSEWHERE = new ScreenState(ScreenPosition.ARM_ELSEWHERE, DateTime.Now, DateTime.Now);

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
        private ScreenPosition _pos;

        ScreenPosition Pos
        {
            get
            {
                return this._pos;
            }
        }

        public ScreenState(ScreenPosition pos, DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
            this._pos = pos;
        }

        public ScreenState(ScreenState goalState)
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
                return this.Pos == ((ScreenState)ss).Pos;
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
            ScreenState gualState = new ScreenState(this);
            // make sure this is same state 
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    ScreenState nextState = (ScreenState)next;

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
            ScreenState gualState = new ScreenState(this);
            // make sure this is same state 
            if (!this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    ScreenState nextState = (ScreenState)next;

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