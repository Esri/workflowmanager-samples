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
    partial class AttachmentTypeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttachmentTypeDialog));
            this.groupBox1_ = new System.Windows.Forms.GroupBox();
            this.noneBtn_ = new System.Windows.Forms.RadioButton();
            this.embeddedBtn_ = new System.Windows.Forms.RadioButton();
            this.linkedBtn_ = new System.Windows.Forms.RadioButton();
            this.label1_ = new System.Windows.Forms.Label();
            this.continueBtn_ = new System.Windows.Forms.Button();
            this.attachToParentJobCb_ = new System.Windows.Forms.CheckBox();
            this.attachToCurrentJobCb_ = new System.Windows.Forms.CheckBox();
            this.groupBox1_.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1_
            // 
            this.groupBox1_.Controls.Add(this.noneBtn_);
            this.groupBox1_.Controls.Add(this.embeddedBtn_);
            this.groupBox1_.Controls.Add(this.linkedBtn_);
            resources.ApplyResources(this.groupBox1_, "groupBox1_");
            this.groupBox1_.Name = "groupBox1_";
            this.groupBox1_.TabStop = false;
            // 
            // noneBtn_
            // 
            this.noneBtn_.Checked = true;
            resources.ApplyResources(this.noneBtn_, "noneBtn_");
            this.noneBtn_.Name = "noneBtn_";
            this.noneBtn_.TabStop = true;
            this.noneBtn_.UseVisualStyleBackColor = true;
            // 
            // embeddedBtn_
            // 
            resources.ApplyResources(this.embeddedBtn_, "embeddedBtn_");
            this.embeddedBtn_.Name = "embeddedBtn_";
            this.embeddedBtn_.UseVisualStyleBackColor = true;
            // 
            // linkedBtn_
            // 
            resources.ApplyResources(this.linkedBtn_, "linkedBtn_");
            this.linkedBtn_.Name = "linkedBtn_";
            this.linkedBtn_.UseVisualStyleBackColor = true;
            // 
            // label1_
            // 
            resources.ApplyResources(this.label1_, "label1_");
            this.label1_.Name = "label1_";
            // 
            // continueBtn_
            // 
            resources.ApplyResources(this.continueBtn_, "continueBtn_");
            this.continueBtn_.Name = "continueBtn_";
            this.continueBtn_.UseVisualStyleBackColor = true;
            this.continueBtn_.Click += new System.EventHandler(this.continueBtn__Click);
            // 
            // attachToParentJobCb_
            // 
            resources.ApplyResources(this.attachToParentJobCb_, "attachToParentJobCb_");
            this.attachToParentJobCb_.Name = "attachToParentJobCb_";
            this.attachToParentJobCb_.UseVisualStyleBackColor = true;
            // 
            // attachToCurrentJobCb_
            // 
            resources.ApplyResources(this.attachToCurrentJobCb_, "attachToCurrentJobCb_");
            this.attachToCurrentJobCb_.Checked = true;
            this.attachToCurrentJobCb_.CheckState = System.Windows.Forms.CheckState.Checked;
            this.attachToCurrentJobCb_.Name = "attachToCurrentJobCb_";
            this.attachToCurrentJobCb_.UseVisualStyleBackColor = true;
            // 
            // AttachmentTypeDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.attachToCurrentJobCb_);
            this.Controls.Add(this.attachToParentJobCb_);
            this.Controls.Add(this.continueBtn_);
            this.Controls.Add(this.label1_);
            this.Controls.Add(this.groupBox1_);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttachmentTypeDialog";
            this.groupBox1_.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1_;
        private System.Windows.Forms.RadioButton embeddedBtn_;
        private System.Windows.Forms.RadioButton linkedBtn_;
        private System.Windows.Forms.Label label1_;
        private System.Windows.Forms.Button continueBtn_;
        private System.Windows.Forms.RadioButton noneBtn_;
        private System.Windows.Forms.CheckBox attachToParentJobCb_;
        private System.Windows.Forms.CheckBox attachToCurrentJobCb_;
    }
}