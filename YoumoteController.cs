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
        private AmbidextrousSwipeLeftDetector ambiSwipeLeftDetector = new AmbidextrousSwipeLeftDetector();
        private AmbidextrousSwipeRightDetector ambiSwipeRightDetector = new AmbidextrousSwipeRightDetector();
        private PullDownIndicator pullDownIndicator = new PullDownIndicator();

        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();



        /* Flags to handle complex manual overrides */
        private Boolean _isOverrideResume;
        private Boolean _isOverridePause;

        //using the Toolkit
        SwipeGestureDetector swipeGestureRecognizer;
        readonly ColorStreamManager colorManager = new ColorStreamManager();
        readonly DepthStreamManager depthManager = new DepthStreamManager();
        readonly BarycenterHelper barycenterHelper = new BarycenterHelper();
        readonly AlgorithmicPostureDetector algorithmicPostureRecognizer = new AlgorithmicPostureDetector();


        /* Stuff needed to 'turn on the tv'*/
        private Television _tv;
        private TextBox _debugPositionBox;
        private TextBox _debugGestureBox;

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
            this._tv = new Television(win);
            this._isOverridePause = false;
            this._isOverrideResume = false;
        }

        void OnGestureDetected(string gesture)
        {
            if (gesture == "SwipeToLeft")
            {
                this._isOverrideResume = true;
                this._debugGestureBox.Text = "you swiped left!";
                this._tv.moveMediaToLeft();
            }
            else if (gesture == "SwipeToRight")
            {
                this._isOverrideResume = true;
                this._debugGestureBox.Text = "to the RIGHT you swiped!!";
                this._tv.moveMediaToRight();
            }
        }

        private void change_speaker_photo(String image_name)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,/Images/" + image_name);
            img.EndInit();
        }

        private void display_message(Message message)
        {
            change_speaker_photo(message.imgFile);
        }

        private void remove_message(Message message)
        {

        }


        private void detectSittingStandingScenarios(Skeleton skeleton)
        {

            this.permanentLeaveDetector.processSkeleton(skeleton);
            this.absentDetector.processSkeleton(skeleton);
            this.standingDetector.processSkeleton(skeleton);
            this.sittingDetector.processSkeleton(skeleton);
            this.ambiResumeDetector.processSkeleton(skeleton);

            Boolean isAbsent = absentDetector.isScenarioDetected();
            Boolean isStanding = standingDetector.isScenarioDetected();
            Boolean isSitting = sittingDetector.isScenarioDetected();
            Boolean isPermanentlyGone = permanentLeaveDetector.isScenarioDetected();
            Boolean hasResumed = ambiResumeDetector.isScenarioDetected();

            if (hasResumed)
            {
                this._debugPositionBox.Text = "Has RESUMED";
                if (this._isOverridePause)
                {
                    this._isOverridePause = false;
                }
                else
                {
                    this._isOverrideResume = true;
                }
            }


            if (isAbsent)
            {
                if (isPermanentlyGone)
                {
                    this._debugPositionBox.Text = "I'm permanently gone";
                    this._tv.turnOff();
                }
                else
                {
                    if (!this._isOverrideResume)
                    {
                        this._debugPositionBox.Text = "I'm off screen so pause TV";
                        this._tv.pause();
                    }
                    else
                    {
                        this._debugPositionBox.Text = "I'm off screen but override resume keeps playing show!";
                        this._tv.play();
                    }
                }

            }
            else if (isStanding)
            {
                if (!this._isOverrideResume)
                {
                    this._debugPositionBox.Text = "I'm standing and paused.";
                    this._tv.pause();
                }
                else
                {
                    this._debugPositionBox.Text = "I'm standing but didn't pause b/c override resume";
                }
            }
            else if (isSitting)
            {
                this._isOverrideResume = false;
                if (!this._isOverridePause)
                {
                    this._debugPositionBox.Text = "Sitting -- so resume!";
                    this._tv.play();
                }
                else
                {
                    this._debugPositionBox.Text = "Sitting, but manual override presents play!";
                }
            }
            else
            {
                this._debugPositionBox.Text = "Neither!";
            }

        }

        private void detectChannelChangingScenarios(Skeleton skeleton, KinectSensor nui)
        {

            if (skeleton != null)
            {
                ambiSwipeLeftDetector.processSkeleton(skeleton);
                if (ambiSwipeLeftDetector.isScenarioDetected())
                {
                    this._debugGestureBox.Text = "Swipe left!";
                }

                ambiSwipeRightDetector.processSkeleton(skeleton);
                if (ambiSwipeRightDetector.isScenarioDetected())
                {
                    this._debugGestureBox.Text = "Right swipe!";
                }
                /*barycenterHelper.Add(skeleton.Position.ToVector3(), skeleton.TrackingId);
                if (!barycenterHelper.IsStable(skeleton.TrackingId))
                    return;

                foreach (Joint joint in skeleton.Joints)
                {
                    if (joint.TrackingState != JointTrackingState.Tracked)
                        continue;

                    if (joint.JointType == JointType.HandRight)
                    {
                        swipeGestureRecognizer.Add(joint.Position, nui);
                    }
                }

                algorithmicPostureRecognizer.TrackPostures(skeleton);*/
            }
        }

        public override void processSkeletonFrame(Skeleton skeleton, KinectSensor nui, Dictionary<int,Target> targets)
        {
            if (!this._tv.IsOn)
            {
                if (skeleton == null)
                {
                    this._debugPositionBox.Text = "null skel";
                }
                else
                {
                    this._debugPositionBox.Text = "recognized skel";
                }
                this.ambiHandWaveDetector.processSkeleton(skeleton);
                Boolean hasWaved = this.ambiHandWaveDetector.isScenarioDetected();
                if (hasWaved)
                {
                    this._debugGestureBox.Text = "Has waved!";
                    // turn on the tv
                    this._tv.turnOn();
                    Console.WriteLine("Has waved");
                }
            }
            else
            {

                //detectSittingStandingScenarios(skeleton);
                detectChannelChangingScenarios(skeleton, nui);

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

                this.ambiScreenDetector.processSkeleton(skeleton);
                Boolean hasPulledDownScreen = this.ambiScreenDetector.isScenarioDetected();
                if (hasPulledDownScreen)
                {
                    this._debugGestureBox.Text = "Has pulled down screen!";
                    this._tv.turnOff();
                }

 
            }
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {

//            this._tv.fakeTVRun();
        }

        public override void addUIElements(TextBlock not_speaker, TextBlock not_text, Image not_image, WinRectangle rect)
        {

        }

        public override void addVideo(MediaElement mediaElement1)
        {

        }
    }
}
