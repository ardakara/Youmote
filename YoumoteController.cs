using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
        private HandOnFaceIndicator handOnFaceIndicator  = new HandOnFaceIndicator();
        private AbsentIndicator absentIndicator = new AbsentIndicator();

        private PermanentLeaveDetector permanentLeaveDetector  = new PermanentLeaveDetector();
        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();

        private MediaElement curVid;
        private TextBlock notification_text;
        private Image notification_image;
        private TextBlock notification_speaker;

        public YoumoteController(MainWindow win)
            : base(win)
        {
            // repeat for all the messages
            this.messageList.pushMessage(10, "TV Ninja", "You're watching a Pixar short with Jeff!", "tv_logo.png");
            this.messageList.pushMessage(20, "Jeff H.", "Hahahahahahaha", "heer_profile.jpg");
            this.messageList.pushMessage(50, "Jeff H.", "Bwahahahha", "heer_profile.jpg");
        }

        private void change_speaker_photo(String image_name)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,/Images/" + image_name);
            img.EndInit();
            notification_image.Source = img;
        }

        private void display_message(Message message)
        {
            change_speaker_photo(message.imgFile);
            notification_text.Text = message.text;
            notification_speaker.Text = message.speaker;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            List<Message> overdueMessages = this.messageList.removeOverdueMessages(sw.Elapsed.TotalSeconds);
            foreach (Message message in overdueMessages)
            {
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

                notification_image.Visibility = Visibility.Visible;
                notification_text.Visibility = Visibility.Visible;
                
            }
            else if (sittingIndicator.isPositionDetected(skeleton))
            {
                Console.WriteLine("I'm sitting!");
                cur.setTargetText("Sitting!");
                curVid.Play();
                sw.Start();
                notification_image.Visibility = Visibility.Visible;
                notification_text.Visibility = Visibility.Visible;
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
            notification_speaker.Visibility = Visibility.Hidden;
            notification_image.Visibility = Visibility.Hidden;
            notification_text.Visibility = Visibility.Hidden;
        }

        public override void addUIElements(TextBlock not_speaker, TextBlock not_text, Image not_image)
        {
            notification_text = not_text;
            notification_image = not_image;
            notification_speaker = not_speaker;
        }

        public override void addVideo(MediaElement mediaElement1)
        {
            curVid = mediaElement1;
        }

        // put your classifier code here as functions that return bool
    }
}
