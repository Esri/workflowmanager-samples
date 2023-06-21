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
    internal class JobStatistics : Button
    {
        protected override void OnClick()
        {
            // Get the Workflow Manager server url
            var jobsManager = WorkflowClientModule.JobsManager;
            var jobId = "HSoSXIeFSmu3nuV4rrEzsA";

            QueuedTask.Run(() =>
            {
                try
                {
                    var jobStats = jobsManager.CalculateJobStatistics(new ArcGIS.Desktop.Workflow.Client.Models.JobStatisticsQuery()
                    {
                        Q = "closed=0"
                    });
                    var title = "Getting the Job information and properties";
                    var msg = $"\nJob Statistics after searching are modeled as: {jobStats}\n\n For Example, jobStats.Total = {jobStats.Total}";
                    MessageBox.Show(msg, title);
                }
                catch (Exception ex)
                {
                    var title = "Failed to get the job information";
                    var msg = $"\nJobId: {jobId}\n"
                        + $"\nError: {ex.Message}";
                    MessageBox.Show(msg, title);
                }
            });

        }
    }
}
