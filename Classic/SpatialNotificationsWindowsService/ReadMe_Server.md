Spatial Notification Windows Service for Workflow Manager Server

* * *

This sample demonstrates how to evaluate the geodatabase archive sessions registered with the jobs in your system for matches to the changes rules you've configured for the ArcGIS Workflow Manager System. <spaces> </spaces>

<table x-use-null-cells="" style="x-cell-content-align: top; width: 50%; border-spacing: 0px; border-spacing: 0px;" cellspacing="0" width="50%"><colgroup><col style="width: 50%;"> <col style="width: 50%;"></colgroup>

<tbody>

<tr style="x-cell-content-align: top;" valign="top">

<td style="width: 50%; padding-right: 10px; padding-bottom: 4px; padding-top: 4px;
	padding-left: 10px; background-color: #c0c0c0; border-top-style: Solid;
	border-bottom-color: #808080; border-bottom-width: 1px; border-bottom-style: Solid;
	border-right-color: #808080; border-right-width: 1px; border-right-style: Solid;
	border-left-color: #808080; border-left-width: 1px; border-left-style: Solid;
	border-top-color: #808080; border-top-width: 1px;" bgcolor="#C0C0C0" width="50%">

Development licensing

</td>

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; background-color: #c0c0c0; border-top-style: Solid;
	border-bottom-color: #808080; border-bottom-width: 1px; border-bottom-style: Solid;
	border-top-color: #808080; border-top-width: 1px; border-right-color: #808080;
	border-right-width: 1px; border-right-style: Solid;" bgcolor="#C0C0C0" width="50%">

Deployment licensing

</td>

</tr>

<tr style="x-cell-content-align: top;" valign="top">

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; border-bottom-color: #808080; border-bottom-width: 1px;
	border-bottom-style: Solid; border-right-color: #808080; border-right-width: 1px;
	border-right-style: Solid; border-left-color: #808080; border-left-width: 1px;
	border-left-style: Solid;" width="50%">

Server Standard: JTX

</td>

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; border-bottom-color: #808080; border-bottom-width: 1px;
	border-bottom-style: Solid; border-right-color: #808080; border-right-width: 1px;
	border-right-style: Solid;" width="50%">

Server Standard: JTX

</td>

</tr>

<tr style="x-cell-content-align: top;" valign="top">

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; border-bottom-style: Solid; border-right-color: #808080;
	border-right-width: 1px; border-right-style: Solid; border-left-color: #808080;
	border-left-width: 1px; border-left-style: Solid; border-bottom-color: #808080;
	border-bottom-width: 1px;" width="50%">

Server Advanced: JTX

</td>

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; border-bottom-style: Solid; border-bottom-color: #808080;
	border-bottom-width: 1px; border-right-color: #808080; border-right-width: 1px;
	border-right-style: Solid;" width="50%">

Server Advanced: JTX

</td>

</tr>

</tbody>

</table>

Additional Requirements

*   Data workspaces configured in the ArcGIS Workflow Manager System with layers registered for notifications

*   Spatial notification change rules configured in the ArcGIS Workflow Manager System

*   A ArcGIS Workflow Manager Server Object configured pointing to the ArcGIS Workflow Manager system.

<span style="font-weight: bold; font-family: Verdana, sans-serif; font-size: 8pt;">Where to Find it</span>

Workflow Manager Install Location\Developer Kit\Samples\SpatialNotificationsWindowsService\CSharp\Server

How to use

1.  Open the solution using Visual Studio.

2.  Update the web reference to match the URL for your ArcGIS Workflow Manager Service.

3.  Edit the app.config.

1.  Check the web service URL.

2.  Provide a ConnectionUser. <spaces> </spaces>This username must match a user configured in the ArcGIS Workflow Manager System.

3.  Choose whether you want the matches found during the evaluation stored in the ArcGIS Workflow Manager Repository.

4.  Enter a time interval between calls to the database to check for new archive sessions logged for the jobs in the session. <spaces> </spaces>The default is 15 seconds. <spaces> </spaces>In other words, every 15 seconds, the service will check the ArcGIS Workflow Manager System for new geodatabase archive sessions; if one is found, it will evaluate all the edits stored against the change rules configured and send notifications for each of the matches found.

5.  Compile the solution.

Install the windows service

1.  To install the windows service, use the .NET utility called InstallUtil.exe.

2.  In a command window, navigate to your <span style="font-family: 'Courier New', monospace;">Microsoft.NET\Framework\v2.0.50727</span> directory.

3.  Type in InstallUtil.exe and hit enter to see the command usage.

4.  Enter in <span style="font-family: 'Courier New', monospace;">InstallUtil.exe SpatialNotificationServiceWS.exe</span>. <spaces> </spaces>Include the logging options if desired. <spaces> </spaces>Note that you will need to provide the full path to the SpatialNotificationServiceWS executable.

5.  You will be prompted to enter in creditials for the process owner.

6.  If the installation finished successfully, open up your Services directory.

7.  From the Start Menu, choose Control Panel > Administrative Tools > Service.

8.  In the list, find the Spatial Notification Service (Web Services).

9.  Start the service.

10.  To receive the notifications, make changes in your data workspace that match the rules.
