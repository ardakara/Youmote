using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace YouMote
{
    public class PullDownIndicator : PositionIndicator
    {

        private double dotProduct(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return (x1 * x2 + y1 * y2 + z1 * z2);
        }

        private double magnitude(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        private double calculateAngle(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            double scalarProduct = dotProduct(x1, y1, z1, x2, y2, z2);
            double magnitudeA = magnitude(x1, y1, z1);
            double magnitudeB = magnitude(x2, y2, z2);

            double theta = Math.Acos(scalarProduct / (magnitudeA * magnitudeB)) * (180.0 / Math.PI);
            return theta;
        }

        private Boolean isStraightLineApprox(double degrees)
        {
            if (degrees <= 180 && degrees > 135)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean isStraightLine(double degrees)
        {
            if (degrees <= 180 && degrees > 160)
            {
                Console.WriteLine("Straight line! b/c degrees: " + degrees);
                return true;
            }
            else
            {
                Console.WriteLine("Not straight! degrees: " + degrees);
                return false;
            }
        }

        public Boolean isRightArmStraight(Skeleton skeleton, bool isLenient)
        {
            if (skeleton == null)
            {
                return false;
            }

            Joint elbowRight = skeleton.Joints[JointType.ElbowRight];
            Joint shoulderRight = skeleton.Joints[JointType.ShoulderRight];
            Joint handRight = skeleton.Joints[JointType.HandRight];

            double elbowRightToshoulderRightX = shoulderRight.Position.X - elbowRight.Position.X;
            double elbowRightToshoulderRightY = shoulderRight.Position.Y - elbowRight.Position.Y;
            double elbowRightToshoulderRightZ = shoulderRight.Position.Z - elbowRight.Position.Z;

            double elbowRightTohandRightX = handRight.Position.X - elbowRight.Position.X;
            double elbowRightTohandRightY = handRight.Position.Y - elbowRight.Position.Y;
            double elbowRightTohandRightZ = handRight.Position.Z - elbowRight.Position.Z;

            double shoulderRightTohandRight_degrees = calculateAngle(elbowRightToshoulderRightX, elbowRightToshoulderRightY, elbowRightToshoulderRightZ, elbowRightTohandRightX, elbowRightTohandRightY, elbowRightTohandRightZ);
            
            //Console.WriteLine("Degrees: " + shoulderRightTohandRight_degrees);

            if (isLenient) {
                return isStraightLineApprox(shoulderRightTohandRight_degrees);
            } else {
                return isStraightLine(shoulderRightTohandRight_degrees);
            }
        }

        public Boolean isLeftArmStraight(Skeleton skeleton, bool isLenient)
        {
            if (skeleton == null)
            {
                return false;
            }

            Joint elbowLeft = skeleton.Joints[JointType.ElbowLeft];
            Joint shoulderLeft = skeleton.Joints[JointType.ShoulderLeft];
            Joint handLeft = skeleton.Joints[JointType.HandLeft];

            double elbowLeftToshoulderLeftX = shoulderLeft.Position.X - elbowLeft.Position.X;
            double elbowLeftToshoulderLeftY = shoulderLeft.Position.Y - elbowLeft.Position.Y;
            double elbowLeftToshoulderLeftZ = shoulderLeft.Position.Z - elbowLeft.Position.Z;

            double elbowLeftTohandLeftX = handLeft.Position.X - elbowLeft.Position.X;
            double elbowLeftTohandLeftY = handLeft.Position.Y - elbowLeft.Position.Y;
            double elbowLeftTohandLeftZ = handLeft.Position.Z - elbowLeft.Position.Z;

            double shoulderLeftTohandLeft_degrees = calculateAngle(elbowLeftToshoulderLeftX, elbowLeftToshoulderLeftY, elbowLeftToshoulderLeftZ, elbowLeftTohandLeftX, elbowLeftTohandLeftY, elbowLeftTohandLeftZ);

            //Console.WriteLine("Degrees: " + shoulderRightTohandRight_degrees);

            if (isLenient)
            {
                return isStraightLineApprox(shoulderLeftTohandLeft_degrees);
            }
            else
            {
                return isStraightLine(shoulderLeftTohandLeft_degrees);
            }
        }

        public Boolean rightArmUp(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }

            Joint elbowRight = skeleton.Joints[JointType.ElbowRight];
            Joint shoulderRight = skeleton.Joints[JointType.ShoulderRight];
            Joint handRight = skeleton.Joints[JointType.HandRight];

            double elbowRightX = elbowRight.Position.X;
            double shoulderRightX = shoulderRight.Position.X;
            double handRightX = handRight.Position.X;
            double deltaX = Math.Max(Math.Abs(elbowRightX - shoulderRightX), Math.Abs(elbowRightX - handRightX));
            
            //Console.WriteLine("deltaX: " + deltaX);
            
            if (deltaX < 0.075)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean isPositionDetected(Skeleton skelton)
        {
            return false;
        }


    }
}
