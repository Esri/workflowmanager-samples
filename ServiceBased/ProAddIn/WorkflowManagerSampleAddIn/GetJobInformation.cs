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
    internal class GetJobInformation : Button
    {
        protected override void OnClick()
        {
            // Get an instance of the JobsManager
            var jobsManager = WorkflowClientModule.JobsManager;

            // Update the jobId information for your workflow item
            var jobId = "HSoSXIeFSmu3nuV4rrEzsA";

            QueuedTask.Run(() =>
            {
                try
                {
                    // Get properties for a job
                    var job = jobsManager.GetJob(jobId, true, true);
                    var title = "Getting the Job information and properties";
                    var msg = $"\nJobId: {jobId}\n\n Job Information is model as: {job}\n\n For Example job.JobId: {job.JobId}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to get the job information";
                    var msg = $"\nJobId: {jobId}\n" + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }
            });
        }
    }
}
