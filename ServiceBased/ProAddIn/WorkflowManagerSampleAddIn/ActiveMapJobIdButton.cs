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
    internal class ActiveMapJobIdButton : Button
    {
        protected override void OnClick()
        {
            QueuedTask.Run(() =>
            {
                var title = "Active Map View - Job Id";
                try
                {
                    // Get the job Id associated with the active map view
                    var jobManager = WorkflowClientModule.JobsManager;
                    var jobId = jobManager.GetJobId();
                    if (jobId != null)
                    {
                        var msg = $"JobId: {jobId}";
                        MessageBox.Show(msg, title);
                    }
                    else
                    {                        
                        var msg = "No job Id associated with active map view";
                        MessageBox.Show(msg, title);
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"Error retrieving jobId from active map view: {ex.Message}";
                    MessageBox.Show(msg, title);
                }
            });
        }
    }
}
