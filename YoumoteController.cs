﻿using System;
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
using Kinect.Toolbox;
using YouMote.Detectors;
using YouMote.Indicators;
using WinRectangle = System.Windows.Shapes.Rectangle;
// FOR CIRCLE Gesture:
using System.IO;
using SysPath = System.IO.Path;
using Youmote.Television;

namespace YouMote
{
    class YoumoteController : SkeletonController
    {
        private StandingDetector standingDetector = new StandingDetector();
        private SittingDetector sittingDetector = new SittingDetector();
        private HandOnFaceIndicator handOnFaceIndicator = new HandOnFaceIndicator();
        private AbsentDetector absentDetector = new AbsentDetector();
        private PermanentLeaveDetector permanentLeaveDetector = new PermanentLeaveDetector();
        private AmbidextrousWaveDetector ambiHandWaveDetector = new AmbidextrousWaveDetector();
        private AmbidextrousScreenDetector ambiScreenDetector = new AmbidextrousScreenDetector();
        private AmbidextrousWaveDetector ambiResumeDetector = new AmbidextrousWaveDetector();

        private PullDownIndicator pullDownIndicator = new PullDownIndicator();

        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();

        private TextBlock notification_text;
        private Image notification_image;
        private TextBlock notification_speaker;
        private WinRectangle notification_background_rect;

        //using the Toolkit

        SwipeGestureDetector swipeGestureRecognizer;
        readonly ColorStreamManager colorManager = new ColorStreamManager();
        readonly DepthStreamManager depthManager = new DepthStreamManager();
        readonly BarycenterHelper barycenterHelper = new BarycenterHelper();
        readonly AlgorithmicPostureDetector algorithmicPostureRecognizer = new AlgorithmicPostureDetector();


        private bool recordNextFrameForPosture;
        bool displayDepth;
        Target gesture_notifier;

        /* Adding Circle Gesture Detector stuff */
        private string circleKBPath;

        /* Stuff needed to 'turn on the tv'*/
        private WinRectangle black_screen;
        private bool IsScreenOn;
        private TextBox _debugPositionBox;
        private TextBox _debugGestureBox;

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
            this._debugPositionBox = win.DebugPositionTextBox;
            this._debugGestureBox = win.DebugGestureTextBox;
            swipeGestureRecognizer = new SwipeGestureDetector();
            swipeGestureRecognizer.OnGestureDetected += OnGestureDetected;
            //LoadCircleGestureDetector();
            IsScreenOn = false;
        }

        void OnGestureDetected(string gesture)
        {
            if (gesture == "SwipeToLeft")
            {
                this._debugGestureBox.Text = "you swiped left!";
            }
            else if (gesture == "SwipeToRight")
            {
                this._debugGestureBox.Text = "to the RIGHT you swiped!!";
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


        private void detectSittingStandingScenarios(Skeleton skeleton)
        {

            this.permanentLeaveDetector.processSkeleton(skeleton);
            this.absentDetector.processSkeleton(skeleton);
            this.standingDetector.processSkeleton(skeleton);
            this.sittingDetector.processSkeleton(skeleton);

            /*if (this.handOnFaceIndicator.isPositionDetected(skeleton))
            {
                curVid.Pause();
            }*/

            this.ambiResumeDetector.processSkeleton(skeleton);
            Boolean hasResumed = ambiResumeDetector.isScenarioDetected();
            if (hasResumed)
            {
                this._debugPositionBox.Text = "Has RESUMED";
            }

            Boolean isAbsent = absentDetector.isScenarioDetected();
            Boolean isStanding = standingDetector.isScenarioDetected();
            Boolean isSitting = sittingDetector.isScenarioDetected();
            Boolean isPermanentlyGone = permanentLeaveDetector.isScenarioDetected();

            if (isAbsent)
            {
                if (isPermanentlyGone)
                {
                    this._debugPositionBox.Text = "I'm permanently gone";
                }
                else
                {
                    this._debugPositionBox.Text = "I'm off screen";
                }

            }
            else if (isStanding)
            {
                this._debugPositionBox.Text = "I'm standing";
            }
            else if (isSitting)
            {
                this._debugPositionBox.Text = "Sitting!";
            }
            else
            {
                this._debugPositionBox.Text = "Neither!";

            }

            if (handOnFaceIndicator.isPositionDetected(skeleton))
            {
                //                t2.setTargetText("Y!");
            }
            else
            {
                //              t2.setTargetText("N!");
            }
        }

        private void detectChannelChangingScenarios(Skeleton skeleton, KinectSensor nui)
        {

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


        }
        public override void processSkeletonFrame(Skeleton skeleton, KinectSensor nui)
        {



            if (!IsScreenOn)
            {
                this.ambiHandWaveDetector.processSkeleton(skeleton);
                Boolean hasWaved = this.ambiHandWaveDetector.isScenarioDetected();
                if (hasWaved)
                {
                    this._debugPositionBox.Text = "Has waved!";
                    black_screen.Visibility = Visibility.Hidden;
                    IsScreenOn = true;
                }
            }
            else
            {

                this.ambiScreenDetector.processSkeleton(skeleton);
                Boolean hasPulledDownScreen = this.ambiScreenDetector.isScenarioDetected();
                if (hasPulledDownScreen)
                {
                    this._debugPositionBox.Text = "Has pulled down screen!";
                }

                List<Message> readyMessages = this.messageList.popReadyMessages(sw.Elapsed.TotalSeconds);
                foreach (Message message in readyMessages)
                {
                    display_message(message);
                    message.startMessageTimer();
                }


                //detectSittingStandingScenarios(skeleton, targets);
                //detectChannelChangingScenarios(skeleton, targets, nui);

                List<Message> finishedMessages = this.messageList.popFinishedMessages();
                foreach (Message message in finishedMessages)
                {
                    remove_message(message);
                    message.stopMessageTimer();
                }


            }
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {

            /* YOUR CODE HERE */
            notification_speaker.Visibility = Visibility.Hidden;
            notification_image.Visibility = Visibility.Hidden;
            notification_text.Visibility = Visibility.Hidden;
            //            gesture_notifier = targets[1];
        }

        public override void addUIElements(TextBlock not_speaker, TextBlock not_text, Image not_image, WinRectangle rect)
        {
            notification_text = not_text;
            notification_image = not_image;
            notification_speaker = not_speaker;
            notification_background_rect = rect;
        }

        public override void addBlackScreen(WinRectangle black_screen)
        {
            this.black_screen = black_screen;
        }

        public override void addVideo(MediaElement mediaElement1)
        {

        }
    }
}
