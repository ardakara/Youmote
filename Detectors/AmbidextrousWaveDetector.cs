﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking.Detectors
{
    public class AmbidextrousWaveDetector : WaveDetector
    {
        protected static double MAX_WAVE_DURATION = 1;

        private Boolean isWaveDetected(ScenarioStateHistory history)
        {
            List<ScenarioStateIMPL> recentStates = history.getLastNStates(3);

            double wave_duration = 0;
            for (int i = 0; i < recentStates.Count; i++)
            {
                wave_duration += recentStates[i].getDurationInMilliseconds();
                if (recentStates.Count == 3)
                {
                    Console.WriteLine("\t RS 0: " + recentStates[0].ToString() + ", RS 1: " + recentStates[1].ToString() + ", RS 2: " + recentStates[2].ToString());
                }
            }
            if (wave_duration < 3000 && recentStates.Count >= 3 )
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
            return (rightHandWaved || leftHandWaved);
        }

    }
}
