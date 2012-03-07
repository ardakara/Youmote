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
        protected ScenarioStateHistory _history;
        protected HandOnFaceIndicator _handOnFaceIndicator = new HandOnFaceIndicator();
        private MainWindow window;
        private SpeechRecognizer speechRecognizer;

        private HashSet<String> greeting = new HashSet<String>();


        public TalkOnPhoneDetector(MainWindow win)
        {
            this._history = new ScenarioStateHistory(30);
            this.window = win;
            this.speechRecognizer = win.speechRecognizer;
            greeting.Add("hi");
            greeting.Add("hello");
            greeting.Add("What's up");
            greeting.Add("Sup");
            greeting.Add("Hey");
        }

        public Boolean isScenarioDetected()
        {
            ScenarioState curState = this._history.Peek();
            if (curState!=null && TalkOnPhoneState.TALK_ON_PHONE.isSameState(curState))
            {
                return true;
            }

            List<ScenarioState> prevStates = this._history.getLastNStates(2);
            if (prevStates.Count == 2)
            {
                Boolean prevStateOnPhone = TalkOnPhoneState.TALK_ON_PHONE.isSameState(prevStates[1]);
                Boolean curStateHandOnHead = TalkOnPhoneState.HAND_ON_FACE.isSameState(prevStates[0]);
                if (prevStateOnPhone && curStateHandOnHead)
                {
                    return true;
                }
            }

            

            return false;
            
        }
        public void processSkeleton(Skeleton skeleton)
        {
            Boolean handIsOnFace = this._handOnFaceIndicator.isPositionDetected(skeleton);
            TalkOnPhoneState state = null;
            if (handIsOnFace && speechRecognizer != null)
            {
                String wordsSaid = speechRecognizer.Word;

                if (greeting.Contains(wordsSaid))
                {
                    state = new TalkOnPhoneState(TalkOnPhoneState.PhoneState.TALK_ON_PHONE, DateTime.Now, DateTime.Now);

                }
                else
                {
                    state = new TalkOnPhoneState(TalkOnPhoneState.PhoneState.HAND_ON_FACE, DateTime.Now, DateTime.Now);
                }
            }
            else
            {
                state = new TalkOnPhoneState(TalkOnPhoneState.PhoneState.HAND_DOWN, DateTime.Now, DateTime.Now);
            }
            this._history.addState(state);
        }
    }
}
