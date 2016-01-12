using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using SpatialNotificationServiceWS.JTXServer;

namespace SpatialNotificationServiceWS
{
    public partial class SNService : ServiceBase
    {
        JTXSO_JTXServer m_jtxServer = new JTXSO_JTXServer();
        JTXCaller m_caller = new JTXCaller();

        System.Timers.Timer m_timer = new System.Timers.Timer();

        Dictionary<string, DateTime> m_workspaces = new Dictionary<string,DateTime>();

        public SNService()
        {
            InitializeComponent();

            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_timer_Elapsed);

            
        }

        void m_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (string dbId in m_workspaces.Keys)
            {
                DateTime dtCurrent = m_jtxServer.GetDatabaseTime(dbId, m_caller).ToUniversalTime();
                m_jtxServer.RunSpatialNotificationsOnHistory(dbId, m_workspaces[dbId], dtCurrent, Properties.Settings.Default.LogMatches, true, m_caller);
                m_workspaces[dbId] = dtCurrent;
            }
        }

        protected override void OnStart(string[] args)
        {
            
            m_caller.Username = Properties.Settings.Default.ConnectionUser;

            JTXDataWorkspaceName[] wsNames = m_jtxServer.GetAllDataWorkspaces(m_caller);
            foreach (JTXDataWorkspaceName wsName in wsNames)
            {
                DateTime dtCurrent = m_jtxServer.GetDatabaseTime(wsName.DatabaseID, m_caller).ToUniversalTime();
                m_workspaces.Add(wsName.DatabaseID, dtCurrent);
            }


            m_timer.Interval = Properties.Settings.Default.Interval * 1000;
            m_timer.Start();
        }

        protected override void OnStop()
        {
            m_timer.Stop();
        }
    }
}
