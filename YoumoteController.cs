using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    class YoumoteController : SkeletonController
    {

        private StandingDetector standingDetector;
        private SittingDetector sittingDetector;
        private LyingdownDetector lyingdownDetector;
        private OnthephoneDetector onthephoneDetector;
        private GetsUpAndLeavesDetector getsUpAndLeavesDetector = new GetsUpAndLeavesDetector();

        private MediaElement curVid;

        public YoumoteController(MainWindow win) : base(win)
        {
            standingDetector = new StandingDetector();
            sittingDetector = new SittingDetector();
            lyingdownDetector = new LyingdownDetector();
            onthephoneDetector = new OnthephoneDetector();
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            this.getsUpAndLeavesDetector.processSkeleton(skeleton);
            this.getsUpAndLeavesDetector.isScenarioDetected();


            Target cur = targets[1];
            Target t2 = targets[2];
            /*
            if (getsUpAndLeavesDetector.isScenarioDetected())
            {
                Console.WriteLine("I'm GETTING UP AND LEAVING");
                cur.setTargetText("I'm GETTING UP AND LEAVING");
            }*/

            
            if (standingDetector.isPositionDetected(skeleton))
            {
                Console.WriteLine("I'm standing!");
                cur.setTargetText("I'm standing!");
                curVid.Pause();
            }
            else if (sittingDetector.isPositionDetected(skeleton))
            {
                Console.WriteLine("I'm sitting!");
                cur.setTargetText("Sitting!");
                curVid.Play(); 
            }
            else if (lyingdownDetector.isPositionDetected(skeleton))
            {
                Console.WriteLine("Lying down!");
                cur.setTargetText("Lying down!");
            }
            else
            {
                Console.WriteLine("Neither sitting nor standing!");
                cur.setTargetText("Neither!");
            }

            if (onthephoneDetector.isPositionDetected(skeleton))
            {
                Console.WriteLine("on the phone! \n");
                t2.setTargetText("Y!");
            }
            else
            {
                Console.WriteLine("not on the phone!");
                t2.setTargetText("N!");
            }
            
            /* we'll call them here */

        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {


            /* YOUR CODE HERE */

        }

        public override void addVideo(MediaElement mediaElement1)
        {
            curVid = mediaElement1;
        }

        // put your classifier code here as functions that return bool
    }
}
