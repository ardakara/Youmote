using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote
{
    public class ScenarioStateHistory
    {

        private double _falseStateDurationInSeconds;
        private static double DEFAULT_FALSE_STATE_DURATION = 0.3;
        private List<ScenarioState> _history;
        private int _maxSize;

        public List<ScenarioState> History
        {
            get
            {
                return this._history;
            }
        }

        public ScenarioStateHistory(int maxSize)
        {
            this._maxSize = maxSize;
            this._history = new List<ScenarioState>();
            this._falseStateDurationInSeconds =DEFAULT_FALSE_STATE_DURATION;
        }

        public ScenarioStateHistory(int maxSize, double falseStateDurationInSeconds)
        {
            this._maxSize = maxSize;
            this._history = new List<ScenarioState>();
            this._falseStateDurationInSeconds = falseStateDurationInSeconds;
        }

        public List<ScenarioState> getLastNStates(int n)
        {
            List<ScenarioState> lastStates = new List<ScenarioState>();
            for (int i = this.History.Count - 1; i > this.History.Count - 1 - n && i >= 0; i--)
            {
                lastStates.Add(this.History.ElementAt<ScenarioState>(i));
            }
            return lastStates;
        }


        public ScenarioState Pop()
        {
            if (this._history.Count > 0)
            {
                ScenarioState state = this._history[this.History.Count - 1];
                this._history.RemoveAt(this._history.Count - 1);
                return state;
            }
            else
            {
                return null;
            }
        }



        public ScenarioState Peek()
        {
            if (this._history.Count > 0)
            {
                ScenarioState state = this._history[this.History.Count - 1];
                return state;
            }
            else
            {
                return null;
            }
        }
        public void addState(ScenarioState nextState)
        {
            
            if (this._history.Count > 0)
            {
                // previous states have been seen
                // pop off the previous state
                ScenarioState lastState = this.Pop();
                if (lastState.isSameState(nextState))
                {
                    // previous state is same as current state
                    // merge them
                    ScenarioState newState = lastState.mergeEqualStates(nextState);
                    // add new state back on
                    // this replaces current and previous state
                    this._history.Add(newState);
                }
                else
                {
                    // previous state is not the same as the current
                    // cap the previous state
                    ScenarioState cappedOldState = lastState.finishState(nextState);
                    if (cappedOldState.getDurationInSeconds() > this._falseStateDurationInSeconds)
                    {
                        // if the capped previous state has a reasonable length
                        // add it
                        // now add next state
                        this._history.Add(cappedOldState);
                        this._history.Add(nextState);
                    }
                    else if (this._history.Count > 0)
                    {
                        // so previous state didn't last for a reasonable amount of time
                        // get the state before previous state... this one is gaurunteed to be a quality amount of time
                        ScenarioState lastGoodState = this.Pop();
                        if (lastGoodState.isSameState(nextState))
                        {
                            // if the next state is the same as the last good state
                            // merge them
                            ScenarioState newState = lastGoodState.mergeEqualStates(nextState);
                            this._history.Add(newState);
                        }
                        else
                        {

                            ScenarioState cappedLastGoodState = lastGoodState.finishState(nextState);
                            // otherwise cap the last good state
                            this._history.Add(cappedLastGoodState);
                            // add the next state
                            this._history.Add(nextState);
                        }

                    }
                    else
                    {
                        // this was the second state added
                        // weve popped off the previous bad state
                        // put this one one now.
                        this._history.Add(nextState);
                    }

                }
            }
            else
            {
                this._history.Add(nextState);
            }

            if (this._history.Count > this._maxSize)
            {
                int numRemove = this._history.Count - this._maxSize;
                this._history.RemoveRange(0, numRemove);
            }
        }
    }
}
