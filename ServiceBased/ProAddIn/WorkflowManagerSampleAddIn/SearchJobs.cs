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
    internal class SearchJobs : Button
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
                    var searchResult = jobsManager.SearchJobs(
                        new ArcGIS.Desktop.Workflow.Client.Models.SearchQuery()
                        {
                            Q = "closed=0",
                            SortFields = new List<ArcGIS.Desktop.Workflow.Client.Models.SortField>()
                            {
                              new ArcGIS.Desktop.Workflow.Client.Models.SortField()
                              {
                                FieldName = "jobName", SortOrder = ArcGIS.Desktop.Workflow.Client.Models.SortOrder.Asc
                              },
                              new ArcGIS.Desktop.Workflow.Client.Models.SortField()
                              {
                                FieldName = "priority", SortOrder = ArcGIS.Desktop.Workflow.Client.Models.SortOrder.Desc
                              }
                            }
                        });
                    var title = "Getting the Job information and properties";
                    var msg = $"Search Results are modeled as:\n {searchResult} \n\n For example, searchResult.Num: {searchResult.Num}";
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
