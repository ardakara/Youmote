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
using YouMote.Television;

//debug indicator for hand on face
using YouMote.Indicators;

namespace YouMote
{
    class YoumoteController : SkeletonController
    {
        private StandingDetector standingDetector = new StandingDetector();
        private SittingDetector sittingDetector = new SittingDetector();
        private AbsentDetector absentDetector = new AbsentDetector();
        private PermanentLeaveDetector permanentLeaveDetector = new PermanentLeaveDetector();
        private AmbidextrousScreenDetector ambiScreenDetector = new AmbidextrousScreenDetector();
        private AmbidextrousResumeDetector ambiResumeDetector = new AmbidextrousResumeDetector();
        private TalkOnPhoneDetector talkOnPhoneDetector;
        private SpeechPauseOverrideDetector speechPauseOverrideDetector;
        private SpeechResumeOverrideDetector speechResumeOverrideDetector;

        private MainWindow window;

        private PullDownIndicator pullDownIndicator = new PullDownIndicator();

        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();

        /* To get channels changing */
        private AmbidextrousSwipeLeftDetector ambiSwipeLeftDetector = new AmbidextrousSwipeLeftDetector();
        private AmbidextrousSwipeRightDetector ambiSwipeRightDetector = new AmbidextrousSwipeRightDetector();
        private Stopwatch swipe_sw = new Stopwatch();

        /* To turn TV on and off */
        private AmbidextrousWaveDetector ambiHandWaveDetector = new AmbidextrousWaveDetector();
        private Stopwatch wave_sw = new Stopwatch();


        /* Flags to handle complex manual overrides */
        private Boolean _isManualResume = false;
        private Boolean _isManualPause = false;

        readonly

        //using the Toolkit
        SwipeGestureDetectorMod swipeGestureRecognizer;
        readonly ColorStreamManager colorManager = new ColorStreamManager();
        readonly DepthStreamManager depthManager = new DepthStreamManager();
        readonly BarycenterHelper barycenterHelper = new BarycenterHelper();
        readonly AlgorithmicPostureDetector algorithmicPostureRecognizer = new AlgorithmicPostureDetector();


        /* Stuff needed to 'turn on the tv'*/
        private YouMote.Television.Television _tv;
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
            this.window = win;
            // repeat for all the messages
            addMessages();
            this._debugPositionBox = win.DebugPositionTextBox;
            this._debugGestureBox = win.DebugGestureTextBox;
            swipeGestureRecognizer = new SwipeGestureDetectorMod();
            swipeGestureRecognizer.OnGestureDetected += OnGestureDetected;
            this._tv = new YouMote.Television.Television(win);
            this._isManualPause = false;
            this._isManualResume = false;
            wave_sw.Start();
            swipe_sw.Start();
            talkOnPhoneDetector = new TalkOnPhoneDetector(win);
            speechPauseOverrideDetector = new SpeechPauseOverrideDetector(win);
            speechResumeOverrideDetector = new SpeechResumeOverrideDetector(win);
        }

        void OnGestureDetected(string gesture)
        {
            if (gesture == "SwipeToLeft")
            {
                this._isManualResume = true;
                this._debugGestureBox.Text = "you swiped left!";
                this._tv.moveMediaToLeft();
            }
            else if (gesture == "SwipeToRight")
            {
                this._isManualResume = true;
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
            /*
            String hello = window.speechRecognizer.Word;
            if(hello !=null && hello.Equals("hello")){
                int a = 3;

            }
*/

            this.permanentLeaveDetector.processSkeleton(skeleton);
            this.absentDetector.processSkeleton(skeleton);
            this.standingDetector.processSkeleton(skeleton);
            this.sittingDetector.processSkeleton(skeleton);
            this.ambiResumeDetector.processSkeleton(skeleton);
            this.talkOnPhoneDetector.processSkeleton(skeleton);

            Boolean isAbsent = absentDetector.isScenarioDetected();
            Boolean isStanding = standingDetector.isScenarioDetected();
            Boolean isSitting = sittingDetector.isScenarioDetected();
            Boolean isPermanentlyGone = permanentLeaveDetector.isScenarioDetected();
            //            Boolean hasResumed = ambiResumeDetector.isScenarioDetected();
            Boolean isTalkingOnPhone = talkOnPhoneDetector.isScenarioDetected();
            Boolean isManualPause = speechPauseOverrideDetector.isScenarioDetected();
            Boolean isManualResume = speechResumeOverrideDetector.isScenarioDetected();



            if (isTalkingOnPhone)
            {
                this._debugGestureBox.Text = "talking on phone";
            }

            HandOnFaceIndicator handOnFaceIndicator = new HandOnFaceIndicator();
            if (handOnFaceIndicator.isPositionDetected(skeleton))
            {
                this._debugGestureBox.Text = "hand on face";
            }


            if (isManualPause)
            {
                this._isManualPause = isManualPause;
                this._debugPositionBox.Text = "Manual pause!";
                this._tv.pause();

            }
            else if (isManualResume)
            {
                this._isManualResume = isManualResume;
                this._debugPositionBox.Text = "manual Resume!";
                this._tv.play();
            }

            //all the detector logic
            if (isAbsent && !this._isManualResume)
            {
                if (isPermanentlyGone)
                {
                    this._tv.turnOff();
                }
                else
                {
                    this._tv.pause(ScreenController.PauseReason.LEAVE);
                }
            }
            else if (isTalkingOnPhone && !this._isManualResume)
            {
                this._isManualPause = false;
                this._debugPositionBox.Text = "I'm talking on phone and paused.";
                this._tv.pause();
            }
            else if (isStanding && !this._isManualResume)
            {
                this._isManualPause = false;
                this._debugPositionBox.Text = "I'm standing and paused.";
                ScreenController.PauseReason reason = ScreenController.PauseReason.STANDUP;
                this._tv.pause(reason);
            }
            else if (isSitting && !this._isManualPause)
            {
                this._isManualResume = false;
                this._debugPositionBox.Text = "Sitting -- so resume!";
                this._tv.play();
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
                if (ambiSwipeLeftDetector.isScenarioDetected() && (swipe_sw.ElapsedMilliseconds > 1000))
                {
                    this._debugGestureBox.Text = "Swipe left!";
                    this._tv.moveMediaToLeft();
                    swipe_sw.Restart();
                }

                ambiSwipeRightDetector.processSkeleton(skeleton);
                if (ambiSwipeRightDetector.isScenarioDetected() && (swipe_sw.ElapsedMilliseconds > 1000))
                {
                    this._debugGestureBox.Text = "Right swipe!";
                    this._tv.moveMediaToRight();
                    swipe_sw.Restart();
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

        public override void processSkeletonFrame(Skeleton skeleton, KinectSensor nui, Dictionary<int, Target> targets)
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
                if (hasWaved && wave_sw.ElapsedMilliseconds > 1500)
                {
                    this._debugGestureBox.Text = "ON-has waved";
                    // turn on the tv
                    this._tv.turnOn();
                    wave_sw.Reset();
                    wave_sw.Start();
                }
            }
            else
            {

                detectSittingStandingScenarios(skeleton);


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

                /*
                this.ambiScreenDetector.processSkeleton(skeleton);
                Boolean hasPulledDownScreen = this.ambiScreenDetector.isScenarioDetected();
                if (hasPulledDownScreen)
                {
                    this._debugGestureBox.Text = "Has pulled down screen!";
                    this._tv.turnOff();
                }*/

                this.ambiHandWaveDetector.processSkeleton(skeleton);
                Boolean hasWaved = this.ambiHandWaveDetector.isScenarioDetected();
                if (hasWaved && wave_sw.ElapsedMilliseconds > 1500)
                {
                    this._debugGestureBox.Text = "OFF-has waved";
                    this._tv.turnOff();
                    wave_sw.Reset();
                    wave_sw.Start();
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
