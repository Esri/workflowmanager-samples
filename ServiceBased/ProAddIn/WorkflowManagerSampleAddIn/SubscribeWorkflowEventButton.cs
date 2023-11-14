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
    internal class SubscribeWorkflowEventButton : Button
    {
        protected override void OnClick()
        {
            if (ModuleData.WorkflowConnectionSubscriptionToken == null)
            {
                var subscriptionToken = WorkflowConnectionChangedEvent.Subscribe(e =>
                {
                    var title = "Event Notification";
                    var msg = $"\nWorkflowConnectionChangedEvent Notifictation\nIs User Signed In: {e.IsSignedIn}";
                    MessageBox.Show(msg, title);

                    return Task.CompletedTask;
                });

                ModuleData.WorkflowConnectionSubscriptionToken = subscriptionToken;

                var title = "Subscribed";
                var msg = $"\nSubscribed to Event: WorkflowConnectionChangedEvent";
                MessageBox.Show(msg, title);
            }
        }
    }
}
