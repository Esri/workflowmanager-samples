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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("9d4a3611-7455-485d-a583-3bfe5b66789b")]
    public class CreateJob : IJTXCustomStep
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
        private readonly string[] m_expectedArgs = { "jobtypeid", "assigngroup", "assignuser" };
        private IJTXDatabase m_ipDatabase = null;

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
                sb.AppendLine(@"Job Type ID:");
                sb.AppendFormat("\t/{0}:<job type id> (required)\r\n\r\n", m_expectedArgs[0]);
                sb.AppendLine(@"Assign To Group:");
                sb.AppendFormat("\t/{0}:<group to assign to> (optional)\r\n", m_expectedArgs[1]);
                sb.AppendLine(@"Assign To User:");
                sb.AppendFormat("\t/{0}:<username to assign to> (optional)\r\n", m_expectedArgs[2]);

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
        public int Execute(int JobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {
            System.Diagnostics.Debug.Assert(m_ipDatabase != null);

            string strValue = "";
            int jobTypeID = 0;

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strValue))
            {
                throw new ArgumentNullException(m_expectedArgs[0], string.Format("\nMissing the {0} parameter!", m_expectedArgs[0]));
            }

            if (!Int32.TryParse(strValue, out jobTypeID))
            {
                throw new ArgumentNullException(m_expectedArgs[0], "Argument must be an integrer!");
            }

            IJTXJobType pJobType = m_ipDatabase.ConfigurationManager.GetJobTypeByID(jobTypeID);
            IJTXJobManager pJobMan = m_ipDatabase.JobManager;
            IJTXJob pNewJob = pJobMan.CreateJob(pJobType, 0, true);

            IJTXActivityType pActType = m_ipDatabase.ConfigurationManager.GetActivityType(Constants.ACTTYPE_CREATE_JOB);
            if (pActType != null)
            {
                pNewJob.LogJobAction(pActType, null, "");
            }

            JTXUtilities.SendNotification(Constants.NOTIF_JOB_CREATED, m_ipDatabase, pNewJob, null);

            // Assign a status to the job if the Auto Assign Job Status setting is enabled
            IJTXConfigurationProperties pConfigProps = (IJTXConfigurationProperties)m_ipDatabase.ConfigurationManager;
            if (pConfigProps.PropertyExists(Constants.JTX_PROPERTY_AUTO_STATUS_ASSIGN))
            {
                string strAutoAssign = pConfigProps.GetProperty(Constants.JTX_PROPERTY_AUTO_STATUS_ASSIGN);
                if (strAutoAssign == "TRUE")
                {
                    pNewJob.Status = m_ipDatabase.ConfigurationManager.GetStatus("Created");
                }
            }

            // Associate the current job with the new job with a parent-child relationship
            pNewJob.ParentJob = JobID;
            
            // Assign the job as specified in the arguments
            string strAssignTo = "";
            if (StepUtilities.GetArgument(ref argv, m_expectedArgs[1], true, out strAssignTo))
            {
                pNewJob.AssignedType = jtxAssignmentType.jtxAssignmentTypeGroup;
                pNewJob.AssignedTo = strAssignTo;
            }
            else if (StepUtilities.GetArgument(ref argv, m_expectedArgs[2], true, out strAssignTo))
            {
                pNewJob.AssignedType = jtxAssignmentType.jtxAssignmentTypeUser;
                pNewJob.AssignedTo = strAssignTo;
            }
            pNewJob.Store();

            // Copy the workflow to the new job
            WorkflowUtilities.CopyWorkflowXML(m_ipDatabase, pNewJob);

            // Create 1-1 extended property entries
            IJTXAuxProperties pAuxProps = (IJTXAuxProperties)pNewJob;
            System.Array contNames = pAuxProps.ContainerNames;
            IEnumerator contNamesEnum = contNames.GetEnumerator();
            contNamesEnum.Reset();

            while (contNamesEnum.MoveNext())
            {
                string strContainerName = (string)contNamesEnum.Current;
                IJTXAuxRecordContainer pAuxContainer = pAuxProps.GetRecordContainer(strContainerName);
                if (pAuxContainer.RelationshipType == esriRelCardinality.esriRelCardinalityOneToOne)
                {
                    pAuxContainer.CreateRecord();
                }
            }

            m_ipDatabase.LogMessage(5, 1000, System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle);

            // Update Application message about the new job
            if (System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle.Length > 0) //if its not running in server
            {
                MessageBox.Show("Created " + pJobType.Name + " Job " + pNewJob.ID, "Job Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

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
            JTXSamples.ArgEditor editorForm = new JTXSamples.ArgEditor(m_ipDatabase, m_expectedArgs);
            object[] newArgs = null;

            return (editorForm.ShowDialog(argsIn, out newArgs) == DialogResult.OK) ? newArgs : argsIn;
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
            string strValue = "";

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strValue)) { return false; }
            return StepUtilities.AreArgumentNamesValid(ref argv, m_expectedArgs);
        }

        #endregion

    }	// End Class
}	// End Namespace
