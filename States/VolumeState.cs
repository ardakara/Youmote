using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Indicators;

namespace YouMote.States
{

    public class VolumeState : ScenarioState
{

        public enum VolumePosition { ADJUST_INITIATED, ADJUST_PREPARE, ADJUST_VOLUME, NON_VOLUME};

        public static VolumeState ADJUST_INITIATED = new VolumeState(VolumePosition.ADJUST_INITIATED, DateTime.Now, DateTime.Now);
        public static VolumeState ADJUST_PREPARE = new VolumeState(VolumePosition.ADJUST_PREPARE, DateTime.Now, DateTime.Now);
        public static VolumeState ADJUST_VOLUME = new VolumeState(VolumePosition.ADJUST_VOLUME, DateTime.Now, DateTime.Now);
        public static VolumeState NON_VOLUME = new VolumeState(VolumePosition.NON_VOLUME, DateTime.Now, DateTime.Now);

        private DateTime _start;
        private DateTime _end;
        private VolumePosition _pos;

        VolumePosition Pos
        {
            get
            {
                return this._pos;
            }
        }

        public VolumeState(VolumePosition pos, DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
            this._pos = pos;
        }

        public VolumeState(VolumeState goalState)
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
                return this.Pos == ((VolumeState)ss).Pos;
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
            VolumeState gualState = new VolumeState(this);
            // make sure this is same state 
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    VolumeState nextState = (VolumeState)next;

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
            VolumeState gualState = new VolumeState(this);
            // make sure this is same state 
            if (!this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    VolumeState nextState = (VolumeState)next;

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