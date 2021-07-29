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
using System.Windows.Forms;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("7b902ebc-655b-49a8-9218-6282a3925c59")]
    public class CheckVersion : IJTXCustomStep
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
        public IJTXDatabase m_ipDatabase = null;

        #region IJTXCustomStep Members

        ////////////////////////////////////////////////////////////////////////
        // PROPERTY: ArgumentDescriptions
        public string ArgumentDescriptions
        {
            get { return "This step does not have any arguments"; }
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: Execute
        public int Execute(int jobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {

            // Verify whether the current job has a version or not

            // Get the current job
            IJTXJobManager pJobManager = m_ipDatabase.JobManager;
            IJTXJob2 pJob = pJobManager.GetJob(jobID) as IJTXJob2;

            // Check for a data workspace
            IJTXDataWorkspaceName pJobDataWorkspace = pJob.ActiveDatabase;
            if (pJobDataWorkspace == null)
            {
                MessageBox.Show("No data workspace selected for job");
                return 3;
            }

            // Check if version exists
            if (pJob.VersionExists())
            {
                MessageBox.Show("Job version has been defined");
                return 1;
            }
            else
            {
                MessageBox.Show("No version defined for job");
                return 2;
            }

        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: Invoke
        public void Invoke()
        {
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: InvokeEditor
        public object[] InvokeEditor(int hWndParent, object[] argIn)
        {
            MessageBox.Show("Not Implemented");
            return null;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: OnCreate
        public void OnCreate(IJTXDatabase ipDatabase)
        {
            m_ipDatabase = ipDatabase;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: ValidateArguments
        public bool ValidateArguments(ref object[] argv)
        {
            return true;
        }

        #endregion

    }	// End Class
}	// End Namespace
