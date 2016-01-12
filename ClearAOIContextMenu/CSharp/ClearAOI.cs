using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;
using ESRI.ArcGIS.JTXUI;
using ESRI.ArcGIS.esriSystem;

namespace JTXSamples
{
    class ClearAOI : IJTXToolbarButton
    {
        IJTXApplication m_pApplication = null;

        public void Attach(object initData)
        {
            if (initData is IJTXApplication)
                m_pApplication = (IJTXApplication)initData;
        }

        public void OnClick()
        {
            // Confirm the user wants to do this
            DialogResult result = MessageBox.Show("Are you sure you want to clear the AOI from the selected jobs?", "Clear AOI", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                // Get the job set from the list of jobs
                IJTXJobViewPanel pJobViewPanel = ((IJTXJobViewPanel)m_pApplication.CurrentView.ViewPanel);
                List<IJTXJob> pSelectedJobs = pJobViewPanel.SelectedJobs;
                foreach (IJTXJob pJob in pSelectedJobs)
                {
                    pJob.AOIExtent = null;
                    pJob.Store();

                    // Update the activity log and send notifications
                    IJTXConfiguration pConfig = m_pApplication.CurrentDatabase.ConfigurationManager;
                    IJTXActivityType pActType = pConfig.GetActivityType(Constants.ACTTYPE_UPDATE_AOI);
                    if (pActType != null)
                    {
                        pJob.LogJobAction(pActType, null, "");
                    }
                    JTXUtilities.SendNotification(Constants.NOTIF_AOI_UPDATED, m_pApplication.CurrentDatabase, pJob, null);
                }

                // If only 1 job is selected, refresh the panels
                if (pSelectedJobs.Count == 1)
                {
                    foreach (IJTXJobPanel ipJobPanel in pJobViewPanel.JobPanels)
                    {
                        ipJobPanel.RefreshJob();
                    }
                }

                MessageBox.Show("Cleared the AOI from selected jobs");
            }
            
        }

        public bool Enabled
        {
            get
            {
                if (m_pApplication.CurrentView is IJTXJobView)
                {
                    List<IJTXJob> pSelectedJobs = ((IJTXJobViewPanel)m_pApplication.CurrentView.ViewPanel).SelectedJobs;
                    if (pSelectedJobs != null && pSelectedJobs.Count > 0)
                        return true;
                }
                return false;
            }
        }

        #region IJTXToolbarItem Members


        public string Caption
        {
            get { return "Clear AOI"; }
        }

        public System.Drawing.Image Icon
        {
            get { return null; }
        }

        public string Name
        {
            get { return "JTXSamples.ClearAOI"; }
        }

        public string ToolTip
        {
            get { return "Clear the Area of Interest from all selected jobs"; }
        }

        public bool Visible
        {
            get { return true; }
        }

        #endregion
    }
}
