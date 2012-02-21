using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{

    class TransitionSDetector:ScenarioDetector
    {

        enum TransitionSStates {NONE, MISSING, RETURNED };

        Enum getScenarioState()
        {
            return TransitionSStates.NONE;
        }

        Boolean isSuccess(Enum scenarioState)
        {
            return false;
        }

        void processSkeleton(SkeletonData skeleton)
        {
            
        }

        public class TransisitionSState
        {
            enum PositionState { Sit, Stand, Prone,Absent};
            PositionState state;
            DateTime curTime;
            
        }
    }
}
