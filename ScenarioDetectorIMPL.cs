using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{

/// <summary>
/// Takes in a skeleton and processes it.  Creates an internal state model of 
/// skeleton history (i.e.  states that represent the skeleton went sitting->standing->leaving).
/// The getScenarioState() function return the current state of the scenario, either completed or uncompleted or other variants if needed.
/// </summary>
    interface ScenarioDetectorIMPL
    {
        /// <summary>
        /// gets the current state of the scenario (usually an enum meaning whether the scenario has happened or hasn't happened.
        /// We chose enum here because future detectors might be more nuanced than yes or no states.  It does this by looking at the history of 
        /// sub states for the skeleton (standing, sitting) and sees if these combine to a person leaving.
        /// </summary>
        /// <returns></returns>
        Boolean isScenarioDetected();

/// <summary>
/// Takes in a skeleton and uses posture detection and other qualities to determine a sub-state (standing,sitting,phoneing).  
/// It then adds this state to its history of states.
/// </summary>
/// <param name="skeleton"></param>
void processSkeleton(Skeleton skeleton);
    }
}
