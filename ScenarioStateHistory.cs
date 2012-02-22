using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    class ScenarioStateHistory
    {
        private List<ScenarioStateIMPL> _history;
        public List<ScenarioStateIMPL> History
        {
            get
            {
                return this._history;
            }
        }

        public ScenarioStateHistory()
        {
            this._history = new List<ScenarioStateIMPL>();
        }

        public List<ScenarioStateIMPL> getLastNStates(int n)
        {
            List<ScenarioStateIMPL> lastStates = new List<ScenarioStateIMPL>();
            for (int i = this.History.Count - 1; i > this.History.Count - 1 - n && i >=0; i--)
            {
                lastStates.Add(this.History[i]);
            }
            return lastStates;
        }
        public void addState(ScenarioStateIMPL nextState)
        {
            if (this._history.Count > 0)
            {
                ScenarioStateIMPL prevState = this._history[this._history.Count - 1];
                if (prevState.isSameState(nextState))
                {
                    // same state, merge them
                    ScenarioStateIMPL newState = prevState.mergeEqualStates(nextState);
                    this._history[this._history.Count - 1] = newState;
                }
                else
                {
                    ScenarioStateIMPL newState = prevState.finishState(nextState);
                    this._history[this._history.Count - 1] = newState;
                    this._history.Add(nextState);
                }
            }
            else
            {
                this._history.Add(nextState);
            }
        }
    }
}
