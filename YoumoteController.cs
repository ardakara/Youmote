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

        private StandingDetector standingDetector = new StandingDetector();
        private SittingDetector sittingDetector = new SittingDetector();
        private HandOnFaceIndicator handOnFaceIndicator = new HandOnFaceIndicator();
        private AbsentDetector absentDetector = new AbsentDetector();
        private PermanentLeaveDetector permanentLeaveDetector = new PermanentLeaveDetector();
        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();

        private MediaElement curVid;
        private TextBlock notification_text;
        private Image notification_image;
        private TextBlock notification_speaker;
        private Rectangle notification_background_rect;

        private void addMessages()
        {
            this.messageList.Clear();
            this.messageList.pushMessage(20, 3, "Jeff H.", "Bwahahahha", "heer_profile.jpg");
            this.messageList.pushMessage(10, 3, "Jeff H.", "Hahahahahahaha", "heer_profile.jpg");
            this.messageList.pushMessage(1, 3, "TV Ninja", "You're watching with Jeff H.!", "tv_logo.png");
        }

        public YoumoteController(MainWindow win)
            : base(win)
        {
            // repeat for all the messages
            addMessages();
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

            notification_text.Visibility = Visibility.Visible;
            notification_speaker.Visibility = Visibility.Visible;
            notification_image.Visibility = Visibility.Visible;
            notification_background_rect.Visibility = Visibility.Visible;
        }

        private void remove_message(Message message)
        {
            notification_text.Visibility = Visibility.Hidden;
            notification_speaker.Visibility = Visibility.Hidden;
            notification_image.Visibility = Visibility.Hidden;
            notification_background_rect.Visibility = Visibility.Hidden;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            List<Message> readyMessages = this.messageList.popReadyMessages(sw.Elapsed.TotalSeconds);
            foreach (Message message in readyMessages)
            {
                display_message(message);
                message.startMessageTimer();
            }

            List<Message> finishedMessages = this.messageList.popFinishedMessages();
            foreach (Message message in finishedMessages)
            {
                remove_message(message);
                message.stopMessageTimer();
            }


            // all detector process skeleton

            this.permanentLeaveDetector.processSkeleton(skeleton);
            this.absentDetector.processSkeleton(skeleton);
            this.standingDetector.processSkeleton(skeleton);
            this.sittingDetector.processSkeleton(skeleton);

            Target cur = targets[1];
            Target t2 = targets[2];

            Boolean isAbsent = absentDetector.isScenarioDetected();
            Boolean isStanding = standingDetector.isScenarioDetected();
            Boolean isSitting = sittingDetector.isScenarioDetected();
            Boolean isPermanentlyGone = permanentLeaveDetector.isScenarioDetected();

            if (isAbsent)
            {
                if (isPermanentlyGone)
                {
                    Console.WriteLine("I'm permanently gone");
                    cur.setTargetText("I'm permanently gone");
                    this.messageList.Clear();
                    addMessages();
                    sw.Reset();
                    curVid.Stop();
                    curVid.Position = TimeSpan.Zero;
                    curVid.Visibility = Visibility.Hidden;

                }
                else
                {

                    Console.WriteLine("I'm off screen");
                    cur.setTargetText("I'm off screen");

                }

            }
            else if (isStanding)
            {
                Console.WriteLine("I'm standing!");
                cur.setTargetText("I'm standing!");
                curVid.Pause();
                sw.Stop();

            }
            else if (isSitting)
            {
                Console.WriteLine("I'm sitting!");
                cur.setTargetText("Sitting!");
                curVid.Visibility = Visibility.Visible;
                curVid.Play();
                sw.Start();
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

        public override void addUIElements(TextBlock not_speaker, TextBlock not_text, Image not_image, Rectangle rect)
        {
            notification_text = not_text;
            notification_image = not_image;
            notification_speaker = not_speaker;
            notification_background_rect = rect;
        }

        public override void addVideo(MediaElement mediaElement1)
        {
            curVid = mediaElement1;
        }

        // put your classifier code here as functions that return bool
    }
}
