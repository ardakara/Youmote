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
    public abstract class StiffSwipeRightDetector : StiffSwipeDetector
    {

        private void processRightSwipeRightHand(Joint rightHand, double swipeCrossLine)
        {
            double curX = rightHand.Position.X;
            double curY = rightHand.Position.Y;
            double curZ = rightHand.Position.Z;


            //if right hand is right of cross line AND the person hasn't started a swipe yet, start the swipe!
            if (!this.rh_swipeInitiated)
            {
                if (!isRightOfCrossline(swipeCrossLine, rightHand.Position.X))
                {
                    this.rh_start.X = rightHand.Position.X;
                    this.rh_start.Y = rightHand.Position.Y;
                    this.rh_start.Z = rightHand.Position.Z;

                    this.rh_last.X = rightHand.Position.X;
                    this.rh_last.Y = rightHand.Position.Y;
                    this.rh_last.Z = rightHand.Position.Z;

                    this.rh_endX = this.rh_start.X + MIN_SWIPE_DISTANCE;
                    this.rh_swipeInitiated = true;
                    SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_STARTED, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                }
            }
            else
            { //the swipe has been started, check if in bounds and moving to the right!

                if (stillWithinYBounds(rightHand.Position.Y, this.rh_start) && curX >= this.rh_last.X)
                {

                    if (curX > this.rh_endX)
                    { //they've completed the swipe!
                        SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_FINISHED, DateTime.Now, DateTime.Now);
                        this._rightHandHistory.addState(state);
                    }
                    else
                    {
                        SwipeState state = new SwipeState(SwipeState.SwipePosition.MOVING_RIGHT, DateTime.Now, DateTime.Now);
                        this._rightHandHistory.addState(state);
                    }
                    this.rh_last.X = curX;
                    this.rh_last.Y = curY;
                    this.rh_last.Z = curZ;
                }
                else
                {
                    resetVars("right");
                    SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_CANCELLED, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                }
            }
        }

        private void processRightSwipeLeftHand(Joint leftHand, double swipeCrossLine)
        {
            double curX = leftHand.Position.X;
            double curY = leftHand.Position.Y;
            double curZ = leftHand.Position.Z;


            //if right hand is right of cross line AND the person hasn't started a swipe yet, start the swipe!
            if (!this.lh_swipeInitiated)
            {
                if (!isRightOfCrossline(swipeCrossLine, leftHand.Position.X))
                {
                    this.lh_start.X = leftHand.Position.X;
                    this.lh_start.Y = leftHand.Position.Y;
                    this.lh_start.Z = leftHand.Position.Z;

                    this.lh_last.X = leftHand.Position.X;
                    this.lh_last.Y = leftHand.Position.Y;
                    this.lh_last.Z = leftHand.Position.Z;

                    this.lh_endX = this.lh_start.X + MIN_SWIPE_DISTANCE;
                    this.lh_swipeInitiated = true;
                    SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_STARTED, DateTime.Now, DateTime.Now);
                    this._leftHandHistory.addState(state);
                }
            }
            else
            { //the swipe has been started, check if in bounds and moving to the right!

                if (stillWithinYBounds(leftHand.Position.Y, this.lh_start) && curX >= this.lh_last.X)
                {

                    if (curX > this.lh_endX)
                    { //they've completed the swipe!
                        SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_FINISHED, DateTime.Now, DateTime.Now);
                        this._leftHandHistory.addState(state);
                    }
                    else
                    {
                        SwipeState state = new SwipeState(SwipeState.SwipePosition.MOVING_RIGHT, DateTime.Now, DateTime.Now);
                        this._leftHandHistory.addState(state);
                    }
                    this.lh_last.X = curX;
                    this.lh_last.Y = curY;
                    this.lh_last.Z = curZ;
                }
                else
                {
                    resetVars("left");
                    SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_CANCELLED, DateTime.Now, DateTime.Now);
                    this._leftHandHistory.addState(state);
                }
            }
        }

        public override void processSkeleton(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                //ONLY TO FIGURE OUT THE DISTANCE--otherwise, hardcode it into swipeRightDetector

                Joint rightHand = skeleton.Joints[JointType.HandRight];
                double rightArmSwipeCrossLine = skeleton.Joints[JointType.ShoulderRight].Position.X;
                //double rightArmSwipeCrossLine = calculate_midpoint(skeleton.Joints[JointType.ShoulderRight].Position.X, skeleton.Joints[JointType.ShoulderCenter].Position.X);

                Joint leftHand = skeleton.Joints[JointType.HandLeft];
                double leftArmSwipeCrossLine = skeleton.Joints[JointType.ShoulderLeft].Position.X;
                //double leftArmSwipeCrossLine = calculate_midpoint(skeleton.Joints[JointType.ShoulderLeft].Position.X, skeleton.Joints[JointType.ShoulderCenter].Position.X);

                Boolean isRightArmStraight = this._straightArmIndicator.isRightArmStraight(skeleton, true);
                Boolean isLeftArmStraight = this._straightArmIndicator.isLeftArmStraight(skeleton, true);

                if (isRightArmStraight)
                {
                    processRightSwipeRightHand(rightHand, rightArmSwipeCrossLine);
                }
                else
                {
                    //Console.WriteLine("Right arm wasn't straight-cancel!");
                    resetVars("right");
                    SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_CANCELLED, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                }

                if (isLeftArmStraight)
                {
                    processRightSwipeLeftHand(leftHand, leftArmSwipeCrossLine);
                }
                else
                {
                    resetVars("left");
                    SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_CANCELLED, DateTime.Now, DateTime.Now);
                    this._leftHandHistory.addState(state);
                }


            }
        }
    }
}
