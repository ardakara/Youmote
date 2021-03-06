﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace YouMote
{
    public class RightHandNextToFace : PositionIndicator
    {

        private double magnitude(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        public Boolean isPositionDetected(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            Joint rightHand = skeleton.Joints[JointType.HandRight];
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            Joint head = skeleton.Joints[JointType.Head];

            double rightHandDistance = magnitude(rightHand.Position.X - head.Position.X, rightHand.Position.Y - head.Position.Y, rightHand.Position.Z - head.Position.Z);
            double leftHandDistance = magnitude(leftHand.Position.X - head.Position.X, leftHand.Position.Y - head.Position.Y, leftHand.Position.Z - head.Position.Z);

            if (rightHandDistance < 0.25 || leftHandDistance < 0.25)
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
