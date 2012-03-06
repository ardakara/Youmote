using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouMote
{
    public class ScenarioStateHistory
    {

        private double _falseStateDuration = 0.3;
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
        }

        public ScenarioStateHistory(int maxSize, double falseStateDuration)
        {
            this._maxSize = maxSize;
            this._history = new List<ScenarioState>();
            this._falseStateDuration = falseStateDuration;
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
                ScenarioState lastState = this.Pop();
                if (lastState.isSameState(nextState))
                {
                    // same state, merge them
                    ScenarioState newState = lastState.mergeEqualStates(nextState);
                    this._history.Add(newState);
                }
                else
                {
                    ScenarioState cappedOldState = lastState.finishState(nextState);
                    if (cappedOldState.getDurationInSeconds() > this._falseStateDuration)
                    {
                        this._history.Add(cappedOldState);
                    }
                    else if(this._history.Count>0)
                    {
                        ScenarioState lastGoodState = this.Pop();

                        if (lastGoodState.isSameState(nextState))
                        {
                            // same state, merge them
                            ScenarioState newState = lastGoodState.mergeEqualStates(nextState);
                            this._history.Add(newState);
                        }
                        else
                        {
                            ScenarioState cappedLastGoodState = lastGoodState.finishState(nextState);
                            this._history.Add(cappedLastGoodState);
                        }
                    }

                    this._history.Add(nextState);
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
