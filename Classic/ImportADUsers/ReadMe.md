<span>Import Active Directory Users</span>

* * *

This sample demonstrates synchronizing the ArcGIS Workflow Manager users and groups with Active Directory users and groups.

<table x-use-null-cells="" style="x-cell-content-align: top; border-spacing: 0px; width: 50%; border-spacing: 0px;" cellspacing="0" width="50%"><colgroup><col style="width: 50%;"> <col style="width: 50%;"></colgroup>

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

*   Active Directory configured with two groups. One should contain a list of users that should be Workflow Manager users, while the other contains a list of groups that should be Workflow Manager groups.

How to use

1.  Open the solution using Visual Studio.

2.  Verify the references are still intact. It may be necessary to configure the JTXUtilities reference again.

3.  Compile the sample project to create the ImportADUsers executable.

Adding a task to the Windows Scheduler

1.  Once the project has been compiled and you've tested running it from the command line, you can build a batch file to add to a Windows Scheduled process.

2.  Ensure the path to the executable matches your deployment location.

3.  Ensure parameters are set correctly.

4.  From the Start Menu, select Accessories > System Tools > Scheduled Tasks

5.  Double click Add Scheduled Task

6.  Follow the directions on the wizard, browsing to the bat file you created and setting the time you'd like the syncronization to occur.

Additional information

The optional arguments the compiled executable will accept are

*   <span style="font-family: Verdana, sans-serif; font-size: 8pt;">/db:<Workflow Manager database alias></span>

*   /domain:<AD domain>

*   /username:<AD username>

*   /password:<AD password>

*   /userGroup:<AD group containing users to import>

*   /groupGroup:<AD group containing groups to import>
