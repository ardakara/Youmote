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

    public abstract class VolumeDetector : ScenarioDetector
    {

        protected static double MAX_X_ALLOWANCE = 0.1f;
        protected static double MIN_ADJUST_DISTANCE = 0.11f;
        protected static double MAX_ADJUST_DISTANCE = 0.80f;

        protected static double MAX_SWIPE_DURATION = 10000;

        protected ScenarioStateHistory _rightHandHistory;
        protected ScenarioStateHistory _leftHandHistory;
        protected PullDownIndicator _straightArmIndicator = new PullDownIndicator();
        
        protected Point3D rh_start;
        protected Point3D lh_start;

        protected Point3D rh_last;
        protected Point3D lh_last;

        protected Double rh_endY;
        protected Double lh_endY;
        
        protected Boolean rh_adjustInitiated;
        protected Boolean lh_adjustInitiated;

        protected double rh_deltaY;
        protected double lh_deltaY;

        public VolumeDetector()
        {
            this._rightHandHistory = new ScenarioStateHistory(30,0);
            this._leftHandHistory = new ScenarioStateHistory(30,0);
            
            rh_adjustInitiated = false;
            lh_adjustInitiated = false;

            rh_start = new Point3D(-1, -1, -1);
            lh_start = new Point3D(-1, -1, -1);

            rh_last = new Point3D(-1, -1, -1);
            lh_last = new Point3D(-1, -1, -1);

            rh_deltaY = 0;
            lh_deltaY = 0;
        }

        protected double calculate_midpoint(double pos1, double pos2)
        {
            return (pos1 + pos2) / 2;
        }

        protected Boolean isAboveCrossLine(double crossline_y, double joint_y)
        {
            return (joint_y > crossline_y);
        }

        protected Boolean isBelowCrossline(double crossline_y, double joint_y)
        {
            return (joint_y < crossline_y);
        }

        protected Boolean isIncreasingZ(double cur_z, double last_z)
        {
            return (cur_z >= last_z);
        }

        protected Boolean isDecreasingZ(double cur_z, double last_z)
        {
            return (cur_z <= last_z);
        }

        protected Boolean stillWithinXBounds(double hand_x, Point3D start)
        {
            //Console.WriteLine("Z Diff: " + Math.Abs(hand_z - start.Z));
            return (Math.Abs(hand_x - start.X) < MAX_X_ALLOWANCE);
            //return true;
        }

        protected void resetVars(String hand)
        {
            if (hand.Equals("right")) {
                this.rh_adjustInitiated = false;
                this.rh_start.X = -1;
                this.rh_start.Y = -1;
                this.rh_start.Z = -1;
                this.rh_last.X = -1;
                this.rh_last.Y = -1;
                this.rh_last.Z = -1;
                this.rh_deltaY = 0;
            } else {
                this.lh_adjustInitiated = false;
                this.lh_start.X = -1;
                this.lh_start.Y = -1;
                this.lh_start.Z = -1;
                this.lh_last.X = -1;
                this.lh_last.Y = -1;
                this.lh_last.Z = -1;
                this.lh_deltaY = 0;
            }

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
