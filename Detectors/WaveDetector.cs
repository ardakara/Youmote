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

        protected ScenarioStateHistory _rightHandHistory;
        protected ScenarioStateHistory _leftHandHistory;
        protected WaveIndicator _righthandwaveindicator = new WaveIndicator();
        protected WaveIndicator _lefthandwaveindicator = new WaveIndicator();

        public WaveDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30);
            this._leftHandHistory = new ScenarioStateHistory(30);
        }

        public abstract Boolean isScenarioDetected();


        public void processSkeleton(Skeleton skeleton)
        {

            Boolean rightHandLeft = this._righthandwaveindicator.rightHandLeft(skeleton);
            Boolean rightHandRight = this._righthandwaveindicator.rightHandRight(skeleton);

            Boolean leftHandLeft = this._lefthandwaveindicator.leftHandLeft(skeleton);
            Boolean leftHandRight = this._lefthandwaveindicator.leftHandRight(skeleton);

            WaveState state;
            if (rightHandLeft)
            {
                state = new WaveState(WaveState.WavePosition.HAND_LEFT, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }
            else if (rightHandRight)
            {
                state = new WaveState(WaveState.WavePosition.HAND_RIGHT, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }
            else
            {
                state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }

            if (leftHandLeft)
            {
                state = new WaveState(WaveState.WavePosition.HAND_LEFT, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
            }
            else if (leftHandRight)
            {
                state = new WaveState(WaveState.WavePosition.HAND_RIGHT, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
            }
            else
            {
                state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
            }

        }

    }
}
