//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using ProgressUILib;
using TSEnvironmentLib;

// ReSharper disable once CheckNamespace
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

        /// <summary>
        /// Gets a Dictionary key/value pairs of current TS variables and values.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllVariables()
        {
            var variablesDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            try
            {
                ITSEnvClass tsEnvVar = new TSEnvClass();
                var variables = tsEnvVar.GetVariables();

                foreach (string variable in variables)
                {
                    variablesDictionary.Add(variable, tsEnvVar[variable]);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return variablesDictionary;
        }

        /// <summary>
        /// Hides the task sequence progress UI dialog
        /// </summary>
        public static void CloseProgressDialog()
        {
            try
            {
                IProgressUI progressUI = new ProgressUI();
                progressUI.CloseProgressDialog();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Show the task sequence dialog with default values.
        /// </summary>
        public static void ShowTSProgress()
        {
            ShowTSProgress(null, null, null, null);
        }

        /// <summary>
        /// Show the task sequence dialog. Use null to indicate default values.
        /// </summary>
        public static void ShowTSProgress(string orgName, string tsName, string customTitle, string curAction)
        {
            try
            {
                IProgressUI progressUI = new ProgressUI();
                ITSEnvClass tsEnvVar = new TSEnvClass();

                orgName = orgName == null ? tsEnvVar["_SMSTSOrgName"] : orgName;
                tsName = tsName == null ?  tsEnvVar["SMSTSPackageName"] : tsName;
                customTitle = customTitle == null ? tsEnvVar["_SMSTSCustomProgressDialogMessage"] : customTitle;
                curAction = curAction == null ? tsEnvVar["_SMSTSCurrentActionName"] : curAction;
                
                uint uStep = Convert.ToUInt32(tsEnvVar["_SMSTSNextInstructionPointer"]);
                uint uMaxStep = Convert.ToUInt32(tsEnvVar["_SMSTSInstructionTableSize"]);
                progressUI.ShowTSProgress(orgName, tsName, customTitle, curAction, uStep, uMaxStep);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Tests if a given task sequence variable name usually holds a password.
        /// </summary>
        /// <param name="tsVariable">Name of the task sequence variable to test</param>
        /// <returns></returns>
        public static bool IsPasswordVariable(string tsVariable)
        {
            string name = tsVariable.ToUpper();

            // Catch all for anything that sounds like a password variable
            if (name.Contains("PASSWORD"))
            {
                return true;
            }

            // Specific variable names
            switch (name)
            {
                case "_SMSTSRESERVED2-000":
                //case "OSDJOINPASSWORD":
                //case "OSDLOCALADMINPASSWORD":
                //case "OSDRANDOMADMINPASSWORD":
                    return true;
                
                default:
                    return false;
            }
        }
    }
}