using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;
namespace YouMote
{
    public abstract class PullDownScreenDetector : ScenarioDetector
    {

        protected ScenarioStateHistory _rightHandHistory;
        protected PullDownIndicator _straightArmIndicator = new PullDownIndicator();
        protected static double MAX_PULL_DURATION = 3000;
        private double startX;
        private double startY;

        private double lastX;
        private double lastY;

        private double allowanceX;
        private double endY;
        private Boolean pullInitiated;

        public PullDownScreenDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30);
            pullInitiated = false;
            startX = -1;
            startY = -1;
            allowanceX = 1000;
            endY = -1;
        }

        public abstract Boolean isScenarioDetected();


        public void processSkeleton(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                Boolean rightArmStraight = this._straightArmIndicator.rightArmStraight(skeleton);
                Boolean rightArmUp = this._straightArmIndicator.rightArmUp(skeleton);

                ScreenState state;
                double curX = skeleton.Joints[JointType.HandRight].Position.X;
                double curY = skeleton.Joints[JointType.HandRight].Position.Y;
                if (rightArmStraight && rightArmUp && !pullInitiated)
                {
                    state = new ScreenState(ScreenState.ScreenPosition.ARM_STRAIGHTUP, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                    startX = curX;
                    startY = curY;

                    lastX = startX;
                    lastY = startY;
                    
                    endY = startY - (skeleton.Joints[JointType.ShoulderCenter].Position.Y - skeleton.Joints[JointType.HipCenter].Position.Y);
                    double x = startY - endY;
                    //Console.WriteLine("The distance b/w shoulder center and hip center is :" + x);
                    Console.WriteLine("startY " + startY + ", endY: " + endY);
                    
                    pullInitiated = true;
                }
                else if (pullInitiated)
                {
                    if (Math.Abs(curX - startX) < allowanceX && (curY < lastY))
                    {
                        Console.WriteLine("WOO! curY: " + curY + ", lastY: " + lastY);
                        if (curY <= endY) //they went far down enough!
                        {
                            Console.WriteLine("woo!");
                            state = new ScreenState(ScreenState.ScreenPosition.ARM_DOWN, DateTime.Now, DateTime.Now);
                            this._rightHandHistory.addState(state);
                        }
                        else
                        {
                            Console.WriteLine("On the way down...");
                            state = new ScreenState(ScreenState.ScreenPosition.ARM_MOVINGDOWN, DateTime.Now, DateTime.Now);
                            this._rightHandHistory.addState(state);
                        }
                        lastX = curX;
                        lastY = curY;
                    }
                    else //they didn't go down straight enough or they went up!
                    {
                        state = new ScreenState(ScreenState.ScreenPosition.ARM_ELSEWHERE, DateTime.Now, DateTime.Now);
                        this._rightHandHistory.addState(state);
                        startX = -1;
                        lastX = -1;
                        startY = -1;
                        lastY = -1;
                        endY = -1;
                        pullInitiated = false;

                    }

                }
                else
                {
                    state = new ScreenState(ScreenState.ScreenPosition.ARM_ELSEWHERE, DateTime.Now, DateTime.Now);
                    this._rightHandHistory.addState(state);
                    pullInitiated = false;
                }
            }

        }

    }
}
