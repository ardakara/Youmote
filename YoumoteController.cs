using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Diagnostics;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using SkeletalTracking.Detectors;

namespace SkeletalTracking
{
    class YoumoteController : SkeletonController
    {

        private StandingIndicator standingIndicator;
        private SittingIndicator sittingIndicator;
        private LyingdownIndicator lyingdownIndicator;
        private HandOnFaceIndicator onthephoneIndicator;
        private PresenceDetector getsUpAndLeavesDetector;
        private PermanentLeaveDetector permanentLeaveDetector;

        private Stopwatch sw;

        private MediaElement curVid;

        public YoumoteController(MainWindow win) : base(win)
        {
            standingIndicator = new StandingIndicator();
            sittingIndicator = new SittingIndicator();
            lyingdownIndicator = new LyingdownIndicator();
            onthephoneIndicator = new HandOnFaceIndicator();
//            getsUpAndLeavesDetector = new PresenceDetector();
            sw = new Stopwatch();
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            this.permanentLeaveDetector.processSkeleton(skeleton);



            Target cur = targets[1];
            Target t2 = targets[2];

            if (permanentLeaveDetector.isScenarioDetected())
            {
                Console.WriteLine("I'm permanently gone");
                cur.setTargetText("I'm permanently gone");
            }

            if (sw.Elapsed.TotalSeconds == 5)
            {
                Console.WriteLine("5 seconds have passed!");
            }

            if (standingIndicator.isPositionDetected(skeleton))
            {
                Console.WriteLine("I'm standing!");
                cur.setTargetText("I'm standing!");
                curVid.Pause();
                sw.Stop();
            }
            else if (sittingIndicator.isPositionDetected(skeleton))
            {
                Console.WriteLine("I'm sitting!");
                cur.setTargetText("Sitting!");
                curVid.Play();
                sw.Start();
            }
            else if (lyingdownIndicator.isPositionDetected(skeleton))
            {
                Console.WriteLine("Lying down!");
                cur.setTargetText("Lying down!");
            }
            else
            {
                Console.WriteLine("Neither sitting nor standing!");
                cur.setTargetText("Neither!");
            }

            if (onthephoneIndicator.isPositionDetected(skeleton))
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
