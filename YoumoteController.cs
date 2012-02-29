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
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Detectors;
using YouMote.Indicators;
//using Kinect.Toolbox;
using WinRectangle = System.Windows.Shapes.Rectangle;
// FOR CIRCLE Gesture:
using System.IO;
using SysPath = System.IO.Path;
using YouMote.Television;

namespace YouMote
{
    class YoumoteController : SkeletonController
    {
        private StandingDetector standingDetector = new StandingDetector();
        private SittingDetector sittingDetector = new SittingDetector();
        private HandOnFaceIndicator handOnFaceIndicator = new HandOnFaceIndicator();
        private AbsentDetector absentDetector = new AbsentDetector();
        private PermanentLeaveDetector permanentLeaveDetector = new PermanentLeaveDetector();
        //        private RightHandWaveDetector rightHandWaveDetector = new RightHandWaveDetector();

        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();

        private TextBlock notification_text;
        private Image notification_image;
        private TextBlock notification_speaker;
        private WinRectangle notification_background_rect;

        //using the Toolkit
        /*
                SwipeGestureDetector swipeGestureRecognizer;
                readonly ColorStreamManager colorManager = new ColorStreamManager();
                readonly DepthStreamManager depthManager = new DepthStreamManager();
                readonly BarycenterHelper barycenterHelper = new BarycenterHelper();
                readonly AlgorithmicPostureDetector algorithmicPostureRecognizer = new AlgorithmicPostureDetector();

        */
        //TemplatedPostureDetector templatePostureDetector = new TemplatedPostureDetector();
        private bool recordNextFrameForPosture;
        bool displayDepth;
        Target gesture_notifier;

        /* Adding Circle Gesture Detector stuff */
        private string circleKBPath;
        /*
                TemplatedGestureDetector circleGestureRecognizer;
                TemplatedGestureDetector waveGestureRecognizer;
                TemplatedGestureDetector resumeGestureRecognizer;
                */
        void LoadCircleGestureDetector()
        {
            /*  circleKBPath = SysPath.Combine(Environment.CurrentDirectory, @"data\circleKB.save");
              Console.WriteLine(Environment.CurrentDirectory);
              Console.WriteLine(circleKBPath);
              using (Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate))
              {
                  circleGestureRecognizer = new TemplatedGestureDetector("Circle", recordStream);
                  circleGestureRecognizer.OnGestureDetected += OnGestureDetected;
              }
             */
            //templates.ItemsSource = circleGestureRecognizer.LearningMachine.Paths;
        }

        private ScreenController _screenController;

        /* END CIRCLE DETECTOR INITIALIZATION */

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
            
            /*
                        swipeGestureRecognizer = new SwipeGestureDetector();
                        swipeGestureRecognizer.OnGestureDetected += OnGestureDetected;
                        LoadCircleGestureDetector();
            */
        }

        void OnGestureDetected(string gesture)
        {
            if (gesture == "SwipeToLeft")
            {
                Console.WriteLine("You swiped to the left!");
                gesture_notifier.setTargetText("You swiped to the left!");
            }
            else if (gesture == "SwipeToRight")
            {
                Console.WriteLine("To the RIGHT you swiped!");
                gesture_notifier.setTargetText("to the RIGHT you swiped!!");
            }
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


        private void detectSittingStandingScenarios(Skeleton skeleton, Dictionary<int, Target> targets)
        {
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
                    cur.setTargetText("I'm permanently gone");
                }
                else
                {
                    cur.setTargetText("I'm off screen");

                }

            }
            else if (isStanding)
            {
                cur.setTargetText("I'm standing!");


            }
            else if (isSitting)
            {
                cur.setTargetText("Sitting!");

            }
            else
            {
                cur.setTargetText("Neither!");
            }

            if (handOnFaceIndicator.isPositionDetected(skeleton))
            {
                t2.setTargetText("Y!");
            }
            else
            {
                t2.setTargetText("N!");
            }
        }

        private void detectChannelChangingScenarios(Skeleton skeleton, Dictionary<int, Target> targets, KinectSensor nui)
        {
            /*
                        if (skeleton != null)
                        {
                            barycenterHelper.Add(skeleton.Position.ToVector3(), skeleton.TrackingId);
                            if (!barycenterHelper.IsStable(skeleton.TrackingId))
                                return;

                            foreach (Joint joint in skeleton.Joints)
                            {
                                if (joint.TrackingState != JointTrackingState.Tracked)
                                    continue;

                                if (joint.JointType == JointType.HandRight)
                                {
                                    swipeGestureRecognizer.Add(joint.Position, nui);
                                    //circleGestureRecognizer.Add(joint.Position, nui);
                                }
                            }

                            algorithmicPostureRecognizer.TrackPostures(skeleton);
                        }
                        //templatePostureDetector.TrackPostures(skeleton);

                        if (recordNextFrameForPosture)
                        {
                            //templatePostureDetector.AddTemplate(skeleton);
                            recordNextFrameForPosture = false;
                        }
                        */
        }
        public override void processSkeletonFrame(Skeleton skeleton, KinectSensor nui, Dictionary<int, Target> targets)
        {

            

            //detectSittingStandingScenarios(skeleton, targets);
            //detectChannelChangingScenarios(skeleton, targets, nui);

            Target cur = targets[1];
            Target t2 = targets[2];
            /*
            this.rightHandWaveDetector.processSkeleton(skeleton);
            Boolean hasWaved = rightHandWaveDetector.isScenarioDetected();

            if (hasWaved)
            {
                cur.setTargetText("Has waved!");
            }
            */


        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {

            /* YOUR CODE HERE */
            notification_speaker.Visibility = Visibility.Hidden;
            notification_image.Visibility = Visibility.Hidden;
            notification_text.Visibility = Visibility.Hidden;
            gesture_notifier = targets[1];
        }

        public override void addUIElements(TextBlock not_speaker, TextBlock not_text, Image not_image, WinRectangle rect)
        {
            notification_text = not_text;
            notification_image = not_image;
            notification_speaker = not_speaker;
            notification_background_rect = rect;
        }

        public override void addVideo(MediaElement mediaElement1)
        {

        }
    }
}
