using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    public class WavingIndicator : PositionIndicatorIMPL
    {
        public Boolean isPositionDetected(SkeletonData skeleton)
        {

            if (skeleton == null)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
