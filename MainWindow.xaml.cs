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

        private static Boolean isDebug = true;
        public MainWindow()
        {
            //YouMote.Client.Client.test();
            InitializeComponent();
            if (isDebug)
            {
                this.DebugPositionTextBox.Visibility = Visibility.Visible;
                this.DebugGestureTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                this.DebugPositionTextBox.Visibility = Visibility.Hidden;
                this.DebugGestureTextBox.Visibility = Visibility.Hidden;
            }

        }

        //Kinect Sensor
        KinectSensor nui;

        //Targets and skeleton controller
        YoumoteController youmoteController;
        HelpController helpController;

        //Holds the currently active controller
        SkeletonController currentController;

        Dictionary<int, Target> targets = new Dictionary<int, Target>();

        Skeleton[] skeletons;

        //Scaling constants
        public float k_xMaxJointScale = 3.0f;
        public float k_yMaxJointScale = 3.0f;
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        SpeechRecognizer mySpeechRecognizer;

        /*
        public SpeechRecognizer getSpeechRecognizer()
        {
            return this.mySpeechRecognizer;
        }
*/
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
            helpController = new HelpController(this);
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
                setSkeletonToInvisible();
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

        void setSkeletonToVisible()
        {
            headEllipse.Visibility = Visibility.Visible;
            leftEllipse.Visibility = Visibility.Visible;
            rightEllipse.Visibility = Visibility.Visible;
            shoulderCenter.Visibility = Visibility.Visible;
            shoulderRight.Visibility = Visibility.Visible;
            shoulderLeft.Visibility = Visibility.Visible;
            ankleRight.Visibility = Visibility.Visible;
            ankleLeft.Visibility = Visibility.Visible;
            footLeft.Visibility = Visibility.Visible;
            footRight.Visibility = Visibility.Visible;
            wristLeft.Visibility = Visibility.Visible;
            wristRight.Visibility = Visibility.Visible;
            elbowLeft.Visibility = Visibility.Visible;
            elbowRight.Visibility = Visibility.Visible;
            ankleLeft.Visibility = Visibility.Visible;
            footLeft.Visibility = Visibility.Visible;
            footRight.Visibility = Visibility.Visible;
            wristLeft.Visibility = Visibility.Visible;
            wristRight.Visibility = Visibility.Visible;
            kneeLeft.Visibility = Visibility.Visible;
            kneeRight.Visibility = Visibility.Visible;
            hipCenter.Visibility = Visibility.Visible;
        }


        void setSkeletonToInvisible()
        {
            headEllipse.Visibility = Visibility.Hidden;
            leftEllipse.Visibility = Visibility.Hidden;
            rightEllipse.Visibility = Visibility.Hidden;
            shoulderCenter.Visibility = Visibility.Hidden;
            shoulderRight.Visibility = Visibility.Hidden;
            shoulderLeft.Visibility = Visibility.Hidden;
            ankleRight.Visibility = Visibility.Hidden;
            ankleLeft.Visibility = Visibility.Hidden;
            footLeft.Visibility = Visibility.Hidden;
            footRight.Visibility = Visibility.Hidden;
            wristLeft.Visibility = Visibility.Hidden;
            wristRight.Visibility = Visibility.Hidden;
            elbowLeft.Visibility = Visibility.Hidden;
            elbowRight.Visibility = Visibility.Hidden;
            ankleLeft.Visibility = Visibility.Hidden;
            footLeft.Visibility = Visibility.Hidden;
            footRight.Visibility = Visibility.Hidden;
            wristLeft.Visibility = Visibility.Hidden;
            wristRight.Visibility = Visibility.Hidden;
            kneeLeft.Visibility = Visibility.Hidden;
            kneeRight.Visibility = Visibility.Hidden;
            hipCenter.Visibility = Visibility.Hidden;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Stop();
        }

        private void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            //mediaElement1.Source = new Uri("Video/pixar_short.avi", UriKind.Relative);

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
            else if (e.Key == Key.D1)
            {
                if (!(currentController == youmoteController))
                {
                    currentController = youmoteController;
                    this.DebugControllerBox.Text = "Youmote";
                    setSkeletonToInvisible();
                }
            }
            else if (e.Key == Key.D2)
            {
                if (!(currentController == helpController))
                {
                    currentController = helpController;
                    this.DebugControllerBox.Text = "Help";
                    setSkeletonToVisible();
                }
            }
            
            else if (youmoteController != null)
            {
                youmoteController.processKeys(e.Key);
            }
        }
    }
}
