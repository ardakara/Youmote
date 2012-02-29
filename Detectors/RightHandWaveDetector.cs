using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking.Detectors
{
    public class RightHandWaveDetector : WaveDetector
    {
        protected static double MAX_WAVE_DURATION = 1;
        public override Boolean isScenarioDetected()
        {
            List<ScenarioStateIMPL> recentStates = this._history.getLastNStates(3);
            Boolean isDetected = true;
            
            ScenarioStateIMPL firstState = recentStates[0];
            if (WaveState.HAND_BELOW.Equals(firstState)) isDetected = false;
            double wave_duration = 0;
            for (int i = 1; i < recentStates.Count; i++)
            {
                ScenarioStateIMPL state = recentStates[i];
                wave_duration += state.getDurationInMilliseconds();
                if (WaveState.HAND_LEFT.Equals(firstState)) 
                { //the hand went left first, so evens should be left, odds should be right.
                    if ( i%2 == 1 && !WaveState.HAND_RIGHT.Equals(state)) {
                        return false;
                    } else if ( i%2 == 1 && !WaveState.HAND_LEFT.Equals(state)) {
                        return false;
                    }
                }
                else if (WaveState.HAND_RIGHT.Equals(firstState))
                { //the hand went right first. so evens should be right, odds should be left.
                    if (i%2 == 1 && !WaveState.HAND_LEFT.Equals(state))
                    {
                        return false;
                    }
                    else if (i%2 == 1 && !WaveState.HAND_RIGHT.Equals(state))
                    {
                        return false;
                    }
                }
            }

            if (wave_duration > 2)
            {
                return false;
            }

            return isDetected;
        }

    }
}
