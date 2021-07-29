/*Copyright 2015 Esri
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.​*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace JTXSamples
{
    public partial class AddressDialog : Form
    {
        public AddressDialog()
        {
            InitializeComponent();
        }

        public string street;

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            street = txtStreetAddress.Text;

            this.Hide();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
        }
    }
}
