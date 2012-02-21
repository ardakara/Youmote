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

        public void addState(ScenarioState nextState){
            ScenarioState prevState = this._history[this._history.Count - 1];
            if (prevState.equals(nextState))
            {
                // same state, merge them
                prevState.mergeEqualStates(nextState);
            }
            else
            {
                prevState.finishState(nextState);
                this._history.Add(nextState);
            }
        }
    }
}
