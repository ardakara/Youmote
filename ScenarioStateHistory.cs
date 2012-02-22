using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    class ScenarioStateHistory
    {
        private static double FALSE_STATE_DURATION_IN_SECONDS = 1;
        private Stack<ScenarioStateIMPL> _history;
        private int _maxSize;
        public Stack<ScenarioStateIMPL> History
        {
            get
            {
                return this._history;
            }
        }

        public ScenarioStateHistory(int maxSize)
        {
            this._maxSize = maxSize;
            this._history = new Stack<ScenarioStateIMPL>();
        }

        public void smoothHistory()
        {

            ScenarioStateIMPL last = this._history.Pop();
            Stack<ScenarioStateIMPL>.Enumerator it = this._history.GetEnumerator();
            while (it.MoveNext())
            {
                if (it.Current.getDurationInSeconds() < ScenarioStateHistory.FALSE_STATE_DURATION_IN_SECONDS)
                {
                    it.Dispose();
                }
            }
            this._history.Push(last);
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
        public void addState(ScenarioStateIMPL nextState)
        {
            if (this._history.Count > 0)
            {
                ScenarioStateIMPL lastState = this._history.Pop();
                if (lastState.isSameState(nextState))
                {
                    // same state, merge them
                    ScenarioStateIMPL newState = lastState.mergeEqualStates(nextState);
                    this._history.Push(newState);
                }
                else
                {
                    ScenarioStateIMPL newState = lastState.finishState(nextState);
                    this._history.Push(newState);
                    this._history.Push(nextState);
                }
            }
            else
            {
                this._history.Push(nextState);
            }

            if (this._history.Count > this._maxSize)
            {
                // holding more states than maxSize allows
                Stack<ScenarioStateIMPL>.Enumerator it = this._history.GetEnumerator();
                if (it.MoveNext())
                {
                        it.Dispose();
                }
            }
        }
    }
}
