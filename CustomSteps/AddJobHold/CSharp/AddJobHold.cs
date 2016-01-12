using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("38ea681d-6975-4e8b-8f0f-8e678ca86c05")]
    public class AddJobHold : IJTXCustomStep
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
                sb.AppendLine(@"What type of hold are you adding?");
                sb.AppendFormat("\t/{0}: The name of the hold type to add\r\n", "HoldType");
                sb.AppendLine(@"   ex:   /HoldType:Resource");
                sb.AppendLine(@"");
                sb.AppendLine(@"Remarks");
                sb.AppendFormat("\t/{0}: The remarks that should be associated with this hold action\r\n", "HoldRemarks");
                sb.AppendLine(@"   ex:   /HoldRemarks:""Job on hold till adequate resources identified""");
                sb.AppendLine(@"");
                sb.AppendLine(@"Possible return codes: ");
                sb.AppendLine(@"  -1: Step failed");
                sb.AppendLine(@"   0: Successfully executed");

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

            // Get the arguments
            string sHoldTypeName = "";
            string sHoldRemarks = "";

            bool bHoldType = StepUtilities.GetArgument(ref argv, "HoldType", true, out sHoldTypeName);
            if (!bHoldType)
            {
                MessageBox.Show("Invalid arguments entered. No hold type entered.");
                return -1;
            }
            bool bHoldRemarks = StepUtilities.GetArgument(ref argv, "HoldRemarks", true, out sHoldRemarks);

            // Get the hold type
            IJTXConfiguration pJTXConfig = m_ipDatabase.ConfigurationManager;
            IJTXHoldType pHoldType = pJTXConfig.GetHoldType(sHoldTypeName);
            if (pHoldType == null)
            {
                MessageBox.Show("Invalid hold type name entered: " + sHoldTypeName);
                return -1;
            }

            // Get the job
            IJTXJobManager pJobManager = m_ipDatabase.JobManager;
            IJTXJob pJob = pJobManager.GetJob(jobID);
            IJTXJobHolds pJobHolds = pJob as IJTXJobHolds;

            // Add new job hold
            IJTXJobHold pNewHold = pJobHolds.CreateHold(pHoldType);
            if (bHoldRemarks)
            {
                pNewHold.HoldComments = sHoldRemarks;
                pNewHold.Store();
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
            IJTXDatabase pJTXDB = m_ipDatabase;

            // Get the arguments
            string sHoldTypeName = "";
            string sHoldRemarks = "";

            bool bHoldType = StepUtilities.GetArgument(ref argv, "HoldType", true, out sHoldTypeName);
            if (!bHoldType)
            {
                MessageBox.Show("Invalid arguments entered. No hold type entered.");
                return false;
            }
            bool bHoldRemarks = StepUtilities.GetArgument(ref argv, "HoldRemarks", true, out sHoldRemarks);

            // Get the hold type
            IJTXConfiguration pJTXConfig = pJTXDB.ConfigurationManager;
            IJTXHoldType pHoldType = pJTXConfig.GetHoldType(sHoldTypeName);
            if (pHoldType == null)
            {
                MessageBox.Show("Invalid hold type name entered: " + sHoldTypeName);
                return false;
            }
            return true;
        }

        #endregion

    }	// End Class
}	// End Namespace
