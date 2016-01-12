using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace JTXSamples
{
    public partial class AttachmentTypeDialog : Form
    {
        public AttachmentTypeDialog()
        {
            InitializeComponent();
        }

        private void continueBtn__Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool isEmbeddedSelected()
        {
            return this.embeddedBtn_.Checked;
        }

        public bool isNoneSelected()
        {
            return this.noneBtn_.Checked;
        }

        public bool isLinkedSelected()
        {
            return this.linkedBtn_.Checked;
        }

        public bool AttachToParent
        {
            get
            {
                return this.attachToParentJobCb_.Checked;
            }
        }

        public bool AttachToCurrent
        {
            get
            {
                return this.attachToCurrentJobCb_.Checked;
            }
        }

        public bool EnableSelectParent
        {
            set
            {
                this.attachToParentJobCb_.Enabled = value;
            }
        }
    }
}