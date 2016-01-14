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
using System.Windows.Forms;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    public partial class CreateChildJobsArgEditor : Form
    {
        private List<object> m_Arguments = new List<object>();
        private IJTXDatabase m_ipDatabase = null;
        private string[] m_expectedArgs;
        
        public CreateChildJobsArgEditor(IJTXDatabase database, string[] expectedArgs)
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
            PopulateStatusTypes(ipJTXConfig);

            // Populate the dialog with the existing argument information
            string strTemp = "";
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[0], true, out strTemp))
            {
                // Then the job type has been entered
                IJTXJobType ipJobType = ipJTXConfig.GetJobType(strTemp);
                
                if (ipJobType != null)
                    cmbJobTypes.SelectedItem = ipJobType.Name;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[1], true, out strTemp))
            {
                // Then a user group has been selected for the new job assignment
                chkGroup.Checked = true;
                chkUser.Checked = false;
                cmbUsers.Enabled = false;
                int idx = cmbGroups.Items.IndexOf(strTemp);
                cmbGroups.SelectedIndex = idx;
                if (idx < 0)
                    cmbGroups.Text = strTemp;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[2], true, out strTemp))
            {
                // Then a user has been selected for the new job assignment
                chkGroup.Checked = false;
                chkUser.Checked = true;
                cmbGroups.Enabled = false;
                int idx = cmbUsers.Items.IndexOf(strTemp);
                cmbUsers.SelectedIndex = idx;
                if (idx < 0)
                    cmbUsers.Text = strTemp;
            }
            
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[3], true, out strTemp))
            {
                // Then a dependency is being created ... 
                chkDependThisStep.Checked = true;
                lblStatus.Enabled = true;
                cboDependentStatus.Enabled = true;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[4], true, out strTemp))
            {
                // Then a dependency is being created ... 
                chkDependNextStep.Checked = true;
                lblStatus.Enabled = true;
                cboDependentStatus.Enabled = true;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[5], true, out strTemp))
            {
                // Then a user has been selected for the new job assignment
                lblStatus.Enabled = true;
                lblStatus.Enabled = true;
                cboDependentStatus.SelectedItem = strTemp;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[6], true, out strTemp))
            {
                // Then the parent job's AOI will be used by the child
                chkAssignParentAOIToChild.Checked = true;
            }
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[7], true, out strTemp))
            {
                // Then the number of child jobs and their AOIs will be determined by a
                // spatial overlap with an input feature class
                chkAssignParentAOIToChild.Enabled = false;
                
                radioButton_generateNumberJobs.Checked = true;
                txtAOIFeatureClassName.Enabled = true;
                txtAOIFeatureClassName.Text = strTemp;
            }
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[8], true, out strTemp))
            {
                // Then the user will specify the number of child jobs
                txtAOIFeatureClassName.Enabled = false;

                radioButton_DefineNumberOfJobs.Checked = true;
                lstNumberOfJobs.Value = Convert.ToDecimal(strTemp);
                chkAssignParentAOIToChild.Enabled = true;
            }
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[9], true, out strTemp))
            {
                // Then a version will be created for the child job(s) from the Default version
                chkCreateVersion.Checked = true;
                cboCreateVersionSetting.Enabled = true;
                int idx = cboCreateVersionSetting.Items.IndexOf(strTemp);
                cboCreateVersionSetting.SelectedIndex = idx;
                if (idx < 0)
                    cboCreateVersionSetting.Text = strTemp;
            }
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[10], true, out strTemp))
            {
                // Then a version will be created for the child job(s) from the Default version
                chkAssignVersion.Checked = true;
                cboAssignVersionSetting.Enabled = true;
                int idx = cboAssignVersionSetting.Items.IndexOf(strTemp);
                cboAssignVersionSetting.SelectedIndex = idx;
                if (idx < 0)
                    cboAssignVersionSetting.Text = strTemp;
            }
            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[11], true, out strTemp))
            {
                // Then the user will not be able to modify any parameters that were pre-configured
                chkSetExtProps.Checked = true;
                txtSetExtProps.Text = strTemp;
            }

            if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[12], true, out strTemp))
            {
                chkDueDate.Checked = true;
                dtpDueDate.Value = JTXUtilities.GenerateDateString(m_ipDatabase.JTXWorkspace, strTemp, false);
            }
            else if (StepUtilities.GetArgument(ref argsIn, m_expectedArgs[13], true, out strTemp))
                txtDuration.Text = strTemp;
                        
            // Show the dialog
            this.ShowDialog();

            argsOut = m_Arguments.ToArray();

            return DialogResult;
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
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[0], cmbJobTypes.SelectedItem.ToString()));
            }

            if (chkGroup.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[1], cmbGroups.Text));
            }
            else if (chkUser.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[2], cmbUsers.Text));
            }

            if (chkDependThisStep.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateFlagArgument(m_expectedArgs[3]));
            }
            if (chkDependNextStep.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateFlagArgument(m_expectedArgs[4]));
            }
            if (chkDependThisStep.Checked || chkDependNextStep.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[5], cboDependentStatus.SelectedItem.ToString()));
            }
            if (radioButton_DefineNumberOfJobs.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[8], lstNumberOfJobs.Value.ToString()));
                if (chkAssignParentAOIToChild.Checked)
                {
                    m_Arguments.Add(StepUtilities.CreateFlagArgument(m_expectedArgs[6]));
                }
            }
            if (radioButton_generateNumberJobs.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[7], txtAOIFeatureClassName.Text));
            }
            if (chkCreateVersion.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[9], cboCreateVersionSetting.Text));               
            }
            if (chkAssignVersion.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[10], cboAssignVersionSetting.Text));
            }
            if (chkSetExtProps.Checked)
            {
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[11], txtSetExtProps.Text));
            }

            // Deal with the date options
            if (chkDueDate.Checked)
            {
                string strDate = JTXUtilities.GenerateDBMSDate(m_ipDatabase.JTXWorkspace, dtpDueDate.Value);
                m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[12], strDate));
            }
            else if (!String.IsNullOrEmpty(txtDuration.Text))
            {
                int duration = -1;
                // Make sure the duration is a positive number 
                int.TryParse(txtDuration.Text, out duration);
                if (duration > 0)
                    m_Arguments.Add(StepUtilities.CreateSingleArgument(m_expectedArgs[13], txtDuration.Text));
            }

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
        private void PopulateStatusTypes(IJTXConfiguration2 ipJTXConfig)
        {
            IJTXStatusSet ipStatuses = ipJTXConfig.Statuses;
            for (int i = 0; i < ipStatuses.Count; i++)
            {
                cboDependentStatus.Items.Add(ipStatuses.get_Item(i).Name);
            }
            cboDependentStatus.SelectedIndex = 0;
        }
        #endregion

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

        private void chkDependThisStep_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDependThisStep.Checked)
            {
                chkDependNextStep.Checked = false;
                lblStatus.Enabled = true;
                cboDependentStatus.Enabled = true;
            }
            else
            {
                if (!chkDependNextStep.Checked)
                {
                    lblStatus.Enabled = false;
                    cboDependentStatus.Enabled = false;
                }
            }
        }

        private void chkDependNextStep_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDependNextStep.Checked)
            {
                chkDependThisStep.Checked = false;
                lblStatus.Enabled = true;
                cboDependentStatus.Enabled = true;
            }
            else
            {
                if (!chkDependThisStep.Checked)
                {
                    lblStatus.Enabled = false;
                    cboDependentStatus.Enabled = false;
                }
            }
        }

        private void chkSetExtProps_CheckedChanged(object sender, EventArgs e)
        {
            txtSetExtProps.Enabled = chkSetExtProps.Checked;
        }

        private void radioButton_DefineNumberOfJobs_CheckedChanged(object sender, EventArgs e)
        {
            txtAOIFeatureClassName.Enabled = !radioButton_DefineNumberOfJobs.Checked;
            
            lstNumberOfJobs.Enabled = radioButton_DefineNumberOfJobs.Checked;
            chkAssignParentAOIToChild.Enabled = radioButton_DefineNumberOfJobs.Checked;            
        }

        private void radioButton_generateNumberJobs_CheckedChanged(object sender, EventArgs e)
        {
            txtAOIFeatureClassName.Enabled = !radioButton_DefineNumberOfJobs.Checked;

            lstNumberOfJobs.Enabled = radioButton_DefineNumberOfJobs.Checked;
            chkAssignParentAOIToChild.Enabled = radioButton_DefineNumberOfJobs.Checked;
        }

        private void chkCreateVersion_CheckedChanged(object sender, EventArgs e)
        {
            lblCreateVersion.Enabled = chkCreateVersion.Checked;
            cboCreateVersionSetting.Enabled = chkCreateVersion.Checked;

            if (chkCreateVersion.Checked)
            {
                chkAssignVersion.Checked = false;
                if (cboCreateVersionSetting.SelectedIndex < 0)
                    cboCreateVersionSetting.SelectedIndex = 0;
            }
        }        

        private void chkAssignVersion_CheckedChanged(object sender, EventArgs e)
        {
            cboAssignVersionSetting.Enabled = chkAssignVersion.Checked;

            if (chkAssignVersion.Checked)
            {
                chkCreateVersion.Checked = false;
                if (cboAssignVersionSetting.SelectedIndex < 0)
                    cboAssignVersionSetting.SelectedIndex = 0;
            }
            
        }

        private void chkDueDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpDueDate.Enabled = chkDueDate.Checked;
            txtDuration.Enabled = !chkDueDate.Checked;
			if( chkDueDate.Checked )
				txtDuration.Text = "";
            //else
            //    dtpDueDate.Value = Constants.NullDate;

			dtpDueDate.Enabled = chkDueDate.Checked;
        }
    }
}