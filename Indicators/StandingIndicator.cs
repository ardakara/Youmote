﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    class StandingIndicator : PositionIndicatorIMPL
    {
        public Boolean isPositionDetected(SkeletonData skeleton)
        {
            Joint hipCenter = skeleton.Joints[JointID.HipCenter];
            Joint head = skeleton.Joints[JointID.Head];
            Joint leftKnee = skeleton.Joints[JointID.KneeLeft];
            Joint rightKnee = skeleton.Joints[JointID.KneeRight];

            /* Turns out that the best indicator is just that the Zs are all at the same depth. X of head, hip, and average of knees just makes it wonky */
            
            double hipX = hipCenter.Position.X;
            double headX = head.Position.X;
            double averageKneeX = (leftKnee.Position.X + rightKnee.Position.X) / 2.0;
            double deltaX = Math.Max(Math.Abs(hipX - headX), Math.Abs(headX - averageKneeX));
            
            double hipZ = hipCenter.Position.Z;
            double headZ = head.Position.Z;
            double leftKneeZ = leftKnee.Position.Z;
            double rightKneeZ = rightKnee.Position.Z;
            double deltaKneeZ = Math.Min(Math.Abs(hipZ - leftKneeZ), Math.Abs(hipZ - rightKneeZ));

            Console.WriteLine("deltaLeftKneeZ: " + deltaKneeZ);

            if ( deltaKneeZ < 0.075 && deltaX < 0.25)
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
