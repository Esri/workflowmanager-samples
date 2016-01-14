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
    public partial class ArgEditor : Form
    {
        private List<object> m_Arguments = new List<object>();
        private IJTXDatabase m_ipDatabase = null;
        private string[] m_expectedArgs;

        public ArgEditor(IJTXDatabase database, string[] expectedArgs)
        {
            InitializeComponent();
            m_ipDatabase = database;
            m_expectedArgs = expectedArgs;
        }

        public DialogResult ShowDialog(object[] argsIn, out object[] argsOut)
        {
            // Populate the combo boxes with the appropriate information
            IJTXConfiguration2 ipJTXConfig = m_ipDatabase.ConfigurationManager as IJTXConfiguration2;
            PopulateJobTypes(ipJTXConfig);
            PopulateUsers(ipJTXConfig);
            PopulateGroups(ipJTXConfig);

            // Populate the dialog with the existing argument information
            string strTemp = "";
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[0], true, out strTemp))
            {
                // Then the job type has been entered
                int iJobTypeID = Convert.ToInt32(strTemp);
                IJTXJobType ipJobType = ipJTXConfig.GetJobTypeByID(iJobTypeID);

                cmbJobTypes.SelectedItem = ipJobType.Name;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[1], true, out strTemp))
            {
                // Then a user group has been selected for the new job assignment
                chkGroup.Checked = true;
                chkUser.Checked = false;
                cmbUsers.Enabled = false;
                cmbGroups.SelectedItem = strTemp;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[2], true, out strTemp))
            {
                // Then a user has been selected for the new job assignment
                chkGroup.Checked = false;
                chkUser.Checked = true;
                cmbGroups.Enabled = false;
                cmbUsers.SelectedItem = strTemp;
            }

            // Show the dialog
            this.ShowDialog();

            argsOut = m_Arguments.ToArray();

            return DialogResult;
        }

        private void chkGroup_CheckedChanged(object sender, EventArgs e)
        {
            cmbGroups.Enabled = chkGroup.Checked;
            if (chkGroup.Checked & chkUser.Checked)
            {
                chkUser.Checked = false;
                cmbUsers.Enabled = false;
            }
        }

        private void chkUser_CheckedChanged(object sender, EventArgs e)
        {
            cmbUsers.Enabled = chkUser.Checked;
            if (chkGroup.Checked & chkUser.Checked)
            {
                chkGroup.Checked = false;
                cmbGroups.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbJobTypes.SelectedItem.ToString() != "")
            {
                IJTXConfiguration ipJTXConfig = m_ipDatabase.ConfigurationManager;
                IJTXJobType ipJobType = ipJTXConfig.GetJobType(cmbJobTypes.SelectedItem.ToString());
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[0], ipJobType.ID.ToString()));
            }

            if (chkGroup.Checked)
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[1], cmbGroups.SelectedItem.ToString()));
            else if (chkUser.Checked)
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[2], cmbUsers.SelectedItem.ToString()));

            DialogResult = DialogResult.OK;
            this.Hide();
        }

        #region Helper Functions
        private void PopulateGroups(IJTXConfiguration2 ipJTXConfig)
        {
            IJTXUserGroupSet ipGroups = ipJTXConfig.UserGroups;
            for (int i = 0; i < ipGroups.Count; i++)
            {
                cmbGroups.Items.Add(ipGroups.get_Item(i).Name);
            }
            cmbGroups.SelectedIndex = 0;
        }

        private void PopulateUsers(IJTXConfiguration2 ipJTXConfig)
        {
            IJTXUserSet ipUsers = ipJTXConfig.Users;
            for (int i = 0; i < ipUsers.Count; i++)
            {
                cmbUsers.Items.Add(ipUsers.get_Item(i).UserName);
            }
            cmbUsers.SelectedIndex = 0;
        }

        private void PopulateJobTypes(IJTXConfiguration2 ipJTXConfig)
        {
            IJTXJobTypeSet ipJobTypes = ipJTXConfig.JobTypes;
            for (int i = 0; i < ipJobTypes.Count; i++)
            {
                cmbJobTypes.Items.Add(ipJobTypes.get_Item(i).Name);
            }
            cmbJobTypes.SelectedIndex = 0;
        }
        #endregion

    }
}