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
    internal class UpdateStepAssignments : Button
    {
        protected override void OnClick()
        {
            // Get the Workflow Manager server url
            var jobsManager = WorkflowClientModule.JobsManager;

            // Update the jobId and stepId information for your workflow item
            var jobId = "3AzCO4YHSrus8Ck6fgSn9w";
            string stepId = null;

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

                    stepId = currentSteps[0].StepId; 
                }
                catch (Exception ex)
                {
                    var title = "Failed Retrieving Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\n"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                    return;
                }

                var assignedToUser = "admin";
                try
                {
                    jobsManager.AssignCurrentStep(jobId, ArcGIS.Desktop.Workflow.Client.Models.AssignedType.User, "admin");
                    var title = "Assigned the current step in the job to another user";
                    var msg = $"\nJobId: {jobId}\nUser: {assignedToUser}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to assign the current step to another user";
                    var msg = $"\nJobId: {jobId}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }

                assignedToUser = "testuser";
                try
                {
                    jobsManager.AssignStep(jobId, stepId, ArcGIS.Desktop.Workflow.Client.Models.AssignedType.User, "admin");
                    var title = "Assigned a specific current step in the job to another user";
                    var msg = $"\nJobId: {jobId}\nStepId: {stepId}\nUser: {assignedToUser}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to assign a step to another user";
                    var msg = $"\nJobId: {jobId}\nStepId: {stepId}\nUser: {assignedToUser}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }
            });
        }
    }
}
