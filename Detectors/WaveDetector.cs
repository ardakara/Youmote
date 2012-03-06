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
    public abstract class WaveDetector : ScenarioDetector
    {

        protected static double MAX_WAVE_DURATION = 1000;
        protected static double MAX_WAVE_DISTANCE = 0.2f;
        protected static double MIN_WAVE_DISTANCE = 0.05f;

        protected ScenarioStateHistory _rightHandHistory;
        protected ScenarioStateHistory _leftHandHistory;
        protected WaveIndicator _righthandwaveindicator = new WaveIndicator();
        protected WaveIndicator _lefthandwaveindicator = new WaveIndicator();

        protected double _rh_most_left;
        protected double _rh_most_right;

        protected double _lh_most_left;
        protected double _lh_most_right;

        protected void reset_extremes(String hand) {
            if (hand.Equals("right") )
            {
                this._rh_most_right = Double.MinValue;
                this._rh_most_left = Double.MaxValue;
            }

            if (hand.Equals("left"))
            {
                this._lh_most_right = Double.MinValue;
                this._lh_most_left = Double.MaxValue;
            }
        }

        public WaveDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30,0);
            this._leftHandHistory = new ScenarioStateHistory(30,0);
            reset_extremes("right");
            reset_extremes("left");
        }


        protected Boolean IsWithinBounds(double diff)
        {

            if ((diff > MIN_WAVE_DISTANCE) && (diff < MAX_WAVE_DISTANCE))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public abstract Boolean isScenarioDetected();

        private void update_rh_state(Joint rightHand, Boolean rightHandLeft, Boolean rightHandRight)
        {
            WaveState state;
            if (rightHandLeft)
            {
                state = new WaveState(WaveState.WavePosition.HAND_LEFT, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
                if (rightHand.Position.X < this._rh_most_left)
                {
                    this._rh_most_left = rightHand.Position.X;
                }
            }
            else if (rightHandRight)
            {
                state = new WaveState(WaveState.WavePosition.HAND_RIGHT, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
                if (rightHand.Position.X > this._rh_most_right)
                {
                    this._rh_most_right = rightHand.Position.X;
                }
            }
            else
            {
                state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
                reset_extremes("right");
            }
        }

        private void update_lh_state(Joint leftHand, Boolean leftHandLeft, Boolean leftHandRight)
        {
            WaveState state;

            if (leftHandLeft)
            {
                state = new WaveState(WaveState.WavePosition.HAND_LEFT, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
                if (leftHand.Position.X < this._lh_most_left)
                {
                    this._lh_most_left = leftHand.Position.X;
                }
            }
            else if (leftHandRight)
            {
                state = new WaveState(WaveState.WavePosition.HAND_RIGHT, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
                if (leftHand.Position.X > this._lh_most_right)
                {
                    this._lh_most_right = leftHand.Position.X;
                }
            }
            else
            {
                state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
                this._leftHandHistory.addState(state);
                reset_extremes("left");
            }
        }


        public void processSkeleton(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                Boolean rightHandLeft = this._righthandwaveindicator.rightHandLeft(skeleton);
                Boolean rightHandRight = this._righthandwaveindicator.rightHandRight(skeleton);

                Boolean leftHandLeft = this._lefthandwaveindicator.leftHandLeft(skeleton);
                Boolean leftHandRight = this._lefthandwaveindicator.leftHandRight(skeleton);

                Joint rightHand = skeleton.Joints[JointType.HandRight];
                Joint leftHand = skeleton.Joints[JointType.HandLeft];

                //update right hand state
                update_rh_state(rightHand, rightHandLeft, rightHandRight);
                double rh_diff = this._rh_most_right - this._rh_most_left;

                if (!IsWithinBounds(rh_diff) && rh_diff > 0)
                {
                    WaveState state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
                    this._leftHandHistory.addState(state);
                    reset_extremes("right");
                }

                //update left hand state
                update_lh_state(leftHand, leftHandLeft, leftHandRight);
                double lh_diff = this._lh_most_right - this._lh_most_left;

                if (!IsWithinBounds(lh_diff) && rh_diff > 0)
                {
                    WaveState state = new WaveState(WaveState.WavePosition.HAND_BELOW, DateTime.Now, DateTime.Now);
                    this._leftHandHistory.addState(state);
                    reset_extremes("left");
                }
            }

        }
    }
}
