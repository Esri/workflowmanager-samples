using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.Geodatabase;

namespace SpatialNotificationService
{
    public partial class Service1 : ServiceBase
    {
        private IJTXDatabase2 m_database;
        private System.Timers.Timer m_timer = new System.Timers.Timer();
        private DateTime m_lastDate;

        private IJTXDataWorkspaceNameSet m_dataWorkspaceNames;
        private IJTXRegisteredLayerInfoSet m_regLayers;

        private List<WorkspaceInfo> m_workspaces = new List<WorkspaceInfo>();

        private class WorkspaceInfo
        {
            public WorkspaceInfo(string wsID, IWorkspace ws, DateTime time)
            {
                workspaceID = wsID;
                workspace = ws;
                lastProcessed = time;
            }

            public string workspaceID;
            public IWorkspace workspace;
            public DateTime lastProcessed;
        }

        public Service1()
        {
            InitializeComponent();

            m_timer.Interval = 15 * 1000;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_timer_Elapsed);
            m_timer.Enabled = false;

        }

        void m_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < m_workspaces.Count; i++)
            {
                for(int j = 0; j < m_regLayers.Count; j++)
                {
                    IJTXRegisteredLayerInfo2 regLayerInfo = (IJTXRegisteredLayerInfo2) m_regLayers.get_Item(j);
                    if(regLayerInfo.DataWorkspaceID == m_workspaces[i].workspaceID)
                    {
                        IFeatureWorkspace featWS = (IFeatureWorkspace)m_workspaces[i].workspace;
                        ITable table = featWS.OpenTable(regLayerInfo.LongName);

                        ProcessObjectClass(m_workspaces[i], (IObjectClass)table);
                    }
                }
                
            }
        }

        private void ProcessObjectClass(WorkspaceInfo wsInfo, IObjectClass objClass)
        {
            IJTXGDBHistoryChanges gdbHistoryChanges = new JTXGDBHistoryChangesClass();
            DateTime now = (DateTime)((IDatabaseConnectionInfo2) wsInfo.workspace).ConnectionCurrentDateTime;
            IJTXChangeSet changeSet = gdbHistoryChanges.GetChanges(m_database, wsInfo.workspaceID, objClass, wsInfo.lastProcessed, now);
            wsInfo.lastProcessed = now;

            if(changeSet.Count > 0)
            {
                m_database.LogMessage(5, 2000, "Changes found");
            }

            IJTXSpatialNotificationManager snMan = m_database.SpatialNotificationManager;
            bool bHasMatches;
            IJTXChangeRuleMatchSet matches = snMan.EvaluateSet(null, changeSet, out bHasMatches);

            if (bHasMatches)
            {
                m_database.LogMessage(5, 2000, "Matches found");
                snMan.Notify(matches);
            }

        }

        private void ProcessWorkspace(IWorkspace workspace)
        {

            
            
        }

        protected override void OnStart(string[] args)
        {
            bool isBound = ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
            if (!isBound)
                return;

            IAoInitialize aoInitialize = new AoInitializeClass();
            aoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
            aoInitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeWorkflowManager);

            IJTXDatabaseManager databaseMan = new JTXDatabaseManagerClass();
            m_database = (IJTXDatabase2)databaseMan.GetActiveDatabase(false);

            //m_lastDate = (DateTime.Now;

            m_dataWorkspaceNames = m_database.GetDataWorkspaceNames(null);
            
            IJTXSpatialNotificationManager spatNotifMan = m_database.SpatialNotificationManager;
            m_regLayers = spatNotifMan.RegisteredLayerInfos;

            for (int i = 0; i < m_dataWorkspaceNames.Count; i++)
            {
                IWorkspace ws = m_database.GetDataWorkspace(m_dataWorkspaceNames.get_Item(i).DatabaseID, "");
                DateTime now = (DateTime)((IDatabaseConnectionInfo2)ws).ConnectionCurrentDateTime;
                m_workspaces.Add(new WorkspaceInfo(m_dataWorkspaceNames.get_Item(i).DatabaseID, ws, now));
            }

            m_timer.Enabled = true;
            
        }

        protected override void OnStop()
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_database);
        }
    }
}
