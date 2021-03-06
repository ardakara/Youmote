﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace YouMote
{
    public class SittingIndicator : PositionIndicator
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

        private Boolean isRecliningAngle(double degrees)
        {
            if (degrees > 30 && degrees < 145)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean isPositionDetected(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }

            Joint hipCenter = skeleton.Joints[JointType.HipCenter];
            Joint head = skeleton.Joints[JointType.Head];
            Joint leftKnee = skeleton.Joints[JointType.KneeLeft];
            Joint rightKnee = skeleton.Joints[JointType.KneeRight];

            double hipToHeadX = head.Position.X - hipCenter.Position.X;
            double hipToHeadY = head.Position.Y - hipCenter.Position.Y;
            double hipToHeadZ = head.Position.Z - hipCenter.Position.Z;

            double hipToLeftKneeX = leftKnee.Position.X - hipCenter.Position.X;
            double hipToLeftKneeY = leftKnee.Position.Y - hipCenter.Position.Y;
            double hipToLeftKneeZ = leftKnee.Position.Z - hipCenter.Position.Z;

            double hipToRightKneeX = rightKnee.Position.X - hipCenter.Position.X;
            double hipToRightKneeY = rightKnee.Position.Y - hipCenter.Position.Y;
            double hipToRightKneeZ = rightKnee.Position.Z - hipCenter.Position.Z;

            double headToLeftKnee_degrees = calculateAngle(hipToHeadX, hipToHeadY, hipToHeadZ, hipToLeftKneeX, hipToLeftKneeY, hipToLeftKneeZ);
            double headToRightKnee_degrees = calculateAngle(hipToHeadX, hipToHeadY, hipToHeadZ, hipToRightKneeX, hipToRightKneeY, hipToRightKneeZ);

            if (isRecliningAngle(headToLeftKnee_degrees) || isRecliningAngle(headToRightKnee_degrees))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
