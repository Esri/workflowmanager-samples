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

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("63774aaa-5849-4b9e-a519-6728377fa104")]
    public class CreateVersion : IJTXCustomStep
    {
        ////////////////////////////////////////////////////////////////////////
        // INFO
        // Return Codes:
        // -1 : Unsuccessful
        //  0 : Successful

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
        private readonly string[] m_expectedArgs = { "scope", "name" };

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
                sb.AppendLine(@"Version Access Scope:");
                sb.AppendFormat("\t/{0}:<public|private|protected> (optional)\r\n", m_expectedArgs[0]);

                sb.AppendLine(@"Version Name Override (overrides default version name assignment):");
                sb.AppendFormat("\t/{0}:<version name> (optional)\r\n", m_expectedArgs[1]);

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
            if (jobID <= 0)
            {
                throw (new ArgumentOutOfRangeException("JobID", jobID, "Job ID must be a positive value"));
            }

            System.Diagnostics.Debug.Assert(m_ipDatabase != null);

            IJTXJobManager pJobMan = m_ipDatabase.JobManager;
            IJTXJob2 pJob = pJobMan.GetJob(jobID) as IJTXJob2;

            // Make sure all the information exists to create this verion
            if (pJob.ActiveDatabase == null)
            {
                System.Windows.Forms.MessageBox.Show("Job does not have a data workspace");
                return -1; 
            }
            if (pJob.ParentVersion == "")
            {
                System.Windows.Forms.MessageBox.Show("Job does not have a parent version");
                return -1; 
            }
            string strVersionNameOverride;
            bool bVNOverride = StepUtilities.GetArgument(ref argv, m_expectedArgs[1], true, out strVersionNameOverride);
            if (pJob.VersionName == "" & !bVNOverride)
            {
                System.Windows.Forms.MessageBox.Show("The job does not have a version name");
                return -1;
            }

            IVersion pNewVersion;
            string strVersionName;

            if (bVNOverride) strVersionName = strVersionNameOverride;
            else strVersionName = pJob.VersionName;

            int index = strVersionName.IndexOf(".", 0);
            if (index >= 0)
            {
                strVersionName = strVersionName.Substring(index + 1);
            }

            esriVersionAccess verAccess = esriVersionAccess.esriVersionAccessPrivate;
            string strVerScope = "";

            if (StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strVerScope))
            {
                strVerScope = strVerScope.ToLower().Trim();

                if (strVerScope == "public")
                {
                    verAccess = esriVersionAccess.esriVersionAccessPublic;
                }
                else if (strVerScope == "protected")
                {
                    verAccess = esriVersionAccess.esriVersionAccessProtected;
                }
            }

            pJob.VersionName = strVersionName;
            pNewVersion = pJob.CreateVersion(verAccess);

            if (pNewVersion == null)
            {
                throw (new System.SystemException("Unable to create version"));
            }

            IPropertySet pOverrides = new PropertySetClass();
            pOverrides.SetProperty("[VERSION]", pNewVersion.VersionName);
            IJTXActivityType pActType = m_ipDatabase.ConfigurationManager.GetActivityType(Constants.ACTTYPE_CREATE_VERSION);
            if (pActType != null)
                pJob.LogJobAction(pActType, pOverrides, "");

            JTXUtilities.SendNotification(Constants.NOTIF_VERSION_CREATED, m_ipDatabase, pJob, pOverrides);

            System.Runtime.InteropServices.Marshal.ReleaseComObject(pNewVersion);

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
            return StepUtilities.AreArgumentNamesValid(ref argv, m_expectedArgs);
        }

        #endregion

    }	// End Class
}	// End Namespace
