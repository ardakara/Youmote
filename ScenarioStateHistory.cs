using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    public class ScenarioStateHistory
    {
        private static double FALSE_STATE_DURATION_IN_SECONDS = 0;
        private List<ScenarioStateIMPL> _history;
        private int _maxSize;
        public List<ScenarioStateIMPL> History
        {
            get
            {
                return this._history;
            }
        }

        public ScenarioStateHistory(int maxSize)
        {
            this._maxSize = maxSize;
            this._history = new List<ScenarioStateIMPL>();
        }

        public List<ScenarioStateIMPL> getLastNStates(int n)
        {
            List<ScenarioStateIMPL> lastStates = new List<ScenarioStateIMPL>();
            for (int i = this.History.Count - 1; i > this.History.Count - 1 - n && i >= 0; i--)
            {
                lastStates.Add(this.History.ElementAt<ScenarioStateIMPL>(i));
            }
            return lastStates;
        }


        public ScenarioStateIMPL Pop()
        {
            if (this._history.Count > 0)
            {
                ScenarioStateIMPL state = this._history[this.History.Count - 1];
                this._history.RemoveAt(this._history.Count - 1);
                return state;
            }
            else
            {
                return null;
            }
        }



        public ScenarioStateIMPL Peek()
        {
            if (this._history.Count > 0)
            {
                ScenarioStateIMPL state = this._history[this.History.Count - 1];
                return state;
            }
            else
            {
                return null;
            }
        }
        public void addState(ScenarioStateIMPL nextState)
        {
            if (this._history.Count > 0)
            {
                ScenarioStateIMPL lastState = this.Pop();
                if (lastState.isSameState(nextState))
                {
                    // same state, merge them
                    ScenarioStateIMPL newState = lastState.mergeEqualStates(nextState);
                    this._history.Add(newState);
                }
                else
                {
                    ScenarioStateIMPL cappedOldState = lastState.finishState(nextState);
                    if (cappedOldState.getDurationInSeconds() > ScenarioStateHistory.FALSE_STATE_DURATION_IN_SECONDS)
                    {
                        this._history.Add(cappedOldState);
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
