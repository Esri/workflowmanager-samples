using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManagerSampleAddIn
{
    internal class RunSpecificStepsButton : Button
    {
        protected override void OnClick()
        {
            // Get the Workflow Manager server url
            var jobsManager = WorkflowClientModule.JobsManager;

            // Update the jobId and stepId information for your workflow item
            var jobId = Module1.Current.JobId ?? "bX7L4B9KSpqwjbsMgfz0vQ";
            var stepIds = new List<string>();

            QueuedTask.Run(() =>
            {
                try
                {
                    // Get current steps on the job
                    var job = jobsManager.GetJob(jobId);
                    var currentSteps = job.CurrentSteps;
                    if (currentSteps == null || currentSteps.Count < 1)
                    {
                        var title = "Failed Retrieving Current Step(s) on a Job";
                        var msg = $"\nJobId: {jobId}\n";
                        MessageBox.Show(msg, title);
                        return;
                    }

                    stepIds.Add(currentSteps[0].StepId); // Set one or more current steps
                }
                catch (Exception ex)
                {
                    var title = "Failed Retrieving Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\n"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                    return;
                }

                var stepIdStr = string.Join(",", stepIds);
                try
                {
                    // Run specific current steps on the job
                    jobsManager.RunSteps(jobId, stepIds);
                    var title = "Running Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepIdStr}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed Running Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepIdStr}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }

                try
                {
                    jobsManager.StopSteps(jobId, stepIds);
                    var title = "Stopped Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepIdStr}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed Stopping Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepIdStr}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }

                try
                {
                    jobsManager.FinishSteps(jobId, stepIds);
                    var title = "Finished Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepIdStr}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed Finishing Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepIdStr}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }
            });
        }
    }
}
