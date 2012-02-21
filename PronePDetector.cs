using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
/// <summary>
/// Detector for whether a skeleton is prone/lying down
/// </summary>
    class PronePDetector : PositionDetector
    {
        static Boolean isPositionDetected(SkeletonData skeleton)
        {
            return false;
        }
    }
}
