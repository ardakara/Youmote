using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Indicators;

namespace YouMote.States
{


    public class WaveState : ScenarioState
{

        public enum WavePosition { HAND_LEFT, HAND_RIGHT, HAND_BELOW};

        public static WaveState HAND_LEFT = new WaveState(WavePosition.HAND_LEFT, DateTime.Now, DateTime.Now);
        public static WaveState HAND_RIGHT = new WaveState(WavePosition.HAND_RIGHT, DateTime.Now, DateTime.Now);
        public static WaveState HAND_BELOW = new WaveState(WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);

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
        private WavePosition _pos;

        WavePosition Pos
        {
            get
            {
                return this._pos;
            }
        }

        public WaveState(WavePosition pos, DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
            this._pos = pos;
        }

        public WaveState(WaveState goalState)
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
                return this.Pos == ((WaveState)ss).Pos;
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
            WaveState gualState = new WaveState(this);
            // make sure this is same state 
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    WaveState nextState = (WaveState)next;

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
            WaveState gualState = new WaveState(this);
            // make sure this is same state 
            if (!this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    WaveState nextState = (WaveState)next;

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