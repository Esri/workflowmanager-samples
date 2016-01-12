namespace JTXSamples
{
    partial class StatusForm
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.m_waitLabel = new System.Windows.Forms.Label();
            this.m_toolNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(12, 32);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(117, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Executing GP Tool:";
            // 
            // m_waitLabel
            // 
            this.m_waitLabel.AutoSize = true;
            this.m_waitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_waitLabel.Location = new System.Drawing.Point(124, 73);
            this.m_waitLabel.Name = "m_waitLabel";
            this.m_waitLabel.Size = new System.Drawing.Size(75, 13);
            this.m_waitLabel.TabIndex = 1;
            this.m_waitLabel.Text = "Please Wait";
            // 
            // m_toolNameLabel
            // 
            this.m_toolNameLabel.AutoSize = true;
            this.m_toolNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_toolNameLabel.Location = new System.Drawing.Point(135, 32);
            this.m_toolNameLabel.Name = "m_toolNameLabel";
            this.m_toolNameLabel.Size = new System.Drawing.Size(0, 13);
            this.m_toolNameLabel.TabIndex = 2;
            // 
            // StatusForm
            // 
            this.ClientSize = new System.Drawing.Size(323, 121);
            this.ControlBox = false;
            this.Controls.Add(this.m_toolNameLabel);
            this.Controls.Add(this.m_waitLabel);
            this.Controls.Add(this.lblStatus);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StatusForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label m_waitLabel;
        private System.Windows.Forms.Label m_toolNameLabel;
    }
}