using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YouMote.Speech;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using YouMote.Indicators;
using YouMote.States;

namespace YouMote.Detectors
{
    class TalkOnPhoneDetector : ScenarioDetector
    {
        private static double MIN_HAND_ON_FACE_TIME_IN_SECONDS = 0.5;
        protected ScenarioStateHistory _history;
        protected HandOnFaceIndicator _handOnFaceIndicator = new HandOnFaceIndicator();
        private Boolean isTalkingOnPhone = false;
        private MainWindow window;
        private SpeechRecognizer speechRecognizer;

        private HashSet<String> greeting = new HashSet<String>();


        public TalkOnPhoneDetector(MainWindow win)
        {
            this._history = new ScenarioStateHistory(30);
            this.window = win;
            this.speechRecognizer = win.speechRecognizer;
            greeting.Add("Hi");
            greeting.Add("Hello");
            greeting.Add("What's up");
            greeting.Add("Sup");
            greeting.Add("Hey");
        }

        public Boolean isScenarioDetected()
        {
            ScenarioState lastState = this._history.Peek();
            if (TalkOnPhoneState.TALK_ON_PHONE.isSameState(lastState))
            {
                return true;
                
            }
            return false;
        }
        public void processSkeleton(Skeleton skeleton)
        {
            Boolean handIsOnFace = this._handOnFaceIndicator.isPositionDetected(skeleton);
            TalkOnPhoneState state = null;
            if (handIsOnFace)
            {
                String wordsSaid = speechRecognizer.Word;
                
                if (greeting.Contains(wordsSaid))
                {
                    state = new TalkOnPhoneState(TalkOnPhoneState.PhoneState.TALK_ON_PHONE, DateTime.Now, DateTime.Now);
                    isTalkingOnPhone = true;
                    
                }
                else
                {
                    state = new TalkOnPhoneState(TalkOnPhoneState.PhoneState.HAND_ON_FACE, DateTime.Now, DateTime.Now);
                    isTalkingOnPhone = false;
                }
            }
            else
            {
                state = new TalkOnPhoneState(TalkOnPhoneState.PhoneState.HAND_DOWN, DateTime.Now, DateTime.Now);
                isTalkingOnPhone = false;
            }
            this._history.addState(state);
        }
    }
}
