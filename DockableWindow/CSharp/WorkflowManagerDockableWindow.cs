using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.JTX;

namespace WorkflowManagerDockableWindow
{
    [Guid("46f5c24d-d17a-4153-a95b-00569c47d1b8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("WorkflowManagerDockableWindow.WorkflowManagerDockableWindow")]
    public partial class WorkflowManagerDockableWindow : UserControl, IDockableWindowDef, IJTXJobListener, IJTXExtensionJobUpdater
    {
        private IApplication m_application;

        IJTXJob m_pJob = null;
        IJTXExtension4 m_pExt = null;

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Unregister(regKey);

        }

        #endregion
        #endregion

        public WorkflowManagerDockableWindow()
        {
            InitializeComponent();
        }

        #region IDockableWindowDef Members

        string IDockableWindowDef.Caption
        {
            get
            {
                return "WMX Dockable Window";
            }
        }

        int IDockableWindowDef.ChildHWND
        {
            get { return this.Handle.ToInt32(); }
        }

        string IDockableWindowDef.Name
        {
            get
            {
                return "WorkflowManagerDockableWindow";
            }
        }

        void IDockableWindowDef.OnCreate(object hook)
        {
            m_application = hook as IApplication;

            if (m_application == null)
                return;

            // Get the extension
            IExtension pExt = m_application.FindExtensionByName("Workflow Manager");
            m_pExt = pExt as IJTXExtension4;

            // Register for listening with the extension
            m_pExt.AttachListener(this);
            m_pExt.AttachUpdater(this);
        }

        void IDockableWindowDef.OnDestroy()
        {
            m_pExt = null;
            m_pJob = null;
        }

        object IDockableWindowDef.UserData
        {
            get { return null; }
        }

        #endregion

        #region IJTXJobListener Members

        /// <summary>
        /// This method is called when a new job is opened
        /// </summary>
        /// <param name="pNewJob"></param>
        public void JobChanged(IJTXJob pNewJob)
        {
            if (pNewJob != null)
            {
                // A new job was opened
                m_pJob = pNewJob;

                // Enable the controls
                txtJobName.Enabled = true;
                cmdSave.Enabled = true;

                // Load the info
                LoadJobInfo();
            }
        }

        #endregion

        #region IJTXExtensionJobUpdater Members

        /// <summary>
        /// This method is called when the current job is updated
        /// </summary>
        /// <param name="pUpdatedJob"></param>
        public void JobUpdated(IJTXJob pUpdatedJob)
        {
            if (pUpdatedJob != null)
            {
                // The current job was updated
                m_pJob = pUpdatedJob;

                LoadJobInfo();
            }
        }

        #endregion

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (m_pJob.Name != txtJobName.Text)
            {
                m_pJob.Name = txtJobName.Text;
                m_pJob.Store();

                // Notify the extension that the job has been changed
                m_pExt.UpdateJob(this.ToString());
            }
        }

        private void LoadJobInfo()
        {
            // Fill out the text boxes with info from the job
            txtJobName.Text = m_pJob.Name;
            txtAssignment.Text = (m_pJob.AssignedType == jtxAssignmentType.jtxAssignmentTypeUnassigned) ? "Unassigned" : m_pJob.AssignedTo;
            int[] currSteps = ((IJTXWorkflowExecution)m_pJob).GetCurrentSteps();
            if (currSteps.Length == 0)
                txtCurrentStep.Text = "None";
            else if (currSteps.Length > 1)
                txtCurrentStep.Text = "Multiple";
            else
            {
                IJTXStep3 pStep = ((IJTXWorkflowConfiguration)m_pJob).GetStep(currSteps[0]) as IJTXStep3;
                txtCurrentStep.Text = pStep.StepName;
            }
        }
    }
}
