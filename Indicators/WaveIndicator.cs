using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    public class WaveIndicator : PositionIndicator
    {
        
        public Boolean leftHandRight(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            
            // Check if the hand is on top and right of the elbow
            if ( (leftHand.Position.Y > leftElbow.Position.Y)
                && (leftHand.Position.X > leftElbow.Position.X) )
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        
        public Boolean leftHandLeft(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            
            // Check if the hand is on top and left of the elbow
            if ( (leftHand.Position.Y > leftElbow.Position.Y)
                && (leftHand.Position.X < leftElbow.Position.X) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Boolean rightHandRight(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
            Joint rightHand = skeleton.Joints[JointType.HandRight];
            
            // Check if the hand is on top and right of the elbow
            if ( (rightHand.Position.Y > rightElbow.Position.Y)
                && (rightHand.Position.X > rightElbow.Position.X) )
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        
        public Boolean rightHandLeft(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
            Joint rightHand = skeleton.Joints[JointType.HandRight];
            
            // Check if the hand is on top and left of the elbow
            if ( (rightHand.Position.Y > rightElbow.Position.Y)
                && (rightHand.Position.X < rightElbow.Position.X) )
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