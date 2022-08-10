/*Copyright 2015 Esri
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.?*/
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("ae9b2e2c-cca6-49bd-91a9-8d3f834529f6")]
    public class ReassignJob : IJTXCustomStep
    {
        #region Registration Code
        [ComRegisterFunction()]
        static void Reg(String regKey)
        {
            ESRI.ArcGIS.JTX.Utilities.JTXUtilities.RegisterJTXCustomStep(regKey);
        }

        [ComUnregisterFunction()]
        static void Unreg(String regKey)
        {
            ESRI.ArcGIS.JTX.Utilities.JTXUtilities.UnregisterJTXCustomStep(regKey);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////
        // DECLARE: Data Members
        private IJTXDatabase m_ipDatabase = null;
        private readonly string[] m_expectedArgs = { "assignType", "assignTo" };

        #region IJTXCustomStep Members

        /// <summary>
        /// A description of the expected arguments for the step type.  This should
        /// include the syntax of the argument, whether or not it is required/optional, 
        /// and any return codes coming from the step type.
        /// </summary>
        public string ArgumentDescriptions
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"Assign Type:");
                sb.AppendFormat("\t/{0}:<user|group> (required)\r\n\r\n", m_expectedArgs[0]);
                sb.AppendLine(@"Assign To:");
                sb.AppendFormat("\t/{0}:<user|group name> (required)\r\n", m_expectedArgs[1]);
                sb.AppendLine("");
                sb.AppendLine("Example:  /assignType:group /assignTo:Managers");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Called when a step of this type is executed in the workflow.
        /// </summary>
        /// <param name="JobID">ID of the job being executed</param>
        /// <param name="StepID">ID of the step being executed</param>
        /// <param name="argv">Array of arguments passed into the step's execution</param>
        /// <param name="ipFeedback">Feedback object to return status messages and files</param>
        /// <returns>Return code of execution for workflow path traversal</returns>
        public int Execute(int jobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {
            IJTXJobManager ipJobManager = m_ipDatabase.JobManager;
            IJTXConfiguration ipConfig = m_ipDatabase.ConfigurationManager;
            IJTXJob ipJob = ipJobManager.GetJob(jobID);
            
            string strAssignType = "";
            string strAssignTo = "";

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strAssignType))
            {
                throw new ArgumentNullException(m_expectedArgs[0], string.Format("\nMissing the {0} parameter!", m_expectedArgs[0]));
            }

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[1], true, out strAssignTo))
            {
                throw new ArgumentNullException(m_expectedArgs[1], string.Format("\nMissing the {0} parameter!", m_expectedArgs[1]));
            }

            if (strAssignType == "group")
            {
                if (ipConfig.GetUserGroup(strAssignTo) == null)
                {
                    throw new ArgumentOutOfRangeException(m_expectedArgs[1], string.Format("\nThe group {0} is not a group defined in the JTX database!", strAssignTo));
                }

                ipJob.AssignedType = jtxAssignmentType.jtxAssignmentTypeGroup;
            }
            else
            {
                if (ipConfig.GetUser(strAssignTo) == null)
                {
                    throw new ArgumentOutOfRangeException(m_expectedArgs[1], string.Format("\nThe user {0} is not a user defined in the JTX database!", strAssignTo));
                }

                ipJob.AssignedType = jtxAssignmentType.jtxAssignmentTypeUser;
            }

            ipJob.AssignedTo = strAssignTo;
            ipJob.Store();

            return 0;
        }

        /// <summary>
        /// Invoke an editor tool for managing custom step arguments.  This is
        /// an optional feature of the custom step and may not be implemented.
        /// </summary>
        /// <param name="hWndParent">Handle to the parent application window</param>
        /// <param name="argsIn">Array of arguments already configured for this custom step</param>
        /// <returns>Returns a list of newely configured arguments as specified via the editor tool</returns>
        public object[] InvokeEditor(int hWndParent, object[] argsIn)
        {
            throw new NotImplementedException("No edit dialog available for this step type");
        }

        /// <summary>
        /// Called when the step is instantiated in the workflow.
        /// </summary>
        /// <param name="ipDatabase">Database connection to the JTX repository.</param>
        public void OnCreate(IJTXDatabase ipDatabase)
        {
            m_ipDatabase = ipDatabase;
        }

        /// <summary>
        /// Method to validate the configured arguments for the step type.  The
        /// logic of this method depends on the implementation of the custom step
        /// but typically checks for proper argument names and syntax.
        /// </summary>
        /// <param name="argv">Array of arguments configured for the step type</param>
        /// <returns>Returns 'true' if arguments are valid, 'false' if otherwise</returns>
        public bool ValidateArguments(ref object[] argv)
        {
            IJTXConfiguration ipConfig = m_ipDatabase.ConfigurationManager;

            string strAssignType = "";
            string strAssignTo = "";

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strAssignType))
            {
                throw new ArgumentNullException(m_expectedArgs[0], string.Format("\nMissing the {0} parameter!", m_expectedArgs[0]));
            }

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[1], true, out strAssignTo))
            {
                throw new ArgumentNullException(m_expectedArgs[1], string.Format("\nMissing the {0} parameter!", m_expectedArgs[1]));
            }

            if (strAssignType == "group")
            {
                if (ipConfig.GetUserGroup(strAssignTo) == null)
                {
                    throw new ArgumentOutOfRangeException(m_expectedArgs[1], string.Format("\nThe group {0} is not a group defined in the JTX database!", strAssignTo));
                }
            }
            else
            {
                if (ipConfig.GetUser(strAssignTo) == null)
                {
                    throw new ArgumentOutOfRangeException(m_expectedArgs[1], string.Format("\nThe user {0} is not a user defined in the JTX database!", strAssignTo));
                }
            }
            return true;
        }

        #endregion

    }	// End Class
}	// End Namespace
