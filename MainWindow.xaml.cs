/* Migration to SDK v1.0
 * TODO:
 * - if the current video image doesn't work, go back to ToBitmapSource();
 * 
 * Help sources:
 * - http://robrelyea.wordpress.com/2012/02/01/k4w-code-migration-from-beta2-to-v1-0-managed/
 * - http://robrelyea.wordpress.com/2012/02/01/k4w-details-of-api-changes-from-beta2-to-v1-managed/#_KinectSensor_discovery
 */

namespace YouMote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Microsoft.Kinect;

    using Coding4Fun.Kinect.Wpf;
    using System.Threading;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using System.IO;
    using YouMote.Speech;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //YouMote.Client.Client.test();
            InitializeComponent();
            showDebug();
            loadHelpSources();
        }

        void loadHelpSources()
        {
            this.Help1.Loaded += new RoutedEventHandler(helpLoaded);
            this.Help2.Loaded += new RoutedEventHandler(helpLoaded);
            this.Help3.Loaded += new RoutedEventHandler(helpLoaded);
            this.Help4.Loaded += new RoutedEventHandler(helpLoaded);
            this.Help5.Loaded += new RoutedEventHandler(helpLoaded);
            this.Help1.MediaEnded += this.handleMediaEnded;
            this.Help2.MediaEnded += this.handleMediaEnded;
            this.Help3.MediaEnded += this.handleMediaEnded;
            this.Help4.MediaEnded += this.handleMediaEnded;
            this.Help5.MediaEnded += this.handleMediaEnded;
        }

        void handleMediaEnded(object sender, EventArgs e)
        {
            MediaElement me = (MediaElement)(sender);
            me.Position= TimeSpan.FromSeconds(0);
            me.Play();

        }
        int helpVideoIdx = 0;
        String[] helpVideoPaths = { "\\Video\\help-tv-on.mp4", "\\Video\\help-tv-pause.mp4", "\\Video\\help-tv-play.mp4",
                                  "\\Video\\help-volume-gesture.mp4", "\\Video\\help-wave-gesture.mp4"};


        private void helpLoaded(object sender, RoutedEventArgs e)
        {
            String currentPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            MediaElement me = (MediaElement)(sender);
            if (me.Source == null)
            {
                me.Source = new Uri(currentPath + helpVideoPaths[helpVideoIdx]);
                helpVideoIdx++;
            }
        }
        //Kinect Sensor
        KinectSensor nui;

        //Targets and skeleton controller
        YoumoteController youmoteController;

        //Holds the currently active controller
        SkeletonController currentController;

        Dictionary<int, Target> targets = new Dictionary<int, Target>();

        Skeleton[] skeletons;

        //Scaling constants
        public float k_xMaxJointScale = 3.0f;
        public float k_yMaxJointScale = 3.0f;
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private Boolean socialShown = false;


        SpeechRecognizer mySpeechRecognizer;

        public SpeechRecognizer speechRecognizer
        {
            get
            {
                return this.mySpeechRecognizer;
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupKinect();
        }

        private void SetupKinect()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                this.Title = "No Kinect connected";
            }
            else
            {
                //use first Kinect
                nui = KinectSensor.KinectSensors[0];

                //Initialize to do skeletal tracking
                nui.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                nui.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                nui.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                });
                nui.Start();

                //need 4 seconds for kinect speech to be ready
                int wait = 4;
                while (wait > 0)
                {
                    wait--;
                    Thread.Sleep(1000);
                }

                this.mySpeechRecognizer = new SpeechRecognizer(this);
                this.mySpeechRecognizer.Start(nui.AudioSource);

                //add event to receive skeleton data
                youmoteController = new YoumoteController(this);
                currentController = youmoteController;
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            }
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (this.skeletons == null || this.skeletons.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        this.skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    receivedData = true;
                }

                if (receivedData)
                {
                    skeletonFrame.CopySkeletonDataTo(this.skeletons);
                    //get the first tracked skeleton
                    Skeleton skeleton = (from s in this.skeletons
                                         where s.TrackingState == SkeletonTrackingState.Tracked
                                         select s).FirstOrDefault();
                    if (skeleton != null)
                    {
                        //set positions on our joints of interest (already defined as Ellipse objects in the xaml)
                        SetEllipsePosition(headEllipse, skeleton.Joints[JointType.Head]);
                        SetEllipsePosition(leftEllipse, skeleton.Joints[JointType.HandLeft]);
                        SetEllipsePosition(rightEllipse, skeleton.Joints[JointType.HandRight]);
                        SetEllipsePosition(shoulderCenter, skeleton.Joints[JointType.ShoulderCenter]);
                        SetEllipsePosition(shoulderRight, skeleton.Joints[JointType.ShoulderRight]);
                        SetEllipsePosition(shoulderLeft, skeleton.Joints[JointType.ShoulderLeft]);
                        SetEllipsePosition(ankleRight, skeleton.Joints[JointType.AnkleRight]);
                        SetEllipsePosition(ankleLeft, skeleton.Joints[JointType.AnkleLeft]);
                        SetEllipsePosition(footLeft, skeleton.Joints[JointType.FootLeft]);
                        SetEllipsePosition(footRight, skeleton.Joints[JointType.FootRight]);
                        SetEllipsePosition(wristLeft, skeleton.Joints[JointType.WristLeft]);
                        SetEllipsePosition(wristRight, skeleton.Joints[JointType.WristRight]);
                        SetEllipsePosition(elbowLeft, skeleton.Joints[JointType.ElbowLeft]);
                        SetEllipsePosition(elbowRight, skeleton.Joints[JointType.ElbowRight]);
                        SetEllipsePosition(ankleLeft, skeleton.Joints[JointType.AnkleLeft]);
                        SetEllipsePosition(footLeft, skeleton.Joints[JointType.FootLeft]);
                        SetEllipsePosition(footRight, skeleton.Joints[JointType.FootRight]);
                        SetEllipsePosition(wristLeft, skeleton.Joints[JointType.WristLeft]);
                        SetEllipsePosition(wristRight, skeleton.Joints[JointType.WristRight]);
                        SetEllipsePosition(kneeLeft, skeleton.Joints[JointType.KneeLeft]);
                        SetEllipsePosition(kneeRight, skeleton.Joints[JointType.KneeRight]);
                        SetEllipsePosition(hipCenter, skeleton.Joints[JointType.HipCenter]);
                    }
                    currentController.processSkeletonFrame(skeleton, nui, targets);
                }
            }
        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            var scaledJoint = joint.ScaleTo(1024, 1280, k_xMaxJointScale, k_yMaxJointScale);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetTop(ellipse, scaledJoint.Position.Y - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetZIndex(ellipse, (int)-Math.Floor(scaledJoint.Position.Z * 100));
            if (joint.JointType == JointType.HandLeft || joint.JointType == JointType.HandRight)
            {
                byte val = (byte)(Math.Floor((joint.Position.Z - 0.8) * 255 / 2));
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(val, val, val));
            }
        }

        void showDebug()
        {
            SkeletonCanvas.Visibility = Visibility.Visible;
            this.DebugPositionTextBox.Visibility = Visibility.Visible;
            this.DebugGestureTextBox.Visibility = Visibility.Visible;
            this.DebugSpeechTextBox.Visibility = Visibility.Visible;
        }

        void hideDebug()
        {
            SkeletonCanvas.Visibility = Visibility.Hidden;
            this.DebugPositionTextBox.Visibility = Visibility.Hidden;
            this.DebugGestureTextBox.Visibility = Visibility.Hidden;
            this.DebugSpeechTextBox.Visibility = Visibility.Hidden;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Stop();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (nui != null)
                {
                    nui.Stop();
                }
                Application.Current.Shutdown();
            }
            else if (e.Key == Key.D1)
            {
                hideDebug();
            }
            else if (e.Key == Key.D2)
            {
                showDebug();
            }
            else if (e.Key == Key.D3)
            {

                if (youmoteController.isHelpMenu)
                {
                    youmoteController.hideHelp();
                }
                else
                {
                    youmoteController.showHelp();
                }
            }
            else if (e.Key == Key.D4)
            {
                try
                {
                    nui.ElevationAngle = 10;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                }
            }
            else if (e.Key == Key.D5)
            {
                nui.ElevationAngle = nui.ElevationAngle = 25;
                try
                {
                    nui.ElevationAngle = -10;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                }
            }

            else if (e.Key == Key.D6)
            {
                try
                {
                    nui.ElevationAngle = 0;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                }
            }
            else if (e.Key == Key.S) {
                if (socialShown)
                {
                    youmoteController.setNotificationToHidden();
                    socialShown = false;
                }
                else
                {
                    youmoteController.setNotificationToVisible();
                    socialShown = true;
                }      
            }
            else if (youmoteController != null)
            {
                youmoteController.processKeys(e.Key);
            }
        }

        private void NotificationBackground_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
