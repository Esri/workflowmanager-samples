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
namespace JTXSamples
{
    partial class CreateChildJobsArgEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbJobTypes = new System.Windows.Forms.ComboBox();
            this.cmbGroups = new System.Windows.Forms.ComboBox();
            this.cmbUsers = new System.Windows.Forms.ComboBox();
            this.chkGroup = new System.Windows.Forms.CheckBox();
            this.chkUser = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbcTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpDueDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.chkDueDate = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDuration = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grpExtProps = new System.Windows.Forms.GroupBox();
            this.txtSetExtProps = new System.Windows.Forms.TextBox();
            this.chkSetExtProps = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkDependThisStep = new System.Windows.Forms.CheckBox();
            this.cboDependentStatus = new System.Windows.Forms.ComboBox();
            this.chkDependNextStep = new System.Windows.Forms.CheckBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton_generateNumberJobs = new System.Windows.Forms.RadioButton();
            this.lstNumberOfJobs = new System.Windows.Forms.NumericUpDown();
            this.chkAssignParentAOIToChild = new System.Windows.Forms.CheckBox();
            this.radioButton_DefineNumberOfJobs = new System.Windows.Forms.RadioButton();
            this.txtAOIFeatureClassName = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkCreateVersion = new System.Windows.Forms.CheckBox();
            this.cboAssignVersionSetting = new System.Windows.Forms.ComboBox();
            this.cboCreateVersionSetting = new System.Windows.Forms.ComboBox();
            this.chkAssignVersion = new System.Windows.Forms.CheckBox();
            this.lblCreateVersion = new System.Windows.Forms.Label();
            this.tbcTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpExtProps.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstNumberOfJobs)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbJobTypes
            // 
            this.cmbJobTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJobTypes.FormattingEnabled = true;
            this.cmbJobTypes.Location = new System.Drawing.Point(9, 43);
            this.cmbJobTypes.Name = "cmbJobTypes";
            this.cmbJobTypes.Size = new System.Drawing.Size(318, 21);
            this.cmbJobTypes.TabIndex = 0;
            // 
            // cmbGroups
            // 
            this.cmbGroups.Enabled = false;
            this.cmbGroups.FormattingEnabled = true;
            this.cmbGroups.Location = new System.Drawing.Point(8, 95);
            this.cmbGroups.Name = "cmbGroups";
            this.cmbGroups.Size = new System.Drawing.Size(319, 21);
            this.cmbGroups.TabIndex = 1;
            // 
            // cmbUsers
            // 
            this.cmbUsers.Enabled = false;
            this.cmbUsers.FormattingEnabled = true;
            this.cmbUsers.Location = new System.Drawing.Point(9, 149);
            this.cmbUsers.Name = "cmbUsers";
            this.cmbUsers.Size = new System.Drawing.Size(318, 21);
            this.cmbUsers.TabIndex = 2;
            // 
            // chkGroup
            // 
            this.chkGroup.AutoSize = true;
            this.chkGroup.Location = new System.Drawing.Point(9, 72);
            this.chkGroup.Name = "chkGroup";
            this.chkGroup.Size = new System.Drawing.Size(142, 17);
            this.chkGroup.TabIndex = 3;
            this.chkGroup.Text = "Assign new job to group:";
            this.chkGroup.UseVisualStyleBackColor = true;
            this.chkGroup.CheckedChanged += new System.EventHandler(this.chkGroup_CheckedChanged);
            // 
            // chkUser
            // 
            this.chkUser.AutoSize = true;
            this.chkUser.Location = new System.Drawing.Point(9, 126);
            this.chkUser.Name = "chkUser";
            this.chkUser.Size = new System.Drawing.Size(135, 17);
            this.chkUser.TabIndex = 4;
            this.chkUser.Text = "Assign new job to user:";
            this.chkUser.UseVisualStyleBackColor = true;
            this.chkUser.CheckedChanged += new System.EventHandler(this.chkUser_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(113, 428);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(194, 428);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Select job type for new job:";
            // 
            // tbcTabs
            // 
            this.tbcTabs.Controls.Add(this.tabPage1);
            this.tbcTabs.Controls.Add(this.tabPage2);
            this.tbcTabs.Controls.Add(this.tabPage3);
            this.tbcTabs.Controls.Add(this.tabPage4);
            this.tbcTabs.Location = new System.Drawing.Point(12, 12);
            this.tbcTabs.Name = "tbcTabs";
            this.tbcTabs.SelectedIndex = 0;
            this.tbcTabs.Size = new System.Drawing.Size(359, 410);
            this.tbcTabs.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.grpExtProps);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(351, 384);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Properties";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpDueDate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkDueDate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtDuration);
            this.groupBox1.Location = new System.Drawing.Point(7, 197);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 81);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Due Date";
            // 
            // dtpDueDate
            // 
            this.dtpDueDate.Enabled = false;
            this.dtpDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDueDate.Location = new System.Drawing.Point(90, 45);
            this.dtpDueDate.Name = "dtpDueDate";
            this.dtpDueDate.Size = new System.Drawing.Size(135, 20);
            this.dtpDueDate.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Due Date";
            // 
            // chkDueDate
            // 
            this.chkDueDate.AutoSize = true;
            this.chkDueDate.Location = new System.Drawing.Point(69, 49);
            this.chkDueDate.Name = "chkDueDate";
            this.chkDueDate.Size = new System.Drawing.Size(15, 14);
            this.chkDueDate.TabIndex = 24;
            this.chkDueDate.UseVisualStyleBackColor = true;
            this.chkDueDate.CheckedChanged += new System.EventHandler(this.chkDueDate_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(196, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "days";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Job Duration";
            // 
            // txtDuration
            // 
            this.txtDuration.Location = new System.Drawing.Point(90, 19);
            this.txtDuration.Name = "txtDuration";
            this.txtDuration.Size = new System.Drawing.Size(100, 20);
            this.txtDuration.TabIndex = 21;
            this.txtDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cmbJobTypes);
            this.groupBox2.Controls.Add(this.cmbGroups);
            this.groupBox2.Controls.Add(this.cmbUsers);
            this.groupBox2.Controls.Add(this.chkGroup);
            this.groupBox2.Controls.Add(this.chkUser);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(338, 185);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Basic Properties";
            // 
            // grpExtProps
            // 
            this.grpExtProps.Controls.Add(this.txtSetExtProps);
            this.grpExtProps.Controls.Add(this.chkSetExtProps);
            this.grpExtProps.Location = new System.Drawing.Point(6, 284);
            this.grpExtProps.Name = "grpExtProps";
            this.grpExtProps.Size = new System.Drawing.Size(338, 60);
            this.grpExtProps.TabIndex = 20;
            this.grpExtProps.TabStop = false;
            this.grpExtProps.Text = "Extended Properties";
            // 
            // txtSetExtProps
            // 
            this.txtSetExtProps.Enabled = false;
            this.txtSetExtProps.Location = new System.Drawing.Point(62, 20);
            this.txtSetExtProps.Name = "txtSetExtProps";
            this.txtSetExtProps.Size = new System.Drawing.Size(264, 20);
            this.txtSetExtProps.TabIndex = 1;
            // 
            // chkSetExtProps
            // 
            this.chkSetExtProps.AutoSize = true;
            this.chkSetExtProps.Location = new System.Drawing.Point(6, 22);
            this.chkSetExtProps.Name = "chkSetExtProps";
            this.chkSetExtProps.Size = new System.Drawing.Size(60, 17);
            this.chkSetExtProps.TabIndex = 0;
            this.chkSetExtProps.Text = "Define:";
            this.chkSetExtProps.UseVisualStyleBackColor = true;
            this.chkSetExtProps.CheckedChanged += new System.EventHandler(this.chkSetExtProps_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(351, 384);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Dependencies";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkDependThisStep);
            this.groupBox3.Controls.Add(this.cboDependentStatus);
            this.groupBox3.Controls.Add(this.chkDependNextStep);
            this.groupBox3.Controls.Add(this.lblStatus);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(339, 125);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dependencies";
            // 
            // chkDependThisStep
            // 
            this.chkDependThisStep.AutoSize = true;
            this.chkDependThisStep.Location = new System.Drawing.Point(6, 19);
            this.chkDependThisStep.Name = "chkDependThisStep";
            this.chkDependThisStep.Size = new System.Drawing.Size(176, 17);
            this.chkDependThisStep.TabIndex = 12;
            this.chkDependThisStep.Text = "Create dependency on this step";
            this.chkDependThisStep.UseVisualStyleBackColor = true;
            this.chkDependThisStep.CheckedChanged += new System.EventHandler(this.chkDependThisStep_CheckedChanged);
            // 
            // cboDependentStatus
            // 
            this.cboDependentStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDependentStatus.Enabled = false;
            this.cboDependentStatus.FormattingEnabled = true;
            this.cboDependentStatus.Location = new System.Drawing.Point(9, 85);
            this.cboDependentStatus.Name = "cboDependentStatus";
            this.cboDependentStatus.Size = new System.Drawing.Size(266, 21);
            this.cboDependentStatus.TabIndex = 14;
            // 
            // chkDependNextStep
            // 
            this.chkDependNextStep.AutoSize = true;
            this.chkDependNextStep.Location = new System.Drawing.Point(6, 42);
            this.chkDependNextStep.Name = "chkDependNextStep";
            this.chkDependNextStep.Size = new System.Drawing.Size(180, 17);
            this.chkDependNextStep.TabIndex = 13;
            this.chkDependNextStep.Text = "Create dependency on next step";
            this.chkDependNextStep.UseVisualStyleBackColor = true;
            this.chkDependNextStep.CheckedChanged += new System.EventHandler(this.chkDependNextStep_CheckedChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Enabled = false;
            this.lblStatus.Location = new System.Drawing.Point(6, 69);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(269, 13);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "This job will be dependent on the child job\'s status type:";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(351, 384);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Number of Jobs";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton_generateNumberJobs);
            this.groupBox4.Controls.Add(this.lstNumberOfJobs);
            this.groupBox4.Controls.Add(this.chkAssignParentAOIToChild);
            this.groupBox4.Controls.Add(this.radioButton_DefineNumberOfJobs);
            this.groupBox4.Controls.Add(this.txtAOIFeatureClassName);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(339, 146);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Number of Jobs and AOI Definition";
            // 
            // radioButton_generateNumberJobs
            // 
            this.radioButton_generateNumberJobs.AutoSize = true;
            this.radioButton_generateNumberJobs.Location = new System.Drawing.Point(6, 83);
            this.radioButton_generateNumberJobs.Name = "radioButton_generateNumberJobs";
            this.radioButton_generateNumberJobs.Size = new System.Drawing.Size(251, 17);
            this.radioButton_generateNumberJobs.TabIndex = 9;
            this.radioButton_generateNumberJobs.TabStop = true;
            this.radioButton_generateNumberJobs.Text = "Overlap feature class to generate jobs and AOIs";
            this.radioButton_generateNumberJobs.UseVisualStyleBackColor = true;
            this.radioButton_generateNumberJobs.CheckedChanged += new System.EventHandler(this.radioButton_generateNumberJobs_CheckedChanged);
            // 
            // lstNumberOfJobs
            // 
            this.lstNumberOfJobs.Location = new System.Drawing.Point(202, 19);
            this.lstNumberOfJobs.Name = "lstNumberOfJobs";
            this.lstNumberOfJobs.Size = new System.Drawing.Size(70, 20);
            this.lstNumberOfJobs.TabIndex = 11;
            this.lstNumberOfJobs.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkAssignParentAOIToChild
            // 
            this.chkAssignParentAOIToChild.AutoSize = true;
            this.chkAssignParentAOIToChild.Location = new System.Drawing.Point(27, 48);
            this.chkAssignParentAOIToChild.Name = "chkAssignParentAOIToChild";
            this.chkAssignParentAOIToChild.Size = new System.Drawing.Size(200, 17);
            this.chkAssignParentAOIToChild.TabIndex = 7;
            this.chkAssignParentAOIToChild.Text = "Assign parent job\'s AOI to child job(s)";
            this.chkAssignParentAOIToChild.UseVisualStyleBackColor = true;
            // 
            // radioButton_DefineNumberOfJobs
            // 
            this.radioButton_DefineNumberOfJobs.AutoSize = true;
            this.radioButton_DefineNumberOfJobs.Location = new System.Drawing.Point(6, 19);
            this.radioButton_DefineNumberOfJobs.Name = "radioButton_DefineNumberOfJobs";
            this.radioButton_DefineNumberOfJobs.Size = new System.Drawing.Size(156, 17);
            this.radioButton_DefineNumberOfJobs.TabIndex = 10;
            this.radioButton_DefineNumberOfJobs.TabStop = true;
            this.radioButton_DefineNumberOfJobs.Text = "Define number of child jobs:";
            this.radioButton_DefineNumberOfJobs.UseVisualStyleBackColor = true;
            this.radioButton_DefineNumberOfJobs.CheckedChanged += new System.EventHandler(this.radioButton_DefineNumberOfJobs_CheckedChanged);
            // 
            // txtAOIFeatureClassName
            // 
            this.txtAOIFeatureClassName.Location = new System.Drawing.Point(27, 106);
            this.txtAOIFeatureClassName.Name = "txtAOIFeatureClassName";
            this.txtAOIFeatureClassName.Size = new System.Drawing.Size(246, 20);
            this.txtAOIFeatureClassName.TabIndex = 8;
            this.txtAOIFeatureClassName.Text = "<feature class name>";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(351, 384);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Versioning";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkCreateVersion);
            this.groupBox5.Controls.Add(this.cboAssignVersionSetting);
            this.groupBox5.Controls.Add(this.cboCreateVersionSetting);
            this.groupBox5.Controls.Add(this.chkAssignVersion);
            this.groupBox5.Controls.Add(this.lblCreateVersion);
            this.groupBox5.Location = new System.Drawing.Point(6, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(339, 151);
            this.groupBox5.TabIndex = 28;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Versioning";
            // 
            // chkCreateVersion
            // 
            this.chkCreateVersion.AutoSize = true;
            this.chkCreateVersion.Location = new System.Drawing.Point(6, 19);
            this.chkCreateVersion.Name = "chkCreateVersion";
            this.chkCreateVersion.Size = new System.Drawing.Size(174, 17);
            this.chkCreateVersion.TabIndex = 23;
            this.chkCreateVersion.Text = "Create a version for child job(s) ";
            this.chkCreateVersion.UseVisualStyleBackColor = true;
            this.chkCreateVersion.CheckedChanged += new System.EventHandler(this.chkCreateVersion_CheckedChanged);
            // 
            // cboAssignVersionSetting
            // 
            this.cboAssignVersionSetting.Enabled = false;
            this.cboAssignVersionSetting.FormattingEnabled = true;
            this.cboAssignVersionSetting.Items.AddRange(new object[] {
            "The parent job\'s version"});
            this.cboAssignVersionSetting.Location = new System.Drawing.Point(26, 109);
            this.cboAssignVersionSetting.Name = "cboAssignVersionSetting";
            this.cboAssignVersionSetting.Size = new System.Drawing.Size(249, 21);
            this.cboAssignVersionSetting.TabIndex = 27;
            this.cboAssignVersionSetting.Text = "The parent job\'s version";
            // 
            // cboCreateVersionSetting
            // 
            this.cboCreateVersionSetting.Enabled = false;
            this.cboCreateVersionSetting.FormattingEnabled = true;
            this.cboCreateVersionSetting.Items.AddRange(new object[] {
            "The parent job\'s version",
            "The parent job\'s parent version",
            "The parent job\'s DEFAULT version",
            "The job type\'s default properties parent version"});
            this.cboCreateVersionSetting.Location = new System.Drawing.Point(26, 53);
            this.cboCreateVersionSetting.Name = "cboCreateVersionSetting";
            this.cboCreateVersionSetting.Size = new System.Drawing.Size(250, 21);
            this.cboCreateVersionSetting.TabIndex = 24;
            this.cboCreateVersionSetting.Text = "The parent job\'s version";
            // 
            // chkAssignVersion
            // 
            this.chkAssignVersion.AutoSize = true;
            this.chkAssignVersion.Location = new System.Drawing.Point(6, 85);
            this.chkAssignVersion.Name = "chkAssignVersion";
            this.chkAssignVersion.Size = new System.Drawing.Size(237, 17);
            this.chkAssignVersion.TabIndex = 26;
            this.chkAssignVersion.Text = "Assign this existing version to the child job(s):";
            this.chkAssignVersion.UseVisualStyleBackColor = true;
            this.chkAssignVersion.CheckedChanged += new System.EventHandler(this.chkAssignVersion_CheckedChanged);
            // 
            // lblCreateVersion
            // 
            this.lblCreateVersion.AutoSize = true;
            this.lblCreateVersion.Location = new System.Drawing.Point(23, 35);
            this.lblCreateVersion.Name = "lblCreateVersion";
            this.lblCreateVersion.Size = new System.Drawing.Size(124, 13);
            this.lblCreateVersion.TabIndex = 25;
            this.lblCreateVersion.Text = "using this parent version:";
            // 
            // CreateChildJobsArgEditor
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(383, 463);
            this.Controls.Add(this.tbcTabs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CreateChildJobsArgEditor";
            this.Text = "Create Job Arguments";
            this.tbcTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grpExtProps.ResumeLayout(false);
            this.grpExtProps.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstNumberOfJobs)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbJobTypes;
        private System.Windows.Forms.ComboBox cmbGroups;
        private System.Windows.Forms.ComboBox cmbUsers;
        private System.Windows.Forms.CheckBox chkGroup;
        private System.Windows.Forms.CheckBox chkUser;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tbcTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox cboDependentStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkDependThisStep;
        private System.Windows.Forms.CheckBox chkDependNextStep;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NumericUpDown lstNumberOfJobs;
        private System.Windows.Forms.RadioButton radioButton_DefineNumberOfJobs;
        private System.Windows.Forms.RadioButton radioButton_generateNumberJobs;
        private System.Windows.Forms.TextBox txtAOIFeatureClassName;
        private System.Windows.Forms.CheckBox chkAssignParentAOIToChild;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ComboBox cboAssignVersionSetting;
        private System.Windows.Forms.CheckBox chkAssignVersion;
        private System.Windows.Forms.Label lblCreateVersion;
        private System.Windows.Forms.ComboBox cboCreateVersionSetting;
        private System.Windows.Forms.CheckBox chkCreateVersion;
        private System.Windows.Forms.GroupBox grpExtProps;
        private System.Windows.Forms.TextBox txtSetExtProps;
        private System.Windows.Forms.CheckBox chkSetExtProps;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpDueDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDueDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDuration;
    }
}