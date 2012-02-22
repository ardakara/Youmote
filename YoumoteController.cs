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
using SkeletalTracking.Indicators;
namespace SkeletalTracking
{
    class YoumoteController : SkeletonController
    {

        private StandingIndicator standingIndicator = new StandingIndicator();
        private SittingIndicator sittingIndicator = new SittingIndicator();
        private LyingdownIndicator lyingdownIndicator = new LyingdownIndicator();
        private HandOnFaceIndicator handOnFaceIndicator = new HandOnFaceIndicator();
        private AbsentIndicator absentIndicator = new AbsentIndicator();

        private PermanentLeaveDetector permanentLeaveDetector = new PermanentLeaveDetector();
        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();

        private MediaElement curVid;

        public YoumoteController(MainWindow win)
            : base(win)
        {
            // repeat for all the messages
            this.messageList.pushMessage(10,3, "Jeff", "hi", "imagefile");
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            List<Message> readyMessages = this.messageList.popReadyMessages(sw.Elapsed.TotalSeconds);
            foreach (Message message in readyMessages)
            {
                message.startMessageTimer();
                // deal with it charlton :p
            }

            List<Message> finishedMessages = this.messageList.popFinishedMessages();
            foreach (Message message in finishedMessages)
            {
                message.stopMessageTimer();
                // deal with it charlton :p
            }


            // all detector process skeleton
            this.permanentLeaveDetector.processSkeleton(skeleton);

            Target cur = targets[1];
            Target t2 = targets[2];

            Boolean isAbsent = absentIndicator.isPositionDetected(skeleton);
            if (isAbsent)
            {
                Boolean isPermanentlyGone = permanentLeaveDetector.isScenarioDetected();
                if (isPermanentlyGone)
                {
                    Console.WriteLine("I'm permanently gone");
                    cur.setTargetText("I'm permanently gone");

                }
                else
                {

                    Console.WriteLine("I'm off screen");
                    cur.setTargetText("I'm off screen");

                }
                return;
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

            if (handOnFaceIndicator.isPositionDetected(skeleton))
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
