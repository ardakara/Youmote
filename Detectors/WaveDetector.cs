using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using SkeletalTracking.Indicators;
namespace SkeletalTracking
{
    public abstract class WaveDetector : ScenarioDetectorIMPL
    {

        protected ScenarioStateHistory _history;
        protected WaveIndicator _righthandwaveindicator = new WaveIndicator();

        public WaveDetector()
        {
            this._history = new ScenarioStateHistory(30);
        }

        public abstract Boolean isScenarioDetected();


        public void processSkeleton(Skeleton skeleton)
        {


            Boolean rightHandLeft = this._righthandwaveindicator.rightHandLeft(skeleton);
            Boolean rightHandRight = this._righthandwaveindicator.rightHandRight(skeleton);
            WaveState state;
            if (rightHandLeft)
            {
                state = new WaveState(WaveState.WavePosition.HAND_LEFT, DateTime.Now, DateTime.Now);
            }
            else if (rightHandRight)
            {
                state = new WaveState(WaveState.WavePosition.HAND_RIGHT, DateTime.Now, DateTime.Now);
            }
            else
            {
                state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
            }

            this._history.addState(state);

        }

    }
}
