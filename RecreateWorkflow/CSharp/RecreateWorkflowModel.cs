using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.JTX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecreateWorkflowWPF
{
    internal class RecreateWorkflowModel
    {
        private IJTXDatabase db;
        private IJTXSystemUtilities3 utils;

        public RecreateWorkflowModel()
        {
            utils = new JTXUtility() as IJTXSystemUtilities3;
            var dbmgr = new JTXDatabaseManager();
            db = dbmgr.GetActiveDatabase();
        }

        public Tuple<bool, bool, string> RecreateWorkflow(IJTXJob _job)
        {
            bool recreated = false;
            try
            {
                var job = _job as IJTXJob4;
                if (job == null)
                    return MakeError(recreated, "Could not retrieve job");

                if (job.Stage == jtxJobStage.jtxJobStageClosed || job.Stage == jtxJobStage.jtxJobStageDoneWorking)
                    return MakeError(recreated, "Cannot recreate workflow for a closed job");

                var workflowExec = job as IJTXWorkflowExecution3;
                var workflowConfig = job as IJTXWorkflowConfiguration2;

                var curStepIDs = workflowExec.GetCurrentSteps();
                if (curStepIDs.Length != 1)
                    return MakeError(recreated, "Must be one and only one current step");

                var curStep = workflowConfig.GetStep(curStepIDs[0]) as IJTXStep4;
                if (curStep == null)
                    return MakeError(recreated, "Could not find current step {0}", curStepIDs[0]);
                var curStepName = curStep.StepName;
                job.RecreateWorkflow(true); // If you aren't auto commiting in your instance, can set this to false, or could get it from system settings
                recreated = true;

                int[] newStepIDs = FindStepByName(workflowConfig, curStepName);

                if (newStepIDs.Length <= 0)
                {
                    utils.LogJobAction("Comment", db, job, null, "The workflow was recreated");
                    return MakeError(recreated, "Could not find {0} step in re-created workflow", curStepName);
                }

                if (newStepIDs.Length > 1)
                {
                    utils.LogJobAction("Comment", db, job, null, "The workflow was recreated");
                    return MakeError(recreated, "Step {0} is not unique in the re-created workflow", curStepName);
                }

                workflowExec.SetCurrentStep(newStepIDs[0]);
                utils.LogJobAction("Comment", db, job, null, "The workflow was recreated and current step was reset to " + curStepName);

                return Tuple.Create<bool, bool, string>(true, true, null);
            }
            catch (Exception ex)
            {
                return MakeError(recreated, ex.Message);
            }
        }

        internal List<IJTXJob> QueryJobs(string query)
        {
            IQueryFilter qf = new QueryFilter();
            qf.WhereClause = query;
            List<IJTXJob> jobs = new List<IJTXJob>();
            var jobSet = db.JobManager.GetJobsByQuery(qf);
            for (int i = 0; i < jobSet.Count; i++)
            {
                jobs.Add(jobSet.Item[i]);
            }
            return jobs;
        }

        private int[] FindStepByName(IJTXWorkflowConfiguration2 workflowConfig, string name)
        {
            var stepIDs = workflowConfig.GetAllSteps();
            var output = new List<int>();
            foreach (var stepID in stepIDs)
            {
                var step = workflowConfig.GetStep(stepID) as IJTXStep4;
                // This is doing a case-insensitive search. May not be appropriate for some use cases
                if (step.StepName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    output.Add(stepID);
            }
            return output.ToArray();
        }

        private Tuple<bool, bool, string> MakeError(bool recreated, string err)
        {
            return Tuple.Create(recreated, false, err);
        }

        private Tuple<bool, bool, string> MakeError(bool recreated, string errFormat, params object[] values)
        {
            return Tuple.Create(recreated, false, String.Format(errFormat, values));
        }
    }
}
