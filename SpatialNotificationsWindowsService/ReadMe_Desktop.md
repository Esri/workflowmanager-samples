Spatial Notification Windows Service for ArcGIS Workflow Manager Desktop

* * *

This sample demonstrates how to evaluate the geodatabase archive sessions registered with the jobs in your system for matches to the changes rules you've configured for the ArcGIS Workflow Manager System.

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

ArcEditor: JTX

</td>

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; border-bottom-color: #808080; border-bottom-width: 1px;
	border-bottom-style: Solid; border-right-color: #808080; border-right-width: 1px;
	border-right-style: Solid;" width="50%">

ArcEditor: JTX

</td>

</tr>

<tr style="x-cell-content-align: top;" valign="top">

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; border-bottom-style: Solid; border-right-color: #808080;
	border-right-width: 1px; border-right-style: Solid; border-left-color: #808080;
	border-left-width: 1px; border-left-style: Solid; border-bottom-color: #808080;
	border-bottom-width: 1px;" width="50%">

ArcInfo: JTX

</td>

<td style="width: 50%; padding-right: 10px; padding-left: 10px; padding-bottom: 3px;
	padding-top: 3px; border-bottom-style: Solid; border-bottom-color: #808080;
	border-bottom-width: 1px; border-right-color: #808080; border-right-width: 1px;
	border-right-style: Solid;" width="50%">

ArcInfo: JTX

</td>

</tr>

</tbody>

</table>

Additional Requirements

*   Data workspaces configured in the ArcGIS Workflow Manager System with layers registered for notifications

*   Spatial notification change rules configured in the ArcGIS Workflow Manager System

<span style="font-weight: bold; font-family: Verdana, sans-serif; font-size: 8pt;">Where to Find it</span>

Workflow Manager Install Location\Developer Kit\Samples\SpatialNotificationsWindowsService\CSharp\Desktop

How to use

1.  Open the solution using Visual Studio.

2.  Verify the references are still intact.

3.  Compile the solution.

Install the windows service

1.  To install the windows service, use the .NET utility called InstallUtil.exe.

2.  In a command window, navigate to your <span style="font-family: 'Courier New', monospace;">Microsoft.NET\Framework\v2.0.50727</span> directory.

3.  Type in InstallUtil.exe and hit enter to see the command usage.

4.  Enter in <span style="font-family: 'Courier New', monospace;">InstallUtil.exe SpatialNotificationService.exe</span>. <spaces> </spaces>Include the logging options if desired. <spaces> </spaces>Note that you will need to provide the full path to the SpatialNotificationService executable.

5.  You will be prompted to enter in creditials for the process owner.

6.  If the installation finished successfully, open up your Services directory.

7.  From the Start Menu, choose Control Panel > Administrative Tools > Service.

8.  In the list, find the Spatial Notification Service.

9.  Start the service.

10.  To receive the notifications, make changes in your data workspace that match the rules.

Additional Information

The service is configured to check for new archive sessions to evaluate ever 15 seconds. You can change this interval in the Service1.cs class.
