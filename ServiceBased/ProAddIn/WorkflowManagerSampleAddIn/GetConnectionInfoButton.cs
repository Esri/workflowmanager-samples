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
    internal class GetConnectionInfoButton : Button
    {
        protected override void OnClick()
        {
            // Determine if there is an active Workflow Manager connection
            var isConnected = WorkflowClientModule.IsConnected;

            // Get the Workflow Manager item Id
            var itemId = WorkflowClientModule.ItemId;

            // Get the Workflow Manager server url
            var serverUrl = WorkflowClientModule.ServerUrl;

            var title = "Workflow Manager Connection Info";
            var msg = $"\nIsConnected:{isConnected}"
                + $"\nItemId: {itemId}"
                + $"\nServerUrl: {serverUrl}";
            MessageBox.Show(msg, title);
        }
    }
}
