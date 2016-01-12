using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("6653425E-9988-4E7D-9EDA-77373202D0C5")]
    public class CheckAOI : IJTXCustomStep
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

            // Verify whether an AOI has been defined for the current job

            try
            {
                // Get the current job
                IJTXJobManager pJobManager = m_ipDatabase.JobManager;
                IJTXJob2 pJob = pJobManager.GetJob(jobID) as IJTXJob2;

                // Check if AOI exists
                if (pJob.AOIExtent != null)
                {
                    // AOI exists
                    return 1;
                }
                else
                {
                    // AOI does not exist
                    return 2;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            throw new NotImplementedException("No edit dialog available for this step type");
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
