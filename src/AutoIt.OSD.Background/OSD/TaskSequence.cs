//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using TSEnvironmentLib;

namespace AutoIt.OSD
{
    public class TaskSequence
    {
        /// <summary>
        ///     If running from a Task Sequence returns the tsVariable,
        /// </summary>
        /// <param name="tsVariable"></param>
        /// <returns></returns>
        public static string GetVariable(string tsVariable)
        {
            try
            {
                ITSEnvClass tsEnvVar = new TSEnvClass();
                return tsEnvVar[tsVariable];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     If running from a Task Sequence set a variable.
        /// </summary>
        /// <param name="tsVariable"></param>
        /// <param name="tsValue"></param>
        public static void SetVariable(string tsVariable, string tsValue)
        {
            try
            {
                ITSEnvClass tsEnvVar = new TSEnvClass();
                tsEnvVar[tsVariable] = tsValue;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}