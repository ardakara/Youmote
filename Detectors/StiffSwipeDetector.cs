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

    public abstract class StiffSwipeDetector : ScenarioDetector
    {

        protected static double MAX_Z_ALLOWANCE = 0.15f;
        protected static double MAX_Y_ALLOWANCE = 0.1f;
        protected static double MIN_SWIPE_DISTANCE = 0.30f;
        protected static double MAX_SWIPE_DISTANCE = 0.70f;

        protected static double MAX_SWIPE_DURATION = 1000;
        
        protected static double MAX_SWIPE_FINISH_DURATION = 60;

        protected ScenarioStateHistory _rightHandHistory;
        protected ScenarioStateHistory _leftHandHistory;
        protected PullDownIndicator _straightArmIndicator = new PullDownIndicator();
        
        protected Point3D rh_start;
        protected Point3D lh_start;

        protected Point3D rh_last;
        protected Point3D lh_last;

        protected Double rh_endX;
        protected Double lh_endX;
        
        protected Boolean rh_swipeInitiated;
        protected Boolean lh_swipeInitiated;

        protected double rh_angle;
        protected double lh_angle;


        public StiffSwipeDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30);
            this._leftHandHistory = new ScenarioStateHistory(30);
            
            rh_swipeInitiated = false;
            lh_swipeInitiated = false;

            rh_start = new Point3D(-1, -1, -1);
            lh_start = new Point3D(-1, -1, -1);

            rh_last = new Point3D(-1, -1, -1);
            lh_last = new Point3D(-1, -1, -1);

            rh_endX = -1;
            lh_endX = -1;
        }

        protected double calculate_midpoint(double pos1, double pos2)
        {
            return (pos1 + pos2) / 2;
        }

        protected Boolean isLeftOfCrossLine(double crossline_x, double joint_x)
        {
            return (joint_x < crossline_x);
        }

        protected Boolean isRightOfCrossline(double crossline_x, double joint_y)
        {
            return (joint_y > crossline_x);
        }

        protected Boolean isIncreasingZ(double cur_z, double last_z)
        {
            return (cur_z >= last_z);
        }

        protected Boolean isDecreasingZ(double cur_z, double last_z)
        {
            return (cur_z <= last_z);
        }

        protected Boolean stillWithinZBounds(double hand_z, Point3D start)
        {
            //Console.WriteLine("Z Diff: " + Math.Abs(hand_z - start.Z));
            return (Math.Abs(hand_z - start.Z) < MAX_Z_ALLOWANCE);
            //return true;
        }

        protected Boolean stillWithinYBounds(double hand_y, Point3D start)
        {
            //return true;
            //Console.WriteLine("Y Diff: " + Math.Abs(hand_y - start.Y));
            return (Math.Abs(hand_y - start.Y) < MAX_Y_ALLOWANCE);
        }

        protected void resetVars(String hand)
        {
            if (hand.Equals("right")) {
                this.rh_swipeInitiated = false;
                this.rh_start.X = -1;
                this.rh_start.Y = -1;
                this.rh_start.Z = -1;
                this.rh_last.X = -1;
                this.rh_last.Y = -1;
                this.rh_last.Z = -1;
                this.rh_endX = -1;
            } else {
                this.lh_swipeInitiated = false;
                this.lh_start.X = -1;
                this.lh_start.Y = -1;
                this.lh_start.Z = -1;
                this.lh_last.X = -1;
                this.lh_last.Y = -1;
                this.lh_last.Z = -1;
                this.lh_endX = -1;
            }

            //initial_theta = -1;
        }

        protected double dotProduct(double x1, double y1, double z1, double x2, double y2, double z2)
        {
       
            return (x1 * x2 + y1 * y2 + z1 * z2);
        }

        protected double magnitude(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        protected double calculateAngle(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            double scalarProduct = dotProduct(x1, y1, z1, x2, y2, z2);
            double magnitudeA = magnitude(x1, y1, z1);
            double magnitudeB = magnitude(x2, y2, z2);

            double theta = Math.Acos(scalarProduct / (magnitudeA * magnitudeB)) * (180.0 / Math.PI);
            return theta;
        }

        public abstract Boolean isScenarioDetected();

        public abstract void processSkeleton(Skeleton skeleton);
    }
}
