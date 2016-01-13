/*Copyright 2015 Esri
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.?*/
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
