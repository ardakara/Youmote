using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    public class LyingdownIndicator : PositionIndicator
    {

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

            /* Turns out that the best indicator is just that the Zs are all at the same depth. X of head, hip, and average of knees just makes it wonky */

            double hipY = hipCenter.Position.X;
            double headY = head.Position.X;
            double leftKneeY = leftKnee.Position.Y;
            double rightKneeY = rightKnee.Position.Y;

            if (Math.Abs(hipY - headY) < 0.15)
            {
                if (Math.Abs(hipY - leftKneeY) < 0.15 || Math.Abs(hipY - rightKneeY) < 0.15)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
