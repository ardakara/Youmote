﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
namespace YouMote.Detectors
{

   class LeftArmVolumeSwipeDetector : VolumeSwipeDetector
    {
        protected override double LEFT_SWIPE_ZONE_ANGLE { get { return 110; } }
        protected override double RIGHT_SWIPE_ZONE_ANGLE { get { return 70; } }

        public LeftArmVolumeSwipeDetector()
            : base()
        {

        }

        /// <summary>
        /// Returns the Point3D for the hand location.
        /// </summary>
        /// <returns></returns>
        protected override Point3D getHandLocation()
        {
            return new Point3D(this.Skeleton.Joints[JointType.HandLeft]);
        }


        /// <summary>
        /// Returns the Point3D for the elbow location.
        /// </summary>
        /// <returns></returns>
        protected override Point3D getElbowLocation()
        {
            return new Point3D(this.Skeleton.Joints[JointType.ElbowLeft]);
        }

        /// <summary>
        /// Returns the Point3D for the shoulder location.
        /// </summary>
        /// <returns></returns>
        protected override Point3D getShoulderLocation()
        {
            return new Point3D(this.Skeleton.Joints[JointType.ShoulderLeft]);
        }
    }
}
