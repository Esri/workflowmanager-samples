using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("48332923-86d6-48d9-870e-291145bf61da")]
    public class CloseJob : IJTXCustomStep
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

        #region IJTXCustomStep Members

        /// <summary>
        /// A description of the expected arguments for the step type.  This should
        /// include the syntax of the argument, whether or not it is required/optional, 
        /// and any return codes coming from the step type.
        /// </summary>
        public string ArgumentDescriptions
        {
            get { return "This step does not include an argument description"; }
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

            IJTXJob pJob = m_ipDatabase.JobManager.GetJob(JobID);

            //Automatic status assignment
            IJTXConfiguration pConfig = m_ipDatabase.ConfigurationManager;
            IJTXConfigurationProperties pConfigProps = (IJTXConfigurationProperties)pConfig;

            IJTXWorkflowConfiguration pWFConfig = (IJTXWorkflowConfiguration)pJob;
            IJTXWorkflowExecution pWFExec = (IJTXWorkflowExecution)pJob;

            int[] iSteps = pWFExec.GetCurrentSteps();
            if (iSteps.Length == 1)
            {
                if (pWFExec.IsLastStep(iSteps[0]))
                {
                    pWFExec.MoveNext(iSteps[0], (int)nullReturnType.null_return);
                }
            }

            pJob.Close();
            pJob.EndDate = System.DateTime.Now;

            if (pConfigProps.PropertyExists(Constants.JTX_PROPERTY_AUTO_STATUS_ASSIGN))
            {
                string strAutoAssign = pConfigProps.GetProperty(Constants.JTX_PROPERTY_AUTO_STATUS_ASSIGN);
                if (strAutoAssign == "TRUE")
                {
                    pJob.Status = m_ipDatabase.ConfigurationManager.GetStatus("Closed");
                }
            }

            pJob.Store();

            IJTXActivityType pActType = pConfig.GetActivityType("CloseJob");
            if (pActType != null)
            {
                pJob.LogJobAction(pActType, null, "");
            }

            JTXUtilities.SendNotification(Constants.NOTIF_JOB_CLOSED, m_ipDatabase, pJob, null);

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
            return argv.Length == 0;
        }

        #endregion

        #region HelperMethods

        private bool JTXSystemConfigPropertyExists(object p)
        {
            throw new Exception("The method or operation is not implemented.");
        }




        #endregion


    }	// End Class
}	// End Namespace
