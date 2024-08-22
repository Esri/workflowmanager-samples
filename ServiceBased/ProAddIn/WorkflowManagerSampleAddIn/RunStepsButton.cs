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
    internal class RunStepsButton : Button
    {
        protected override void OnClick()
        {
            // Get an instance of the JobsManager
            var jobsManager = WorkflowClientModule.JobsManager;

            // Update the jobId information for your workflow item
            var jobId = Module1.Current.JobId ?? "bX7L4B9KSpqwjbsMgfz0vQ";

            QueuedTask.Run(() =>
            {
                try
                {
                    jobsManager.RunSteps(jobId);
                    var title = "Running Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}";
                    MessageBox.Show(msg, title);
                } catch (Exception ex)
                {
                    var title = "Failed Running Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }

                try
                {
                    jobsManager.StopSteps(jobId);
                    var title = "Stopped Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed Stopping Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nError: {ex.Message}";
                    MessageBox.Show(msg, title);

                }

                try
                {
                    jobsManager.FinishSteps(jobId);
                    var title = "Finished Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed Finishing Current Step(s) on a Job";
                    var msg = $"\nJobId: {jobId}\nError: {ex.Message}";
                    MessageBox.Show(msg, title);

                }
            });

        }
    }
}
