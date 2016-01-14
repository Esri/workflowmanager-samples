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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    public partial class DataWorkspaceSelectorDialog : Form
    {
        IJTXDatabase m_pJTXDatabase = null;
        IJTXJob2 m_pJob = null;

        public DataWorkspaceSelectorDialog()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        public string DWName
        {
            set { this.cboDWList.Items.Add(value); }
        }
        public IJTXDatabase JTXDB
        {
            set { m_pJTXDatabase = value; }
        }
        public IJTXJob2 theJob
        {
            set { m_pJob = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Set the active data workspace for the job to the one the user selected.
            IJTXDatabaseConnectionManager pDBConnManager = new JTXDatabaseConnectionManagerClass();
            IJTXDatabaseConnection pDBConnection = pDBConnManager.GetConnection(m_pJTXDatabase.Alias);
            
            IJTXWorkspaceConfiguration pSelectedDW = pDBConnection.get_DataWorkspace(cboDWList.SelectedIndex);

            m_pJob.SetActiveDatabase(pSelectedDW.DatabaseID);
            m_pJob.Store();

            DialogResult = DialogResult.OK;
            this.Hide();
        }

    }

}