using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
/// <summary>
/// Detector for whether a skeleton is standing
/// </summary>
    class StandPDetector : PositionDetector
    {
        static Boolean isPositionDetected(SkeletonData skeleton)
        {
            return false;
        }
    }
}
