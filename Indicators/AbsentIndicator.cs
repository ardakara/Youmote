using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking.Indicators
{
    public class AbsentIndicator
    {

        public Boolean isPositionDetected(SkeletonData skeleton)
        {
            if (skeleton == null)
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
