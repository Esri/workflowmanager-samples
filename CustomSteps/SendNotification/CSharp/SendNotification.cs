using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("694314bc-5964-43b8-bf12-c228ba33f5f4")]
    public class SendNotification : IJTXCustomStep
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
        private readonly string[] m_expectedArgs = { "notifType", "subscribers" };
        private IJTXNotificationType m_ipNotifType = null;
        private IJTXNotificationConfiguration m_ipNotifConfig = null;
        private string[] sList; 

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
                sb.AppendLine(@"Notification Type:");
                sb.AppendFormat("\t/{0}:<type> (required)\r\n", m_expectedArgs[0]);
                sb.AppendLine(@"Additional subscribers:");
                sb.AppendFormat("\t/{0}:<comma delimited email list> (optional)\r\n", m_expectedArgs[1]);
                sb.AppendLine("");
                sb.AppendLine("Example: /notifType:Alert /subscribers:cjones@email.com,amiller@email.com");
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

            string strNotifType = "";
            string strSubscribers = "";

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strNotifType))
            {
                throw new ArgumentNullException(m_expectedArgs[0], string.Format("\nMissing the {0} parameter!", m_expectedArgs[0]));
            }
            bool bSubscribers = StepUtilities.GetArgument(ref argv, m_expectedArgs[1], true, out strSubscribers);

            IJTXJob ipJob = m_ipDatabase.JobManager.GetJob(jobID);

            if (bSubscribers)
                UpdateSubscriberList(strNotifType, strSubscribers, "add");

            JTXUtilities.SendNotification(strNotifType, m_ipDatabase, ipJob, null);

            if (bSubscribers)
                UpdateSubscriberList(strNotifType, strSubscribers, "remove");

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
            string strValue = "";

            if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strValue)) { return false; }
            return StepUtilities.AreArgumentNamesValid(ref argv, m_expectedArgs);
        }

        #endregion

        #region Helper Functions

        private void UpdateSubscriberList(string strNotifType, string strSubscribers, string sAddRemove)
        {
            try
            {
                if (sAddRemove.ToUpper() == "ADD")
                {
                    m_ipNotifConfig = m_ipDatabase.ConfigurationManager as IJTXNotificationConfiguration;
                    m_ipNotifType = m_ipNotifConfig.GetNotificationType(strNotifType);

                    sList = strSubscribers.Split(',');
                    for (int i = 0; i < sList.Length; i++)
                    {
                        m_ipNotifType.Subscribe(sList[i]);
                    }
                    m_ipNotifType.Store();
                }
                else
                {
                    for (int i = 0; i < sList.Length; i++)
                    {
                        m_ipNotifType.UnSubscribe(sList[i]);
                    }
                    m_ipNotifType.Store();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred updating subscriber list!: " + ex.Message);
            }
        }

        #endregion
    }	// End Class
}	// End Namespace
