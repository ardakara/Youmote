using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;
using YouMote.States;

namespace YouMote
{
    public abstract class MuteDetector : ScenarioDetector
    {

        protected ScenarioStateHistory _rightHandHistory;
        //protected ScenarioStateHistory _leftHandHistory;
        protected RightHandOnFaceIndicator _rightHandOnFaceIndicator = new RightHandOnFaceIndicator();
        //protected ResumeIndicator _lefthandResumeIndicator = new ResumeIndicator();

        public MuteDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30);
            //this._leftHandHistory = new ScenarioStateHistory(30);
        }

        public abstract Boolean isScenarioDetected();


        public void processSkeleton(Skeleton skeleton)
        {

            Boolean rightHandOnFace = _rightHandOnFaceIndicator.isPositionDetected(skeleton);

            ResumeState state;
            if (rightHandOnFace)
            {
                double xPos = skeleton.Joints[JointType.HandRight].Position.X;
                double yPos = skeleton.Joints[JointType.HandRight].Position.Y;
                state = new ResumeState(ResumeState.ResumePosition.HAND_BACK, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }
            else
            {
                state = new ResumeState(ResumeState.ResumePosition.HAND_FRONT, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }
        }

    }
}
