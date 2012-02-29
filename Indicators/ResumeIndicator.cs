using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    public class ResumeIndicator : PositionIndicatorIMPL
    {
        
        public Boolean leftHandFront(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            
            // Check if the hand is on top and right of the elbow
            if ( (leftHand.Position.X > leftElbow.Position.X)
                && (leftHand.Position.Z > leftElbow.Position.Z) )
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        
        public Boolean leftHandBack(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            
            // Check if the hand is on top and left of the elbow
            if ( (leftHand.Position.X > leftElbow.Position.X)
                && (leftHand.Position.Z <= leftElbow.Position.Z) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Boolean rightHandFront(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
            Joint rightHand = skeleton.Joints[JointType.HandRight];
            
            // Check if the hand is on top and right of the elbow
            if ((rightHand.Position.X < rightElbow.Position.X)
                && (rightHand.Position.Z > rightElbow.Position.Z))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        
        public Boolean rightHandBack(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
            Joint rightHand = skeleton.Joints[JointType.HandRight];
            
            // Check if the hand is on top and left of the elbow
            if ((rightHand.Position.X < rightElbow.Position.X)
                && (rightHand.Position.Z <= rightElbow.Position.Z))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public Boolean isPositionDetected(Skeleton skeleton)
        {
            return false;
        }

    }
}