using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    public class WaveIndicator : PositionIndicatorIMPL
    {
        
        public Boolean leftHandRight(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return false;
            }
            
            Joint leftElbow = skeleton.Joints[JointID.ElbowLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            
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
            
            Joint leftElbow = skeleton.Joints[JointID.ElbowLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            
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
            
            Joint rightElbow = skeleton.Joints[JointID.ElbowRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            
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
            
            Joint rightElbow = skeleton.Joints[JointID.ElbowRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            
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
    }
}