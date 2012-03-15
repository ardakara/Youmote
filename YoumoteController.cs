using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
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
using YouMote.States;

namespace YouMote
{
    class YoumoteController : SkeletonController
    {
        private StandingDetector standingDetector = new StandingDetector();
        private SittingDetector sittingDetector = new SittingDetector();
        private AbsentDetector absentDetector = new AbsentDetector();
        private PermanentLeaveDetector permanentLeaveDetector = new PermanentLeaveDetector();

        private TalkOnPhoneDetector talkOnPhoneDetector;
        private SpeechPauseOverrideDetector speechPauseOverrideDetector;
        private SpeechResumeOverrideDetector speechResumeOverrideDetector;
        private SpeechOnOverrideDetector speechStartOverrideDetector;
        private SpeechOffOverrideDetector speechOffOverrideDetector;
        private SpeechHelpOverrideDetector speechHelpOverrideDetector;
        private SpeechExitHelpDetector speechExitHelpDetector;
        private RightArmSwipeDetector rSwipeDetector = new RightArmSwipeDetector();
        private LeftArmSwipeDetector lSwipeDetector = new LeftArmSwipeDetector();

        private MainWindow window;

        private PullDownIndicator pullDownIndicator = new PullDownIndicator();

        private MessageList messageList = new MessageList();
        private Stopwatch sw = new Stopwatch();

        /* To get channels changing */
        private Stopwatch swipe_sw = new Stopwatch();

        /* To turn TV on and off */
        private AmbidextrousWaveDetector ambiHandWaveDetector = new AmbidextrousWaveDetector();
        private Stopwatch wave_sw = new Stopwatch();


        /* Flags to handle complex manual overrides */
        private Boolean _isManualResume = false;
        private Boolean _isManualPause = false;

        private Boolean _keyboardResume = false;
        private Boolean _keyboardPause = false;

        private Boolean _isHelpMenu = false;


        /*Handling volume*/
        private VolumeDecreaseDetector volumeDetector = new VolumeDecreaseDetector();

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

            //this._debugGestureBox.Visibility = Visibility.Hidden;
            //this._debugPositionBox.Visibility = Visibility.Hidden;
            //swipeGestureRecognizer = new SwipeGestureDetectorMod();
            //swipeGestureRecognizer.OnGestureDetected += OnGestureDetected;
            this._tv = new YouMote.Television.Television(win);
            this._isManualPause = false;
            this._isManualResume = false;
            wave_sw.Start();
            swipe_sw.Start();
            talkOnPhoneDetector = new TalkOnPhoneDetector(win);
            speechPauseOverrideDetector = new SpeechPauseOverrideDetector(win);
            speechResumeOverrideDetector = new SpeechResumeOverrideDetector(win);
            speechStartOverrideDetector = new SpeechOnOverrideDetector(win);
            speechOffOverrideDetector = new SpeechOffOverrideDetector(win);
            speechHelpOverrideDetector = new SpeechHelpOverrideDetector(win);
            speechExitHelpDetector = new SpeechExitHelpDetector(win);
            setUpHelpVideos();
        }

        void setUpHelpVideos()
        {
            String currentPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            window.Help1.Source = new Uri(currentPath + "\\help-tv-on.mp4");
            window.Help2.Source = new Uri(currentPath + "\\help-swipe-gesture.mp4");
            window.Help3.Source = new Uri(currentPath + "\\help-tv-pause.mp4");
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

        public void processKeys(Key key)
        {
            if (key == Key.A)   //pause
            {
                this._debugPositionBox.Text = "key = A";
                manualPauseResume(true);
            }
            else if (key == Key.Z)  //play
            {
                this._debugPositionBox.Text = "key = Z";
                if (!this._tv.IsOn)
                {
                    this._tv.turnOn();
                }
                else
                {
                    manualPauseResume(false);
                }
            }
            else if (key == Key.X)
            {
                this._debugPositionBox.Text = "key = X";
                this._tv.turnOff();
            }

        }

        private void manualPauseResume(Boolean pause)
        {
            this._isManualPause = pause;
            this._isManualResume = !pause;
            this._keyboardPause = pause;
            this._keyboardResume = !pause;
            if (pause)
            {
                this._debugPositionBox.Text = "Manual pause!";
                this._tv.pause(ScreenController.PauseReason.SPEECH);
            }
            else
            {
                this._debugPositionBox.Text = "Manual Resume!";
                this._tv.play();
            }
        }

        public void showHelp()
        {
            this._isHelpMenu = true;
            window.DebugPositionTextBox.Text = "HELP MODE";
            if (!(this._tv.Channels != null || this._tv.Channels.Count==0))
            {
                this._tv.pause();
            }
            window.HelpScreen.Visibility = Visibility.Visible;
            window.Help1.Play();
            window.Help2.Play();
            window.Help3.Play();
            //play the help video
        }

        public void hideHelp()
        {
            this._isHelpMenu = false;
            if (!(this._tv.Channels != null || this._tv.Channels.Count == 0))
            {
                //pause help video
                this._tv.play();
            }
            window.DebugPositionTextBox.Text = "HELP EXIT";
            window.HelpScreen.Visibility = Visibility.Hidden;
            window.Help1.Pause();
            window.Help2.Pause();
            window.Help3.Pause();
        }

        public Boolean isHelpMenu
        {
            get
            {
                return this._isHelpMenu;
            }
        }



        private void detectSittingStandingScenarios(Skeleton skeleton)
        {

            this.permanentLeaveDetector.processSkeleton(skeleton);
            this.absentDetector.processSkeleton(skeleton);
            this.standingDetector.processSkeleton(skeleton);
            this.sittingDetector.processSkeleton(skeleton);
            //            this.ambiResumeDetector.processSkeleton(skeleton);
            this.talkOnPhoneDetector.processSkeleton(skeleton);

            Boolean isAbsent = absentDetector.isScenarioDetected();
            Boolean isStanding = standingDetector.isScenarioDetected();
            Boolean isSitting = sittingDetector.isScenarioDetected();
            Boolean isPermanentlyGone = permanentLeaveDetector.isScenarioDetected();

            Boolean isTalkingOnPhone = talkOnPhoneDetector.isScenarioDetected();
            Boolean isManualPause = speechPauseOverrideDetector.isScenarioDetected() || this._keyboardPause;
            Boolean isManualResume = speechResumeOverrideDetector.isScenarioDetected() || this._keyboardResume;

            Boolean isHelp = speechHelpOverrideDetector.isScenarioDetected();
            Boolean isExitHelp = speechExitHelpDetector.isScenarioDetected();

            if (isTalkingOnPhone)
            {
                this._debugGestureBox.Text = "talking on phone";
            }

            HandOnFaceIndicator handOnFaceIndicator = new HandOnFaceIndicator();
            if (handOnFaceIndicator.isPositionDetected(skeleton))
            {
                this._debugGestureBox.Text = "hand on face";
            }

            //window.DebugSpeechTextBox.Text = this._isHelpMenu.ToString();

            

            if (this._isHelpMenu)
            {
                if (isExitHelp)
                {
                    hideHelp();
                }
            }
            else
            {
                if (isManualPause)
                {
                    manualPauseResume(true);
                }
                else if (isManualResume)
                {
                    manualPauseResume(false);
                }

                //all the detector logic
                if (isHelp)
                {
                    showHelp();
                }
                else if (isAbsent && !this._isManualResume)
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
                    //this._isManualPause = false;
                    ScreenController.PauseReason reason = ScreenController.PauseReason.PHONE;
                    this._tv.pause(reason);
                }
                else if (isStanding && !this._isManualResume)
                {
                    //this._isManualPause = false;
                    ScreenController.PauseReason reason = ScreenController.PauseReason.STANDUP;
                    this._tv.pause(reason);
                }
                else if (isSitting)
                {
                    this._isManualResume = false;
                    this._keyboardResume = false;
                    if (!this._isManualPause)
                    {
                        this._tv.play();
                    }

                }
            }
            if (isTalkingOnPhone)
            {
                this._debugPositionBox.Text = "I'm talking on phone.";
            }

            if (isStanding)
            {
                this._debugPositionBox.Text = "I'm standing.";
            }

            if (isSitting)
            {
                this._debugPositionBox.Text = "Sitting!";
            }


        }

        private void detectChannelChangingScenarios(Skeleton skeleton, KinectSensor nui)
        {

            if (skeleton != null)
            {

                this.rSwipeDetector.processSkeleton(skeleton);
                this.lSwipeDetector.processSkeleton(skeleton);

                Boolean rHandSwipeDetected = rSwipeDetector.isScenarioDetected();
                Boolean lHandSwipeDetected = lSwipeDetector.isScenarioDetected();
                if (lSwipeDetector.getCurrentState().Pos.Equals(SwipePosition.MOVING))
                {
                    this._debugGestureBox.Text = lSwipeDetector.getSwipePosition() + "";
                }
                else
                {
                    this._debugGestureBox.Text = lSwipeDetector.getCurrentState().toString();
                }



                if (rHandSwipeDetected)
                {
                    if (rSwipeDetector.getSwipeDirection().Equals(SwipeDirection.LEFT))
                    {
                        this._debugGestureBox.Text = "Swipe left!";
                        this._tv.moveMediaToLeft();
                        swipe_sw.Restart();
                    }
                    else if (rSwipeDetector.getSwipeDirection().Equals(SwipeDirection.RIGHT))
                    {

                        this._debugGestureBox.Text = "Right swipe!";
                        this._tv.moveMediaToRight();
                        swipe_sw.Restart();
                    }

                }
                else if (lHandSwipeDetected)
                {
                    if (lSwipeDetector.getSwipeDirection().Equals(SwipeDirection.LEFT))
                    {
                        this._debugGestureBox.Text = "Swipe left!";
                        this._tv.moveMediaToLeft();
                        swipe_sw.Restart();
                    }
                    else if (lSwipeDetector.getSwipeDirection().Equals(SwipeDirection.RIGHT))
                    {

                        this._debugGestureBox.Text = "Right swipe!";
                        this._tv.moveMediaToRight();
                        swipe_sw.Restart();
                    }

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

        private double adjustVolume(double deltaV)
        {
            if (this._tv.Volume + deltaV > 1)
            {
                return 1;
            }
            else if (this._tv.Volume + deltaV < 0)
            {
                return 0;
            }
            else
            {
                return this._tv.Volume + deltaV;
            }
        }

        private void detectVolumeChangingScenarios(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                volumeDetector.processSkeleton(skeleton);
                double deltaVolume = volumeDetector.getVolumeDelta();
                if (deltaVolume != 0)
                {
                    //                    this._debugGestureBox.Text = "cV " + this._tv.Volume;
                    this._tv.Volume = adjustVolume(deltaVolume);
                }
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
                Boolean manualOverrideOn = speechStartOverrideDetector.isScenarioDetected();
                if (manualOverrideOn)
                {
                    this._debugGestureBox.Text = "ON-speech override";
                    // turn on the tv
                    this._tv.turnOn();
                    this._tv.Volume = 0.05;
                }
            }
            else
            {
                SwipeState swipeState = rSwipeDetector.getCurrentState();
                if (this._tv._screenController.isInSwipe)
                {
                    if (swipeState.Pos == SwipePosition.END ||
                        swipeState.Pos == SwipePosition.NEUTRAL)
                    {
                        this._tv._screenController.abortSwipe();
                    }
                    else
                    {
                        this._tv._screenController.updateSwipe(rSwipeDetector.getSwipePosition(), rSwipeDetector.getSwipeDirection());
                    }
                }
                else
                {
                    if (swipeState != null && 
                       (swipeState.Pos == SwipePosition.START ||
                        swipeState.Pos == SwipePosition.MOVING))
                    {
                        this._tv._screenController.startSwipe();
                    }
                }

                detectSittingStandingScenarios(skeleton);
                detectChannelChangingScenarios(skeleton, nui);
                detectVolumeChangingScenarios(skeleton);

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


                this.ambiHandWaveDetector.processSkeleton(skeleton);
                Boolean hasWaved = this.ambiHandWaveDetector.isScenarioDetected();
                if (hasWaved && wave_sw.ElapsedMilliseconds > 1500)
                {
                    this._debugGestureBox.Text = "OFF-has waved";
                    this._tv.turnOff();
                    wave_sw.Reset();
                    wave_sw.Start();
                }
                Boolean manualOverrideOff = speechOffOverrideDetector.isScenarioDetected();
                if (manualOverrideOff)
                {
                    this._debugGestureBox.Text = "OFF-speech override";
                    this._tv.turnOff();
                }
            }
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            //this._tv.fakeTVRun();
        }

        public override void addUIElements(TextBlock not_speaker, TextBlock not_text, Image not_image, WinRectangle rect)
        {

        }

        public override void addVideo(MediaElement mediaElement1)
        {

        }
    }
}
