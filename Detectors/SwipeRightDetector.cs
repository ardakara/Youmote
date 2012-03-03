using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;
namespace YouMote
{
    public abstract class SwipeRightDetector : ScenarioDetector
    {

        protected ScenarioStateHistory _rightHandHistory;
        protected PullDownIndicator _straightArmIndicator = new PullDownIndicator();
        private double startX;
        private double startY;

        private double lastX;
        private double lastY;

        private double endX;
        private Boolean swipeInitiated;

        private static double MAX_Y_ALLOWANCE = 0.05f;
        private static double MIN_SWIPE_DISTANCE = 0.3f;

        public SwipeRightDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30);
            swipeInitiated = false;
            startX = -1;
            startY = -1;
            endX = -1;
        }

        public abstract Boolean isScenarioDetected();

        private double calculate_midpoint(double pos1, double pos2)
        {
            return (pos1 + pos2) / 2;
        }

        private Boolean isRightOfCrossLine(double crossline_x, double joint_x) {
            return (joint_x < crossline_x);
        }

        private Boolean isRightOfCrossline(double crossline_x, double joint_y) {
            return (joint_y > crossline_x);
        }

        private Boolean stillWithinBounds(double righthand_y)
        {
            Console.WriteLine(Math.Abs(righthand_y - startY));
            return (Math.Abs(righthand_y - startY) < MAX_Y_ALLOWANCE);
        }

        private void resetVars()
        {
            swipeInitiated = false;
            startX = -1;
            startY = -1;
            lastX = -1;
            lastY = -1;
            endX = -1;
        }

        public void processSkeleton(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                //ONLY TO FIGURE OUT THE DISTANCE--otherwise, hardcode it into swipeRightDetector

                Joint rightHand = skeleton.Joints[JointType.HandRight];
                double rightArmSwipeCrossLine = calculate_midpoint(skeleton.Joints[JointType.ShoulderRight].Position.X, skeleton.Joints[JointType.ShoulderCenter].Position.X);

                double curX = skeleton.Joints[JointType.HandRight].Position.X;
                double curY = skeleton.Joints[JointType.HandRight].Position.Y;


                //if right hand is right of cross line AND the person hasn't started a swipe yet, start the swipe!
                if ( !isRightOfCrossline(rightArmSwipeCrossLine, rightHand.Position.X) && !swipeInitiated ) 
                {
                    startX = rightHand.Position.X;
                    startY = rightHand.Position.Y;

                    lastX = rightHand.Position.X;
                    lastY = rightHand.Position.Y;

                    endX = startX + (MIN_SWIPE_DISTANCE);
                    swipeInitiated = true;
                    SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_STARTED, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                    //ADD START STATE
                } else { //the swipe has been started, check if in bounds and moving to the left!

                    if (stillWithinBounds(rightHand.Position.Y) ){
                        Console.WriteLine("within bounds! \n");
                    }

                    if (curX >= lastX) {
                        Console.WriteLine("moving right!) \n");   
                    }

                    if (stillWithinBounds(rightHand.Position.Y) && curX >= lastX) {

                        if (curX > endX) { //they've completed the swipe!
                            SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_FINISHED, DateTime.Now, DateTime.Now);
                            this._rightHandHistory.addState(state);
                        } else {
                            SwipeState state = new SwipeState(SwipeState.SwipePosition.MOVING_RIGHT, DateTime.Now, DateTime.Now);
                            this._rightHandHistory.addState(state);
                        }
                        lastX = curX;
                        lastY = curY;
                    } else {
                        resetVars();
                        SwipeState state = new SwipeState(SwipeState.SwipePosition.SWIPE_CANCELLED, DateTime.Now, DateTime.Now);
                        this._rightHandHistory.addState(state);
                        //ADD CANCEL STATE
                    }
                }
            }
        }
    }
}
