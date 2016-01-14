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