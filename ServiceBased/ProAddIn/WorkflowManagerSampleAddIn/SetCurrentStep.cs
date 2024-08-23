using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Workflow.Client;
using System;

namespace WorkflowManagerSampleAddIn
{
    internal class SetCurrentStep : Button
    {
        protected override void OnClick()
        {
            // Get the Workflow Manager server url
            var jobsManager = WorkflowClientModule.JobsManager;

            // Update the jobId and stepId information for your workflow item
            var jobId = Module1.Current.JobId ?? "3AzCO4YHSrus8Ck6fgSn9w";
            string otherStepId = "dea81717-cf6a-4965-bb47-3388a2df3632";

            QueuedTask.Run(() =>
            {
                try
                {
                    jobsManager.SetCurrentStep(jobId, otherStepId);
                    var title = "Updated the Current step to a different step.";
                    var msg = $"\nJobId: {jobId}\nNew Current StepId: {otherStepId}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to set the current step";
                    var msg = $"\nJobId: {jobId}\nNew Current StepId: {otherStepId}"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }
            });
        }
    }
}
