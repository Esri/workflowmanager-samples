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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTXUI;

namespace JTXSamples
{
    public partial class URLPanel : UserControl, IJTXJobPanel
    {
        public URLPanel()
        {
            InitializeComponent();
        }

        #region IJTXJobPanel Members

        public void Attach(IJTXApplication ipApplication)
        {
            webBrowser1.Url = new Uri("http://www.esri.com/");
        }

        public bool CanRedo
        {
            get { return false; }
        }

        public bool CanSave
        {
            get { return false; }
        }

        public bool CanUndo
        {
            get { return false; }
        }

        public string Caption
        {
            get { return "ESRI.com WebPage"; }
        }

        public System.Drawing.Image Icon
        {
            get { return JTXSamples.Properties.Resources.Globe; }
        }

        public bool IsDirty
        {
            get { return false; }
        }

        public bool IsVisible
        {
            get { return true; }
        }

        public string Name
        {
            get { return "ESRI.com"; }
        }

        public event OnJobInvalidatedEventHandler OnJobInvalidated;

        public event OnJobUpdatedEventHandler OnJobUpdated;

        public void Redo()
        {

        }

        public void RefreshJob()
        {

        }

        public void Save()
        {

        }

        public void SetJob(IJTXJob ipJob)
        {

        }

        public void SetJobs(List<IJTXJob> jobs)
        {

        }

        public string Title
        {
            get { return "Visit ESRI.com"; }
        }

        public string Tooltip
        {
            get { return "URL Panel"; }
        }

        public void Undo()
        {

        }

        #endregion
    }
}
