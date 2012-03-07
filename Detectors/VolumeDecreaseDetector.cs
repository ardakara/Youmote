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
    public class VolumeDecreaseDetector : VolumeDetector
    {

        private void processVolumeDecreaseRightHand(Joint rightHand, double adjustCrossLine) 
        {
            double curX = rightHand.Position.X;
            double curY = rightHand.Position.Y;
            double curZ = rightHand.Position.Z;

            //if right hand is right of cross line AND the person hasn't started a swipe yet, start the swipe!
            if (!this.rh_adjustInitiated)
            {
                this.rh_start.X = rightHand.Position.X;
                this.rh_start.Y = rightHand.Position.Y;
                this.rh_start.Z = rightHand.Position.Z;

                this.rh_last.X = rightHand.Position.X;
                this.rh_last.Y = rightHand.Position.Y;
                this.rh_last.Z = rightHand.Position.Z;
                
                this.rh_adjustInitiated = true;
                VolumeState state = new VolumeState(VolumeState.VolumePosition.ADJUST_INITIATED, DateTime.Now, DateTime.Now);
                this._rightHandHistory.addState(state);
            }
            else
            { //the swipe has been started, check if in bounds and moving to the left!

                if (stillWithinXBounds(curX, this.rh_start))
                {
                    VolumeState state = new VolumeState(VolumeState.VolumePosition.ADJUST_VOLUME, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                    this.rh_deltaY = curY - rh_last.Y;
               
                    this.rh_last.X = curX;
                    this.rh_last.Y = curY;
                    this.rh_last.Z = curZ;
                }
                else //if NOT in y bounds OR is moving to the right!
                {
                    resetVars("right");
                    VolumeState state = new VolumeState(VolumeState.VolumePosition.NON_VOLUME, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                }
            }
        }

        public override bool isScenarioDetected()
        {
            return false;
        }

        public double getVolumeDelta() {
            List<ScenarioState> recentStates = _rightHandHistory.getLastNStates(2);
            if (recentStates.Count == 2)
            {
                Console.WriteLine("\t VOLUME RS 0: " + recentStates[0].ToString() + ", RS 1: " + recentStates[1].ToString());
            }

            if (recentStates.Count >= 2) 
            {
                if (VolumeState.ADJUST_VOLUME.isSameState(recentStates[0]) && VolumeState.ADJUST_INITIATED.isSameState(recentStates[1]) )
                {
                    Console.WriteLine("rh_deltaY: " + this.rh_deltaY);
                    double change_since_start = this.rh_last.Y - this.rh_start.Y;
                    Console.WriteLine("rh delta since arm straight: " + change_since_start);
                    return (this.rh_deltaY / 0.8);
                    //return (this.rh_last.Y - this.rh_start.Y);
                }
            }
            return 0;
        }


        public override void processSkeleton(Skeleton skeleton)
        {
            if (skeleton != null)
            {

                //ONLY TO FIGURE OUT THE DISTANCE--otherwise, hardcode it into swipeLeftDetector

                Joint rightHand = skeleton.Joints[JointType.HandRight];
                double swipeCrossLine = skeleton.Joints[JointType.ShoulderRight].Position.Y;


                Boolean isRightArmStraight = this._straightArmIndicator.isRightArmStraight(skeleton, false);
                if (isRightArmStraight)
                {
                    processVolumeDecreaseRightHand(rightHand, swipeCrossLine);
                }
                else
                {
                    resetVars("right");
                    VolumeState state = new VolumeState(VolumeState.VolumePosition.NON_VOLUME, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                }

            }
        }
    }
}
