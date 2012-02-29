using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;
namespace YouMote
{
    public abstract class ResumeDetector : ScenarioDetector
    {

        protected ScenarioStateHistory _rightHandHistory;
        protected ScenarioStateHistory _leftHandHistory;
        protected ResumeIndicator _righthandResumeIndicator = new ResumeIndicator();
        protected ResumeIndicator _lefthandResumeIndicator = new ResumeIndicator();

        public ResumeDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30);
            this._leftHandHistory = new ScenarioStateHistory(30);
        }

        public abstract Boolean isScenarioDetected();


        public void processSkeleton(Skeleton skeleton)
        {

            Boolean rightHandBack = this._righthandResumeIndicator.rightHandBack(skeleton);
            Boolean rightHandFront = this._righthandResumeIndicator.rightHandFront(skeleton);

            Boolean leftHandLeft = this._lefthandResumeIndicator.leftHandBack(skeleton);
            Boolean leftHandRight = this._lefthandResumeIndicator.leftHandFront(skeleton);

            ResumeState state;
            if (rightHandBack)
            {
                state = new ResumeState(ResumeState.ResumePosition.HAND_BACK, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }
            else if (rightHandFront)
            {
                state = new ResumeState(ResumeState.ResumePosition.HAND_FRONT, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }
            else
            {
                state = new ResumeState(ResumeState.ResumePosition.HAND_AWAY, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }

            if (leftHandLeft)
            {
                state = new ResumeState(ResumeState.ResumePosition.HAND_BACK, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
            }
            else if (leftHandRight)
            {
                state = new ResumeState(ResumeState.ResumePosition.HAND_FRONT, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
            }
            else
            {
                state = new ResumeState(ResumeState.ResumePosition.HAND_AWAY, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
            }

        }

    }
}
