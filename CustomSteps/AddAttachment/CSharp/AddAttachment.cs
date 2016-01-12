using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("BDAD8094-2BD4-43F1-AD61-049989E81B62")]
    public class AddAttachmentStep : IJTXCustomStep
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
        public int Execute(int jobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {
            int retVal = 0;

            // This is a little convoluted, but was set up this way to allow this function
            // potentially being called from within a loop.
            retVal = addAttachments(jobID, stepID, ref argv, ref ipFeedback);

            return retVal;
        }

        private int addAttachments(int jobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {
            // Get a handle to the job
            IJTXJobManager pJobManager = m_ipDatabase.JobManager;
            IJTXJob2 pJob = pJobManager.GetJob(jobID) as IJTXJob2;
            IJTXJob2 pParentJob = pJobManager.GetJob(pJob.ParentJob) as IJTXJob2;

            // Prompt the user to choose whether the attachment should be embedded
            // into the database or merely stored as a URL, UNC path, etc.
            AttachmentTypeDialog atd = new AttachmentTypeDialog();

            // Check to see if this job has a parent.  If so, enable the "attach to parent" dialog.
            if (pParentJob == null)
            {
                atd.EnableSelectParent = false;
            }
            else
            {
                atd.EnableSelectParent = true;
            }

            atd.ShowDialog();
            if (atd.isNoneSelected())
            {
                return 0;
            }

            // Set the file storage type, according to what the user selected.
            bool embedAttachment = atd.isEmbeddedSelected();
            jtxFileStorageType fileStorageType;
            if (embedAttachment)
            {
                fileStorageType = jtxFileStorageType.jtxStoreInDB;
            }
            else
            {
                fileStorageType = jtxFileStorageType.jtxStoreAsLink;
            }

            // Prompt the user to select the file(s) to be attached.
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.ShowDialog();

            // Add each file that the user selected.
            foreach (string filename in ofd.FileNames)
            {
                if (atd.AttachToCurrent)
                {
                    pJob.AddAttachment(filename, fileStorageType, "");
                }
                if (atd.AttachToParent)
                {
                    pParentJob.AddAttachment(filename, fileStorageType, "");
                }
            }

            return 1;
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
            return true;
        }

        #endregion

    }	// End Class
}	// End Namespace
