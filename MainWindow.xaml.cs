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
        MediaElement curVid;

        Skeleton[] skeletons;

        //Scaling constants
        public float k_xMaxJointScale = 3.0f;
        public float k_yMaxJointScale = 3.0f;
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        //audio stuff
        RecognizerInfo speechRecognizer;
        KinectAudioSource audioSource;
        SpeechRecognitionEngine sre;
        Stream stream;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupKinect();
            youmoteController = new YoumoteController(this);
            currentController = youmoteController;

 /*           
            audioSource = nui.AudioSource;
            audioSource.EchoCancellationMode = EchoCancellationMode.None; // No AEC for this sample
            audioSource.AutomaticGainControlEnabled = false; // Important to turn this off for speech recognition
            BuildSpeechEngine();
            nui.Stop();
  */
        }

        private void BuildSpeechEngine()
        {
            speechRecognizer = GetKinectRecognizer();
            if (speechRecognizer == null)
            {
                targets[1].setTargetText("Could not find Kinect speech recognizer. Please refer to the sample requirements.");
                return;
            }
            targets[1].setTargetText("Using: " + speechRecognizer.Name);

            int wait = 4;
            while (wait > 0)
            {
                targets[1].setTargetText("Device will be ready in " + wait + "second(s).\r");
                wait--;
                Thread.Sleep(1000);
            }

            //sre = new SpeechRecognitionEngine(speechRecognizer.Id);
            using (var sre = new SpeechRecognitionEngine(speechRecognizer.Id))
            {
                var choices = new Choices();
                choices.Add("hi");
                choices.Add("hello");
                choices.Add("What's up");
                choices.Add("Sup");
                choices.Add("Hey");

                var gb = new GrammarBuilder { Culture = speechRecognizer.Culture };
                gb.Append(choices);

                var g = new Grammar(gb);
                sre.LoadGrammar(g);
                sre.SpeechRecognized += SreSpeechRecognized;
                sre.SpeechHypothesized += SreSpeechHypothesized;
                sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
                using (stream = audioSource.Start())
                {
                    sre.SetInputToAudioStream(
                    stream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

                    Console.WriteLine("Recognizing speech. Say: 'red', 'green' or 'blue'. Press ENTER to stop");

                    sre.RecognizeAsync(RecognizeMode.Multiple);
                    Console.ReadLine();
                    Console.WriteLine("Stopping recognizer ...");
                    sre.RecognizeAsyncStop();
                }
            }
        }

        private static void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("\nSpeech Rejected");
            /*         if (e.Result != null)
                     {
                         DumpRecordedAudio(e.Result.Audio);
                     }
             */
        }

        private static void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        private static void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.7)
            {
                Console.WriteLine("\nSpeech Recognized: \t{0}\tConfidence:\t{1}", e.Result.Text, e.Result.Confidence);
            }
            else
            {
                Console.WriteLine("\nSpeech Recognized but confidence was too low: \t{0}", e.Result.Confidence);
                //DumpRecordedAudio(e.Result.Audio);
            }
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
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

                //add event to receive skeleton data
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
