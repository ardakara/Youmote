using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    class AbsentDetector
    {


        public Boolean isPositionDetected(SkeletonData skeleton)
        {
            return skeleton == null;
        }
    }
}
