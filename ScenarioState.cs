﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace YouMote
{
    public interface ScenarioState
    {
        Boolean isSameState(ScenarioState ss);

        /// <summary>
        /// States are being added every frame.  Instead of having a list of repeating, sitting frames, we merge two frames if they are the same
        /// and add any additional information that may be needed.  For example, if a person has been sitting for 40 frames and said "hello" on frame 41,
        /// there will be a single state representing sitting that contains "hello" for the 41st frame that will be merged with a state that represents sitting from
        /// 0 to 40th frame.  Now the merged state represents sitting from 0 to 41st state.
        /// </summary>
        /// <param name="ss"></param>
        ScenarioState mergeEqualStates(ScenarioState ss);

        /// <summary>
        /// Used when the state representing previous frames is not equal to the state that is about to be added for most current frame.
        /// </summary>
        /// <param name="next"></param>
        ScenarioState finishState(ScenarioState next);

        DateTime Start{get;}
        DateTime End{get;}
        /// <summary>
        /// returns the duration that this state took place in minutes;
        /// </summary>
        /// <returns></returns>
        double getDurationInMinutes();

        /// <summary>
        /// returns the duration that this state took place in seconds;
        /// </summary>
        /// <returns></returns>
        double getDurationInSeconds();

        /// <summary>
        /// returns the duration that this state took place in seconds;
        /// </summary>
        /// <returns></returns>
        double getDurationInMilliseconds();
    }

}
