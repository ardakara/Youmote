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
            youmoteController = new YoumoteController(this);
            currentController = youmoteController;
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

                //add event to receive skeleton data
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
                this.mySpeechRecognizer = new SpeechRecognizer(this);
                this.mySpeechRecognizer.Start(nui.AudioSource);

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



        private void Window_Closed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Stop();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.D1)
            {
                currentController = youmoteController;
                currentController.controllerActivated(targets);
            }
        }


        private void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            //mediaElement1.Source = new Uri("Video/pixar_short.avi", UriKind.Relative);

        }
    }


}
