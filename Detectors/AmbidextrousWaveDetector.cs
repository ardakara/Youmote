using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Indicators;
using YouMote.States;

namespace YouMote
{
    public class AmbidextrousWaveDetector : WaveDetector
    {

        private Boolean isWaveDetected(ScenarioStateHistory history)
        {
            List<ScenarioState> recentStates = history.getLastNStates(3);

            if (recentStates.Count == 3)
            {
                //Console.WriteLine("\t RS 0: " + recentStates[0].ToString() + ", RS 1: " + recentStates[1].ToString() + ", RS 2: " + recentStates[2].ToString());
            }

            double wave_duration = 0;
            for (int i = 0; i < recentStates.Count; i++)
            {
                wave_duration += recentStates[i].getDurationInMilliseconds();
            }

            if (wave_duration < MAX_WAVE_DURATION && recentStates.Count >= 3 )
            {
                if (WaveState.HAND_RIGHT.isSameState(recentStates[0]) && WaveState.HAND_LEFT.isSameState(recentStates[1]) && WaveState.HAND_RIGHT.isSameState(recentStates[2]))
                {
                    return true;
                }
                else if (WaveState.HAND_LEFT.isSameState(recentStates[0]) && WaveState.HAND_RIGHT.isSameState(recentStates[1]) && WaveState.HAND_LEFT.isSameState(recentStates[2]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }

        public override Boolean isScenarioDetected()
        {
            Boolean rightHandWaved = isWaveDetected(this._rightHandHistory);
            Boolean leftHandWaved = isWaveDetected(this._leftHandHistory);

            double rh_diff = this._rh_most_right - this._rh_most_left;
            double lh_diff = this._lh_most_right - this._lh_most_left;

            if ( (rightHandWaved && IsWithinBounds(rh_diff) ) || (leftHandWaved && IsWithinBounds(lh_diff) ) )
            {
                WaveState state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
                this._leftHandHistory.addState(state);
                reset_extremes("right");
                reset_extremes("left");
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
