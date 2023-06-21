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
            var jobId = "3AzCO4YHSrus8Ck6fgSn9w";

            var stepOneId = "aeccabed-cc0c-442f-b7f5-566ff0ed9403";
            var stepTwoId = "eb472524-1908-42a9-a64c-2fd4076055db";
            var stepThreeId = "943932bc-a855-4d58-92e9-87026c829ecd";
            QueuedTask.Run(() =>
            {
                try
                {
                    jobsManager.AssignCurrentStep(jobId, ArcGIS.Desktop.Workflow.Client.Models.AssignedType.User, "manager_one");
                    var title = "Assigning the current step in the job to another user";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepOneId}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to assign the current step to another user";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepOneId}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }

                try
                {
                    jobsManager.AssignStep(jobId, stepOneId, ArcGIS.Desktop.Workflow.Client.Models.AssignedType.User, "admin");
                    var title = "Assigning a step in the job to another user";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepOneId}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to assign a step to another user";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepOneId}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }

                try
                {
                    jobsManager.SetCurrentStep(jobId, stepThreeId);
                    var title = "Set the Current step to a different step.";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepThreeId}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to set the current step";
                    var msg = $"\nJobId: {jobId}\nStepId(s): {stepThreeId}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }
            });

        }
    }
}
