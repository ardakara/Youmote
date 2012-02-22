using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    public abstract class PresenceDetector : ScenarioDetectorIMPL
    {

        protected ScenarioStateHistory _history;
        protected StandingIndicator _standingDetector = new StandingIndicator();
        protected SittingIndicator _sittingDetector = new SittingIndicator();
        protected AbsentDetector _absentDetector = new AbsentDetector();
        public PresenceDetector()
        {
            this._history = new ScenarioStateHistory(30);
        }

        public abstract Boolean isScenarioDetected();
        

        public void processSkeleton(SkeletonData skeleton)
        {
            Boolean isAbsent = this._absentDetector.isPositionDetected(skeleton);
            if (isAbsent)
            {
                PresenceState state = new PresenceState(PresenceState.GetsUpAndLeavesPosition.ABSENT, DateTime.Now, DateTime.Now);
                this._history.addState(state);
            }
            else
            {
                Boolean isStanding = this._standingDetector.isPositionDetected(skeleton);
                Boolean isSitting = this._sittingDetector.isPositionDetected(skeleton);
                if (isStanding)
                {
                    PresenceState state = new PresenceState(PresenceState.GetsUpAndLeavesPosition.STAND, DateTime.Now, DateTime.Now);
                    this._history.addState(state);
                }
                else if (isSitting)
                {
                    PresenceState state = new PresenceState(PresenceState.GetsUpAndLeavesPosition.SIT, DateTime.Now, DateTime.Now);
                    this._history.addState(state);
                }

            }

        }

    }
}
