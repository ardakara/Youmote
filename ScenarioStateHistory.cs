using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    class ScenarioStateHistory
    {
        private List<ScenarioState> _history;
        public List<ScenarioState> History
        {
            get
            {
                return this._history;
            }
        }

        public ScenarioStateHistory()
        {
            this._history = new List<ScenarioState>();
        }

        public List<ScenarioState> getLastNStates(int n)
        {
            List<ScenarioState> lastStates = new List<ScenarioState>();
            for (int i = this.History.Count - 1; i > this.History.Count - 1 - n && i >=0; i--)
            {
                lastStates.Add(this.History[i]);
            }
            return lastStates;
        }
        public void addState(ScenarioState nextState)
        {
            if (this._history.Count > 0)
            {
                ScenarioState prevState = this._history[this._history.Count - 1];
                if (prevState.isSameState(nextState))
                {
                    // same state, merge them
                    ScenarioState newState = prevState.mergeEqualStates(nextState);
                    this._history[this._history.Count - 1] = newState;
                }
                else
                {
                    ScenarioState newState = prevState.finishState(nextState);
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
