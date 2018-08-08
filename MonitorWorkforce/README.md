# WorkflowManager-WorkforceMonitor

This Python sample script monitors the status of Workforce for ArcGIS assignments and automatically moves an ArcGIS Workflow Manager job to the next step when its Workforce assignment has been completed.

## Deployment

Before you begin, ensure that the machine on which you are deploying the script has ArcGIS Pro installed and that you have a workflow with the [Create Workforce Assignment](https://github.com/Esri/workflowmanager-samples/tree/master/CreateWorkforceAssignment) step configured.

1.  Download the Python sample and save it locally.

2.  Install the Requests library for Python if you are using ArcGIS Pro 1.2 or earlier.

    Requests is included with ArcGIS Pro starting at version 1.3.

3.  Browse to the location where you saved the script and open `config.ini` in a text editor.

4.  Update the `PROJECT` parameter in the `[WORKFORCE]` section to match your Workforce project ID.

5.  Update the `URL` parameter in the `[WORKFLOWMANAGER]` section to match the location of your Workflow Manager service URL.

    **Important:**  
    The `URL` parameter must include a trailing slash.

6.  Update the USER parameter in the `[WORKFLOWMANAGER]` section with the Workflow Manager user account you want the script to use to move jobs to the next step when a Workforce assignment is completed.

7.  Update the USERNAME and PASSWORD parameters with the user account credentials you want to the script to use to log in to Workforce.

8.  Update the `LOGFILE` parameter in the `[LOG]` section as necessary.

9.  Start the script to begin monitoring Workforce assignments.

10. Configure a new procedural step in your workflow after the Create Workforce Assignment step.

    When the Workforce assignment is completed the script will automatically move the job from the procedural step you created to the next step in your workflow.
