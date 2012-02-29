using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    interface PositionIndicator
    {
        /// <summary>
        /// Takes in a skeleton and returns whether that position is detected.  For Example, If this is a SittingDetector, it will return true
        /// if the skeleton has legs and abdomen at an angle that represents sitting, otherwise false.  These detectors can be used to create
        /// states that represent a ScenarioDetector.  
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        Boolean isPositionDetected(Skeleton skeleton);
    }
}
