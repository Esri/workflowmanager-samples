using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("d8df0319-93b1-474e-bee0-6538da47115e")]
    public class CleanUp : IJTXCustomStep
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
        private readonly string[] m_expectedArgs = { "version", "v", "mxd", "m", "attachments", "a" };

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
                sb.AppendLine(@"Version Cleanup:");
                sb.AppendFormat("\t/{0}|v (optional)\r\n\r\n", m_expectedArgs[0]);
                sb.AppendLine(@"MXD Cleanup:");
                sb.AppendFormat("\t/{0}|m (optional)\r\n", m_expectedArgs[2]);
                sb.AppendLine(@"Attachment Cleanup:");
                sb.AppendFormat("\t/{0}|a (optional)\r\n", m_expectedArgs[4]);

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

            IJTXJob pJob = m_ipDatabase.JobManager.GetJob(jobID);
            string strValue = "";

            if (StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strValue) ||
                StepUtilities.GetArgument(ref argv, m_expectedArgs[1], true, out strValue))
            {
                if (pJob.VersionExists()) { pJob.DeleteVersion(null); }
            }

            if (StepUtilities.GetArgument(ref argv, m_expectedArgs[2], true, out strValue) ||
                StepUtilities.GetArgument(ref argv, m_expectedArgs[3], true, out strValue))
            {
                if (pJob.MXDExists()) { pJob.DeleteMXD(); }
            }

            if (StepUtilities.GetArgument(ref argv, m_expectedArgs[4], true, out strValue) ||
                StepUtilities.GetArgument(ref argv, m_expectedArgs[5], true, out strValue))
            {
                IJTXAttachmentSet attachments = pJob.Attachments;
                for (int i = attachments.Count - 1; i >= 0; i--)
                {
                    int id = (attachments.get_Item(i).ID);
                    pJob.DeleteAttachment(id);
                }
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
            return StepUtilities.AreArgumentNamesValid(ref argv, m_expectedArgs);
        }

        #endregion

    }	// End Class
}	// End Namespace
