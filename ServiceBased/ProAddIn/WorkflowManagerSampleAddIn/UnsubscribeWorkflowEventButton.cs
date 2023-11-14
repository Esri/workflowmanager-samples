using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Workflow.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManagerSampleAddIn
{
    internal class UnsubscribeWorkflowEventButton : Button
    {
        protected override void OnClick()
        {
            var subscriptionToken = ModuleData.WorkflowConnectionSubscriptionToken;
            if (subscriptionToken != null )
            {
                WorkflowConnectionChangedEvent.Unsubscribe(subscriptionToken);

                var title = "Unsubscribed";
                var msg = $"\nUnsubscribed to Event: WorkflowConnectionChangedEvent";
                MessageBox.Show(msg, title);

                ModuleData.WorkflowConnectionSubscriptionToken = null;
            }
        }
    }
}
