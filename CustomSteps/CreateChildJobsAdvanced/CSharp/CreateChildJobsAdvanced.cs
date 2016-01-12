using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;
using ESRI.ArcGIS.JTXUI;

namespace JTXSamples
{
    [Guid("CDC6ACDA-F9D2-4A7F-B4DB-1772284057B1")]
    public class CreateChildJobsAdvanced : IJTXCustomStep
    {
        #region Registration Code
        [ComRegisterFunction()]
        static void Reg(String regKey)
        {
            ESRI.ArcGIS.JTX.Utilities.JTXUtilities.RegisterJTXCustomStep(regKey);
        }

        [ComUnregisterFunction()]
        static void Unreg(String regKey)
        {
            ESRI.ArcGIS.JTX.Utilities.JTXUtilities.UnregisterJTXCustomStep(regKey);
        }
        #endregion

        #region Private Members
        ////////////////////////////////////////////////////////////////////////
        // DECLARE: Data Members
        private IJTXDatabase m_ipDatabase = null;
        private readonly string[] m_ExpectedArgs = { 
            "jobtypename", 
            "assigngroup", 
            "assignuser", 
            "dependThisStep", 
            "dependNextStep", 
            "dependStatus", 
            "useparentaoi", 
            "aoiOverlapFeatureClassName",
            "numberChildJobs",
            "createVersionSetting",
            "assignVersionSetting",
            "setChildExtendedProps",
            "dueDate",
            "jobDuration"
        };

        private IJTXJobSet m_ipJobs = null;

        private string m_paramJobTypeName = null;
        private string m_paramAssignToGroup = null;
        private string m_paramAssignToUser = null;
        private bool m_paramDependThisStep = false;
        private bool m_paramDependNextStep = false;
        private string m_paramStatusType = null;
        private bool m_paramHasStatusType = false;
        private bool m_paramUseParentAOI = false;
        private string m_AOIOverlapFeatureClassName = null;
        private int m_paramNumberChildJobs = 1;
        private CreateVersionType m_paramCreateVersionType = CreateVersionType.None;
        private AssignVersionType m_paramAssignVersionType = AssignVersionType.None;
        private string m_paramExtendedProperties = null;
        private DateTime m_dueDate = Constants.NullDate;
        private int m_duration = -1;

        #endregion

        #region Enums and Structs

        private enum CreateVersionType
        {
            UseParentJobsVersion,
            UseParentJobsParentVersion,
            UseParentJobsDefaultVersion,
            UseJobTypeDefaultSettings,
            None
        };
        private enum AssignVersionType
        {
            UseParentJobsVersion,
            None
        };

        public struct ExtendedPropertyIdentifier
        {
            public string LongTableName;
            public string TableName;
            public string FieldName;
            public string Value;

            public ExtendedPropertyIdentifier(string longTableName, string tableName, string fieldName, string value)
            {
                LongTableName = longTableName;
                TableName = tableName;
                FieldName = fieldName;
                Value = value;
            }
        }

        #endregion

        #region IJTXCustomStep Members

        /// <summary>
        /// A description of the expected arguments for the step type.  This should
        /// include the syntax of the argument, whether or not it is required/optional, 
        /// and any return codes coming from the step type.
        /// </summary>
        public string ArgumentDescriptions
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"Job Type Name:");
                sb.AppendFormat("\t/{0}:<job type name> (required)\r\n\r\n", m_ExpectedArgs[0]);
                sb.AppendLine(@"Assign To Group:");
                sb.AppendFormat("\t/{0}:<group to assign to> (optional)\r\n", m_ExpectedArgs[1]);
                sb.AppendLine(@"Assign To User:");
                sb.AppendFormat("\t/{0}:<username to assign to> (optional)\r\n", m_ExpectedArgs[2]);
                sb.AppendLine(@"Dependency will be created and current job held at this step:");
                sb.AppendFormat("\t/{0} (optional)\r\n", m_ExpectedArgs[3]);
                sb.AppendLine(@"Dependency will be created and current job held at the next step in the workflow:");
                sb.AppendFormat("\t/{0} (optional)\r\n", m_ExpectedArgs[4]);
                sb.AppendLine(@"Dependency status (current job held until child job reaches this status):");
                sb.AppendFormat("\t/{0}:<Status Type Name> (optional)\r\n", m_ExpectedArgs[5]);
                sb.AppendLine(@"Use the parent job's AOI as the child job's AOI:");
                sb.AppendFormat("\t/{0} (optional)\r\n", m_ExpectedArgs[6]);
                sb.AppendLine(@"Create child jobs based on the overlap between the parent job's AOI and this feature class:");
                sb.AppendFormat("\t/{0}:<fully qualified feature class name> (optional)\r\n", m_ExpectedArgs[7]);
                sb.AppendLine(@"Default number of child jobs to create:");
                sb.AppendFormat("\t/{0}:<number of jobs to create> (optional)\r\n", m_ExpectedArgs[8]);
                sb.AppendLine(@"A version will be created for the child job(s) based on this selection:");
                sb.AppendFormat("\t/{0}:<the version to use as the parent version> (optional)\r\n", m_ExpectedArgs[9]);
                sb.AppendLine(@"A version will be assigned to the child job(s) based on this selection:");
                sb.AppendFormat("\t/{0}:<the existing version the job will be assigned to> (optional)\r\n", m_ExpectedArgs[10]);
                sb.AppendLine(@"Child job(s) extended properties value will be set to: one of the parent job's extended properties values (specified by JTX Token) or to the given string value:");
                sb.AppendFormat("\t/{0}:<ChildJobFullyQualifiedExtendedPropertiesTableName.FieldName=[JOBEX:ParentJobFullyQualifiedExtendedPropertiesTableName.FieldName]> (optional)\r\n", m_ExpectedArgs[11]);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Called when a step of this type is executed in the workflow.
        /// </summary>
        /// <param name="JobID">ID of the job being executed</param>
        /// <param name="StepID">ID of the step being executed</param>
        /// <param name="argv">Array of arguments passed into the step's execution</param>
        /// <param name="ipFeedback">Feedback object to return status messages and files</param>
        /// <returns>Return code of execution for workflow path traversal</returns>
        public int Execute(int JobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {
            System.Diagnostics.Debug.Assert(m_ipDatabase != null);

            if (!ConfigurationCache.IsInitialized)
                ConfigurationCache.InitializeCache(m_ipDatabase);

            IJTXJobManager jobMan = m_ipDatabase.JobManager;
            IJTXJob2 m_ParentJob = (IJTXJob2)jobMan.GetJob(JobID);

            if (!GetInputParams(argv)) 
                return 0;

            return CreateJobs(m_ParentJob);
        }

        /// <summary>
        /// Invoke an editor tool for managing custom step arguments.  This is
        /// an optional feature of the custom step and may not be implemented.
        /// </summary>
        /// <param name="hWndParent">Handle to the parent application window</param>
        /// <param name="argsIn">Array of arguments already configured for this custom step</param>
        /// <returns>Returns a list of newely configured arguments as specified via the editor tool</returns>
        public object[] InvokeEditor(int hWndParent, object[] argsIn)
        {
            JTXSamples.CreateChildJobsArgEditor editorForm = new JTXSamples.CreateChildJobsArgEditor(m_ipDatabase, m_ExpectedArgs);
            object[] newArgs = null;

            return (editorForm.ShowDialog(argsIn, out newArgs) == DialogResult.OK) ? newArgs : argsIn;
        }

        /// <summary>
        /// Called when the step is instantiated in the workflow.
        /// </summary>
        /// <param name="ipDatabase">Database connection to the JTX repository.</param>
        public void OnCreate(IJTXDatabase ipDatabase)
        {
            m_ipDatabase = ipDatabase;
        }

        /// <summary>
        /// Method to validate the configured arguments for the step type.  The
        /// logic of this method depends on the implementation of the custom step
        /// but typically checks for proper argument names and syntax.
        /// </summary>
        /// <param name="argv">Array of arguments configured for the step type</param>
        /// <returns>Returns 'true' if arguments are valid, 'false' if otherwise</returns>
        public bool ValidateArguments(ref object[] argv)
        {
            string strValue = "";

            if (!StepUtilities.GetArgument(ref argv, m_ExpectedArgs[0], true, out strValue)) { return false; }
            return StepUtilities.AreArgumentNamesValid(ref argv, m_ExpectedArgs);
        }

        #endregion

        #region Private Methods

        ////////////////////////////////////////////////////////////////////////
        // METHOD: GetInputParams
        private bool GetInputParams(object [] argv)
        {
            string sTemp = "";

            // Job Type 
            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[0], true, out m_paramJobTypeName);

            // Job Assignment
            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[1], true, out m_paramAssignToGroup);
            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[2], true, out m_paramAssignToUser);

            // Job Dependencies 
            m_paramDependThisStep = StepUtilities.GetArgument(ref argv, m_ExpectedArgs[3], true, out sTemp);
            m_paramDependNextStep = StepUtilities.GetArgument(ref argv, m_ExpectedArgs[4], true, out sTemp);
            m_paramHasStatusType = StepUtilities.GetArgument(ref argv, m_ExpectedArgs[5], true, out m_paramStatusType);

            // Number of Jobs and AOI Definition
            m_paramUseParentAOI = StepUtilities.GetArgument(ref argv, m_ExpectedArgs[6], true, out sTemp);
            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[7], true, out m_AOIOverlapFeatureClassName);
            if (StepUtilities.GetArgument(ref argv, m_ExpectedArgs[8], true, out sTemp))
                m_paramNumberChildJobs = Convert.ToInt16(sTemp);

            // Versioning
            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[9], true, out sTemp);
            if (string.IsNullOrEmpty(sTemp))
                m_paramCreateVersionType = CreateVersionType.None;
            else if (sTemp.Equals("The parent job's version"))
                m_paramCreateVersionType = CreateVersionType.UseParentJobsVersion;
            else if (sTemp.Equals("The parent job's parent version"))
                m_paramCreateVersionType = CreateVersionType.UseParentJobsParentVersion;
            else if (sTemp.Equals("The parent job's DEFAULT version"))
                m_paramCreateVersionType = CreateVersionType.UseParentJobsDefaultVersion;
            else if (sTemp.Equals("The job type's default properties parent version"))
                m_paramCreateVersionType = CreateVersionType.UseJobTypeDefaultSettings;

            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[10], true, out sTemp);
            if (string.IsNullOrEmpty(sTemp))
                m_paramAssignVersionType = AssignVersionType.None;
            else if (sTemp.Equals("The parent job's version"))
                m_paramAssignVersionType = AssignVersionType.UseParentJobsVersion;

            // Extended Properties
            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[11], true, out m_paramExtendedProperties);

            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[12], true, out sTemp);
            if (!String.IsNullOrEmpty(sTemp))
                m_dueDate = JTXUtilities.GenerateDateString(m_ipDatabase.JTXWorkspace, sTemp, false);

            StepUtilities.GetArgument(ref argv, m_ExpectedArgs[13], true, out sTemp);
            if (!String.IsNullOrEmpty(sTemp))
                int.TryParse(sTemp, out m_duration);

            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: CreateJobs
        private int CreateJobs(IJTXJob2 pParentJob)
        {
            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                bool bAutoCommit = ConfigurationCache.AutoCommitWorkflow;

                m_ipDatabase.LogMessage(5, 2000, "CreateJobs");

                // Set the job template values
                IJTXJobManager2 pJobMan = m_ipDatabase.JobManager as IJTXJobManager2;
                IJTXJobDescription pJobDescription = new JTXJobDescriptionClass();

                pJobDescription.Description = pParentJob.Description;
                pJobDescription.Priority = pParentJob.Priority;
                pJobDescription.ParentJobId = pParentJob.ID;

                pJobDescription.StartDate = pParentJob.StartDate;

                if (m_dueDate != Constants.NullDate)
                    pJobDescription.DueDate = m_dueDate;
                else if (m_duration > 0)
                    pJobDescription.DueDate = System.DateTime.Now.AddDays(m_duration);
                else
                    pJobDescription.DueDate = pParentJob.DueDate;

                if (!String.IsNullOrEmpty(m_paramAssignToGroup))
                {
                    pJobDescription.AssignedType = jtxAssignmentType.jtxAssignmentTypeGroup;
                    pJobDescription.AssignedTo = m_paramAssignToGroup;
                }
                else if (!String.IsNullOrEmpty(m_paramAssignToUser))
                {
                    pJobDescription.AssignedType = jtxAssignmentType.jtxAssignmentTypeUser;
                    pJobDescription.AssignedTo = m_paramAssignToUser;
                }
                else
                {
                    pJobDescription.AssignedType = jtxAssignmentType.jtxAssignmentTypeUnassigned;
                }

                pJobDescription.OwnedBy = ConfigurationCache.GetCurrentJTXUser().UserName;

                if (pParentJob.ActiveDatabase != null)
                    pJobDescription.DataWorkspaceID = pParentJob.ActiveDatabase.DatabaseID;

                // Set the parent version.  This only makes sense if the active workspace has been set
                if (pJobDescription.DataWorkspaceID != null)
                {
                    if (m_paramCreateVersionType == CreateVersionType.None
                        || m_paramCreateVersionType == CreateVersionType.UseParentJobsVersion)
                        pJobDescription.ParentVersionName = pParentJob.VersionName;	// This has to be set here because setting the job workspace resets the value
                    else if (m_paramCreateVersionType == CreateVersionType.UseJobTypeDefaultSettings)
                    {
                        IJTXJobType pJobType = m_ipDatabase.ConfigurationManager.GetJobType(m_paramJobTypeName);
                        if (pJobType != null)
                            pJobDescription.ParentVersionName = pJobType.DefaultParentVersionName;
                    }
                    else if (m_paramCreateVersionType == CreateVersionType.UseParentJobsDefaultVersion)
                        pJobDescription.ParentVersionName = pParentJob.JobType.DefaultParentVersionName;
                    else if (m_paramCreateVersionType == CreateVersionType.UseParentJobsParentVersion)
                        pJobDescription.ParentVersionName = pParentJob.ParentVersion;
                }

                // Determine the number of jobs to make
                m_ipDatabase.LogMessage(5, 2000, "Before Determining Number of Jobs");

                IArray aoiList = null;
                int numJobs;
                if (!GetNumberOfJobs(pParentJob, ref aoiList, out numJobs))
                    return 1;

                if (numJobs <= 0)
                {
                    MessageBox.Show(Properties.Resources.ZeroJobCount);
                    return 0;
                }
                pJobDescription.AOIList = aoiList;
                m_ipDatabase.LogMessage(5, 2000, "After Determining Number of Jobs");


                // Create the job objects
                m_ipDatabase.LogMessage(5, 2000, "Before CreateJobs");
                pJobDescription.JobTypeName = m_paramJobTypeName;
                IJTXExecuteInfo pExecInfo;
                m_ipJobs = pJobMan.CreateJobsFromDescription(pJobDescription, numJobs, true, out pExecInfo);
                m_ipDatabase.LogMessage(5, 2000, "After CreateJobs");


                // Populate the job data
                for (int i = 0; i < m_ipJobs.Count; ++i)
                {
                    IJTXJob pJob = m_ipJobs.get_Item(i);

                    SetJobProperties(pJobMan, pJob, pParentJob);
                }
                return 1;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == (int)fdoError.FDO_E_SE_INVALID_COLUMN_VALUE)
                {
                    MessageBox.Show(Properties.Resources.InvalidColumn, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
                return 0;
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: GetNumberOfJobs
        private bool GetNumberOfJobs(IJTXJob2 pParentJob, ref IArray aoiList, out int numJobs)
        {
            bool AOIFromOverlap = false;
            bool AOIFromParent = false;
            numJobs = -1;

            // Then use defaults
            AOIFromOverlap = !string.IsNullOrEmpty(m_AOIOverlapFeatureClassName);
            AOIFromParent = m_paramUseParentAOI;
            numJobs = m_paramNumberChildJobs;

            if (AOIFromOverlap)
            {
                if (!PopulateAOIListUsingOverlapFeatureClass(pParentJob, ref aoiList))
                {
                    return false;
                }
                numJobs = aoiList.Count;
            }
            else if (AOIFromParent)
            {
                if (!PopulateAOIListUsingParent(pParentJob, ref aoiList, numJobs))
                {
                    return false;
                }
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: PopulateAOIListUsingOverlapFeatureClass
        private bool PopulateAOIListUsingOverlapFeatureClass(IJTXJob2 pParentJob, ref IArray aoiList)
        {
            try
            {
                // Make sure all the information exists to get the data workspace
                if (pParentJob.ActiveDatabase == null)
                {
                    MessageBox.Show("Unable to proceed: Please set the data workspace for this job.");
                    return false;
                }
                if (pParentJob.AOIExtent == null)
                {
                    MessageBox.Show("Unable to proceed: Please assign the AOI for this job.");
                    return false;
                }

                // Get the feature workspace from the current data workspace
                IJTXDatabase2 pJTXDB = (IJTXDatabase2)m_ipDatabase;
                string activeDBID = pParentJob.ActiveDatabase.DatabaseID;
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)pJTXDB.GetDataWorkspace(activeDBID, pParentJob.VersionExists() ? pParentJob.VersionName : "");
                if (featureWorkspace == null)
                {
                    MessageBox.Show("Unable to connect to Data Workspace");
                    return false;
                }

                IFeatureClass featureClass = null;
                try
                {
                    featureClass = featureWorkspace.OpenFeatureClass(m_AOIOverlapFeatureClassName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to connect to feature class to generate AOIs: " +
                        m_AOIOverlapFeatureClassName + "\n Error: " + ex.ToString());
                    return false;
                }

                // Get all features that intersect the parent job's AOI
                //
                // Note: The parent job's AOI is shrunk very slightly so features that merely adjoin the parent's AOI 
                // are *not* returned.  Only features that have some part of their actual area intersecting the parent's 
                // AOI are returned.
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                ITopologicalOperator topOp = (ITopologicalOperator)pParentJob.AOIExtent;
                IPolygon slightlySmallerExtent = (IPolygon)topOp.Buffer(-0.0001);
                spatialFilter.Geometry = slightlySmallerExtent;
                spatialFilter.GeometryField = featureClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);

                aoiList = new ArrayClass();
                IFeature feature = null;
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    aoiList.Add(feature.Shape);
                }

                // Explicitly release the cursor.  
                Marshal.ReleaseComObject(featureCursor);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create AOIs based on feature class: " + m_AOIOverlapFeatureClassName + ". " + ex.ToString());
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: PopulateAOIListUsingParent
        private bool PopulateAOIListUsingParent(IJTXJob2 pParentJob, ref IArray aoiList, int numCopies)
        {
            try
            {
                aoiList = new ArrayClass();
                for (int i = 0; i < numCopies; i++)
                {
                    aoiList.Add(pParentJob.AOIExtent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create AOIs based on parent job's AOI: " + ex.Message);
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: SetJobProperties
        private IJTXJob SetJobProperties(IJTXJobManager pJobMan, IJTXJob pJob, IJTXJob2 pParentJob)
        {
            m_ipDatabase.LogMessage(5, 2000, "Before LogJobAction (CreateJobs)");

            IJTXActivityType pActType = m_ipDatabase.ConfigurationManager.GetActivityType(Constants.ACTTYPE_COMMENT);

            pParentJob.LogJobAction(pActType, null,
                String.Format(Properties.Resources.ActTypeMessage, pJob.ID.ToString()));

            if (!string.IsNullOrEmpty(m_paramExtendedProperties))
            {
                m_ipDatabase.LogMessage(5, 2000, "Before Create Extended Properties");

                ExtendedPropertyIdentifier childExProps = new ExtendedPropertyIdentifier();
                try
                {
                    ParseExtendedPropertiesParam(out childExProps);
                }
                catch (Exception ex)
                {
                    string msg = string.Format(
                        "Unable to read parent job's extended property. Job ID: {0} ERROR: {1}",
                        pParentJob.ID, ex.Message);
                    m_ipDatabase.LogMessage(3, 1000, msg);
                }
                try
                {
                    CreateExtendedPropertyRecord(pJob, childExProps);
                }
                catch (Exception ex)
                {
                    string msg = string.Format(
                        "Unable to set child job's extended property. Child Job ID: {0} ERROR: {1}",
                        pJob.ID, ex.Message);
                    m_ipDatabase.LogMessage(3, 1000, msg);
                }
                m_ipDatabase.LogMessage(5, 2000, "After Create Extended Properties");
            }


            // Create dependencies on parent job if configured 
            m_ipDatabase.LogMessage(5, 2000, "Before Setting Dependencies");

            if (m_paramDependNextStep || m_paramDependThisStep)
            {
                IJTXJobDependencies ipDependencyManager = pJobMan as IJTXJobDependencies;
                CreateDependencyOnParentJob(ipDependencyManager, pParentJob, pJob.ID);
            }
            m_ipDatabase.LogMessage(5, 2000, "After Setting Dependencies");


            // Create or assign version if configured 
            m_ipDatabase.LogMessage(5, 2000, "Before Versioning");

            if (m_paramCreateVersionType != CreateVersionType.None)
            {
                if (pParentJob.ActiveDatabase != null && !String.IsNullOrEmpty(pJob.ParentVersion) && 
                    (m_paramCreateVersionType != CreateVersionType.UseParentJobsVersion || pParentJob.VersionExists()))
                    CreateChildVersion(ref pJob);
                else
                    MessageBox.Show("Could not create version.  Please ensure parent data workspace and versions are set correctly", Properties.Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (m_paramAssignVersionType != AssignVersionType.None)
            {
                if (pParentJob.ActiveDatabase != null)
                    AssignChildVersion(pParentJob, ref pJob);
                else
                    MessageBox.Show("Could not assign version.  Please ensure parent data workspace is set correctly", Properties.Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            m_ipDatabase.LogMessage(5, 2000, "After Versioning");


            // Store the job and save the changes
            m_ipDatabase.LogMessage(5, 2000, "Before Storing Job");
            pJob.Store();
            m_ipDatabase.LogMessage(5, 2000, "After Storing Job");
            return pJob;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: ParseExtendedPropertiesParam
        private bool ParseExtendedPropertiesParam(out ExtendedPropertyIdentifier child)
        {
            string[] props = m_paramExtendedProperties.Split('=');

            string childInfo = props[0];

            int lastDot = childInfo.LastIndexOf('.');
            child.FieldName = childInfo.Substring(lastDot + 1, childInfo.Length - (lastDot + 1));

            child.LongTableName = childInfo.Substring(0, lastDot);

            lastDot = child.LongTableName.LastIndexOf('.');
            if (lastDot >= 0)
            {
                child.TableName = child.LongTableName.Substring(lastDot + 1);
            }
            else
            {
                child.TableName = child.LongTableName;
            }

            child.Value = props[1];

            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: CreateExtendedPropertyRecord
        private bool CreateExtendedPropertyRecord(IJTXJob job, ExtendedPropertyIdentifier child)
        {
            IJTXAuxProperties pAuxProps = (IJTXAuxProperties)job;

            IJTXAuxRecordContainer pAuxContainer = pAuxProps.GetRecordContainer(child.LongTableName);
            if (!string.IsNullOrEmpty(m_paramExtendedProperties) && pAuxContainer == null)
            {
                string msg = string.Format(
                        "Unable to set extended property for child job {0}. Unable to find child job's extended property table: {1}",
                        job.ID, child.LongTableName);
                m_ipDatabase.LogMessage(3, 2000, msg);
            }

            System.Array contNames = pAuxProps.ContainerNames;
            System.Collections.IEnumerator contNamesEnum = contNames.GetEnumerator();
            contNamesEnum.Reset();
            while (contNamesEnum.MoveNext())
            {
                try
                {
                    string strContainerName = (string)contNamesEnum.Current;
                    if (!string.IsNullOrEmpty(m_paramExtendedProperties) && (strContainerName.ToUpper()).Equals(child.LongTableName.ToUpper()))
                    {
                        pAuxContainer = pAuxProps.GetRecordContainer(strContainerName);

                        if (pAuxContainer.RelationshipType != esriRelCardinality.esriRelCardinalityOneToOne)
                        {
                            throw new Exception("The table relationship is not one-to-one.");
                        }
                        IJTXAuxRecord childRecord = pAuxContainer.GetRecords().get_Item(0);//pAuxContainer.CreateRecord();


                        SetExtendedPropertyValues(childRecord, child);
                        JobUtilities.LogJobAction(m_ipDatabase, job, Constants.ACTTYPE_UPDATE_EXT_PROPS, "", null);
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format(
                        "Unable to create extended property {0} in record for jobid {1}. ERROR: {2}",
                        child.FieldName, job.ID, ex.Message);
                    m_ipDatabase.LogMessage(3, 2000, msg);
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: SetExtendedPropertyValues
        private bool SetExtendedPropertyValues(IJTXAuxRecord childRecord, ExtendedPropertyIdentifier exPropIdentifier)
        {
            int fieldIndex = GetFieldIndex(childRecord, exPropIdentifier);
            childRecord.set_Data(fieldIndex, exPropIdentifier.Value);
            childRecord.Store();

            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: GetFieldIndex
        private int GetFieldIndex(IJTXAuxRecord record, ExtendedPropertyIdentifier exPropIdentifier)
        {
            for (int i = 0; i < record.Count; i++)
            {
                string propertyName = record.get_PropName(i).ToUpper();
                if (propertyName.Equals(exPropIdentifier.FieldName.ToUpper()))
                {
                    return i;
                }
            }
            throw new Exception("Unable to find field name " + exPropIdentifier.FieldName + " in " + exPropIdentifier.TableName);
        }
        ////////////////////////////////////////////////////////////////////////
        // METHOD: CreateDependencyOnParentJob
        private void CreateDependencyOnParentJob(IJTXJobDependencies dependencyManager, IJTXJob2 pParentJob, int childJobID)
        {
            IJTXJobDependency dependency = dependencyManager.CreateDependency(pParentJob.ID);

            dependency.DepJobID = childJobID;
            dependency.DepOnType = jtxDependencyType.jtxDependencyTypeStatus;

            IJTXStatus statusType = null;
            if (!m_paramHasStatusType) statusType = m_ipDatabase.ConfigurationManager.GetStatus("Closed");
            else statusType = m_ipDatabase.ConfigurationManager.GetStatus(m_paramStatusType);

            dependency.DepOnValue = statusType.ID;
            dependency.HeldOnType = jtxDependencyType.jtxDependencyTypeStep;

            IJTXWorkflowExecution parentWorkflow = pParentJob as IJTXWorkflowExecution;
            int[] currentSteps = parentWorkflow.GetCurrentSteps();
            int dependentStep = currentSteps[0];

            if (m_paramDependNextStep)
            {
                IJTXWorkflowConfiguration workflowConf = pParentJob as IJTXWorkflowConfiguration;
                try
                {
                    dependentStep = workflowConf.GetAllNextSteps(currentSteps[0])[0];
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show(Properties.Resources.NoNextStep, Properties.Resources.Error,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            dependency.HeldOnValue = dependentStep;
            dependency.Store();

            IPropertySet props = new PropertySetClass();
            props.SetProperty("[DEPENDENCY]", dependency.ID);
            JobUtilities.LogJobAction(m_ipDatabase, pParentJob, Constants.ACTTYPE_ADD_DEPENDENCY, "", props);

        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: AssignChildVersion
        private bool AssignChildVersion(IJTXJob2 pParentJob, ref IJTXJob pJob)
        {
            if (m_paramAssignVersionType == AssignVersionType.UseParentJobsVersion)
            {
                ((IJTXJob2)pJob).SetActiveDatabase(pParentJob.ActiveDatabase.DatabaseID);
                pJob.ParentVersion = pParentJob.ParentVersion;
                pJob.VersionName = pParentJob.VersionName;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        // METHOD: CreateChildVersion
        private bool CreateChildVersion(ref IJTXJob pJob)
        {
            IVersion pNewVersion = null;
            try
            {
                string strVersionName = pJob.VersionName;
                int index = strVersionName.IndexOf(".", 0);
                if (index >= 0)
                {
                    strVersionName = strVersionName.Substring(index + 1);
                }
                pJob.VersionName = strVersionName;

                pNewVersion = pJob.CreateVersion(esriVersionAccess.esriVersionAccessPublic);

                if (pNewVersion == null)
                {
                    m_ipDatabase.LogMessage(3, 1000, "Unable to create version for child job ID: " + pJob.ID);
                }
                else
                {
                    IPropertySet pOverrides = new PropertySetClass();
                    pOverrides.SetProperty("[VERSION]", pNewVersion.VersionName);
                    JobUtilities.LogJobAction(m_ipDatabase, pJob, Constants.ACTTYPE_CREATE_VERSION, "", pOverrides);
                    JTXUtilities.SendNotification(Constants.NOTIF_VERSION_CREATED, m_ipDatabase, pJob, pOverrides);
                }

            }
            catch (Exception ex)
            {
                m_ipDatabase.LogMessage(3, 1000, "Unable to create version for child job ID: " + pJob.ID + ". ERROR: " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pNewVersion);
            }

            return true;
        }

        #endregion
    }	// End Class
}	// End Namespace
