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
            var jobId = "bX7L4B9KSpqwjbsMgfz0vQ";
            var stepIds = new List<string> { "df4c8d20-5c99-457f-0be1-21fa8f830760" }; // Set one or more current steps
            var stepIdStr = string.Join(",", stepIds);

            QueuedTask.Run(() =>
            {
                try
                {
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
