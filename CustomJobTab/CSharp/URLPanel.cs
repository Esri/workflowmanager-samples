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
