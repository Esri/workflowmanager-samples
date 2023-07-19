using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Core.Events;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Workflow.Client;
using ArcGIS.Desktop.Workflow.Client.Models;
using ArcGIS.Desktop.Workflow.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace WorkflowManagerSampleAddIn
{
  internal class SubscribeToJobs : Button
  {
    SubscriptionToken stepMessageReceivedEventToken = null;

    private void Subscribe()
    {
      stepMessageReceivedEventToken = StepMessageReceivedEvent.Subscribe(OnStepMessageReceived);
    }

    internal void OnStepMessageReceived(StepMessageEventArgs msg)
    {
      if (msg != null || msg.Message != null)
      {
        var title = "Messages have been recieved!";
        var para = $"Message Type: {msg.Type}\n\n Message Contents:{msg.Message}";
        MessageBox.Show(para, title);
      }
    }


    protected override void OnClick()
    {
      var mgr = WorkflowClientModule.NotificationManager;
      var jobId = "yrS6WMxzQ1qmtMDQxR_jjg";

      Subscribe();

      var jobs = new List<string>() { jobId };
      QueuedTask.Run(async () => {
        mgr.SubscribeToJobs(jobs);

        var title = "Subscribe to a job, with no messages received yet";
        var msg = $"To recieve messages, start a step on the job";
        MessageBox.Show(msg, title);
      }
      );
    }
  }
}
