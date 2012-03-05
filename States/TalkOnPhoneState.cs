using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;

namespace YouMote.States
{
    class TalkOnPhoneState : ScenarioState
    {

        public enum PhoneState { HAND_DOWN, HAND_ON_FACE, TALK_ON_PHONE};


        private DateTime _start;
        private DateTime _end;
        private PhoneState _state;

        public static TalkOnPhoneState HAND_DOWN = new TalkOnPhoneState(PhoneState.HAND_DOWN, DateTime.Now, DateTime.Now);
        public static TalkOnPhoneState HAND_ON_FACE = new TalkOnPhoneState(PhoneState.HAND_ON_FACE, DateTime.Now, DateTime.Now);
        public static TalkOnPhoneState TALK_ON_PHONE = new TalkOnPhoneState(PhoneState.TALK_ON_PHONE, DateTime.Now, DateTime.Now);
        

        PhoneState State
        {
            get
            {
                return this._state;
            }
        }

        public TalkOnPhoneState(PhoneState state, DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
            this._state = state;
        }

        public TalkOnPhoneState(TalkOnPhoneState phoneState)
        {
            this._start = phoneState._start;
            this._end = phoneState._end;
            this._state = phoneState._state;
        }

        public Boolean isSameState(ScenarioState ss)
        {
            if (!ss.GetType().Equals(this.GetType()))
            {
                return false;
            }
            else
            {
                return this.State == ((TalkOnPhoneState)ss).State;
            }
        }

        public ScenarioState mergeEqualStates(ScenarioState next)
        {
            TalkOnPhoneState curState = new TalkOnPhoneState(this);
            if (this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    TalkOnPhoneState nextState = (TalkOnPhoneState)next;

                    // make gualState's start be the earlier of the two
                    if (nextState._start.CompareTo(curState._start) < 0)
                    {
                        curState._start = nextState._start;
                    }

                    // make gualState's start be the later of the two
                    if (nextState._end.CompareTo(curState._end) > 0)
                    {
                        curState._end = nextState._end;
                    }
                }
            }
            return curState;
        }

        public ScenarioState finishState(ScenarioState next)
        {
            TalkOnPhoneState curState = new TalkOnPhoneState(this);
            // make sure this is same state 
            if (!this.isSameState(next))
            {
                if (next.GetType().Equals(this.GetType()))
                {
                    TalkOnPhoneState nextState = (TalkOnPhoneState)next;

                    // make gualState's start be the earlier of the two
                    if (nextState._start.CompareTo(curState._start) < 0)
                    {
                        curState._start = nextState._start;
                    }
                    // make gualState's start be the later of the two
                    if (nextState._end.CompareTo(curState._end) > 0)
                    {
                        curState._end = nextState._end;
                    }
                }
            }
            return curState;
        }

        public double getDurationInMinutes()
        {
            return this._end.Subtract(this._start).TotalMinutes;
        }

        public double getDurationInSeconds()
        {
            return this._end.Subtract(this._start).TotalSeconds;
        }

        public double getDurationInMilliseconds()
        {
            return this._end.Subtract(this._start).TotalMilliseconds;
        }
    }
}
