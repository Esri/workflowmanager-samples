using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("595BA29F-FEA8-494c-81F4-9C348C6D791B")]
    public class SelectDataWorkspace : IJTXCustomStep
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
            // Present form to user for choosing the data workspace 
            //    they want to work with for the current job.

            JTXSamples.DataWorkspaceSelectorDialog frmDataWSSelector = new JTXSamples.DataWorkspaceSelectorDialog();
            frmDataWSSelector.StartPosition = FormStartPosition.CenterParent;

            // Populate the list of data workspaces configured for the JTX system
            IJTXDatabaseConnectionManager pDBConnManager = new JTXDatabaseConnectionManagerClass();
            IJTXDatabaseConnection pDBConnection = pDBConnManager.GetConnection(m_ipDatabase.Alias);

            for (int a = 0; a < pDBConnection.DataWorkspaceCount; a++)
            {
                IJTXWorkspaceConfiguration pDataWorkspace = pDBConnection.get_DataWorkspace(a);
                frmDataWSSelector.DWName = pDataWorkspace.Name;
            }
            
            // Pass some other information to the form
            frmDataWSSelector.JTXDB = m_ipDatabase;

            IJTXJobManager pJobManager = m_ipDatabase.JobManager;
            IJTXJob2 pJob = pJobManager.GetJob(jobID) as IJTXJob2;
            frmDataWSSelector.theJob = pJob;
            
            if (frmDataWSSelector.ShowDialog() == DialogResult.OK)
            {
                return 1;
            }

            return 0;
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
