using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using YouMote;

namespace YouMote.Speech
{
    public class SpeechRecognizer
    {
        private SpeechRecognitionEngine sre;
        private KinectAudioSource kinectAudioSource;
        private MainWindow mainWindow;
        private bool isDisposed;
        private String wordSaid;

        public SpeechRecognizer(MainWindow window)
        {
            mainWindow = window;
            RecognizerInfo ri = GetKinectRecognizer();
            this.sre = new SpeechRecognitionEngine(ri);
            this.isDisposed = false;
            this.LoadGrammar(this.sre);
        }

        public void Start(KinectAudioSource kinectSource)
        {
            this.CheckDisposed();

            this.kinectAudioSource = kinectSource;
            this.kinectAudioSource.AutomaticGainControlEnabled = false;
            this.kinectAudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = this.kinectAudioSource.Start();
            this.sre.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Stop()
        {
            this.CheckDisposed();

            if (this.sre != null)
            {
                this.kinectAudioSource.Stop();
                this.sre.RecognizeAsyncCancel();
                this.sre.RecognizeAsyncStop();

                this.sre.SpeechRecognized -= this.SreSpeechRecognized;
                this.sre.SpeechHypothesized -= this.SreSpeechHypothesized;
                this.sre.SpeechRecognitionRejected -= this.SreSpeechRecognitionRejected;
            }
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("SpeechRecognizer");
            }
        }

        private void LoadGrammar(SpeechRecognitionEngine speechRecognitionEngine)
        {
            //build grammar
            var grammar = new Choices();
            grammar.Add("hi");
            grammar.Add("hello");
            grammar.Add("What's up");
            grammar.Add("Sup");
            grammar.Add("Hey");
            grammar.Add("TV pause");
            grammar.Add("TV play");
            grammar.Add("TV off");
            grammar.Add("TV help");
            grammar.Add("exit help");
            grammar.Add("TV on");

            var gb = new GrammarBuilder { Culture = speechRecognitionEngine.RecognizerInfo.Culture };
            gb.Append(grammar);

            var g = new Grammar(gb);
            speechRecognitionEngine.LoadGrammar(g);

            speechRecognitionEngine.SpeechRecognized += this.SreSpeechRecognized;
            speechRecognitionEngine.SpeechHypothesized += this.SreSpeechHypothesized;
            speechRecognitionEngine.SpeechRecognitionRejected += this.SreSpeechRecognitionRejected;


        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            this.RejectSpeech(e.Result);
        }

        private void RejectSpeech(RecognitionResult result)
        {
            string status = "Rejected: " + (result == null ? string.Empty : result.Text + " " + result.Confidence);
            mainWindow.DebugSpeechTextBox.Text = status;
            wordSaid = null;
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            mainWindow.DebugSpeechTextBox.Text = "hypothesize as: " + e.Result.Text;
            //this.ReportSpeechStatus("Hypothesized: " + e.Result.Text + " " + e.Result.Confidence);
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //SolidColorBrush brush;

            if (e.Result.Confidence < 0.3)
            {
                this.RejectSpeech(e.Result);
                return;
            }
            wordSaid = e.Result.Text;
            mainWindow.DebugSpeechTextBox.Text = "recognize as: " + e.Result.Text;
        }

        public String Word
        {
            get
            {
                return this.wordSaid;
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
    }
}
