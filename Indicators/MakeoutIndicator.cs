using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace YouMote
{
    public class MakeoutIndicator : PositionIndicator
    {

        private double magnitude(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        public Boolean isMakeoutDetected(Skeleton skel1, Skeleton skel2)
        {
            if (skel1 != null && skel2 != null)
            {
                Joint head1 = skel1.Joints[JointType.Head];
                Joint head2 = skel2.Joints[JointType.Head];

                double headDistance = magnitude(head1.Position.X - head2.Position.X, head1.Position.Y - head2.Position.Y, head1.Position.Z - head2.Position.Z);

                if (headDistance < 0.25)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
