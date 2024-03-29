# Workflow Manager-Samples

Simple samples that show you how to add different types of add ins and steps to your Workflow Manager desktop application.

## Features
* [ClearAOIContextMenu](./ClearAOIContextMenu)
* [CreateWorkforceAssignment](./CreateWorkforceAssignment)
* [CustomAOICommand](./CustomAOICommand)
* [CustomJobTab](./CustomJobTab)
* [CustomNotifier](./CustomNotifier)
* [CustomSteps](./CustomSteps)
* [DockableWindow](./DockableWindow)
* [EnvVariableParser](./EnvVariableParser)
* [ImportADUsers](./ImportADUsers)
* [MonitorWorkforce](./MonitorWorkforce)
* [OverdueJobsNotification](./OverdueJobsNotification)
* [RecreateWorkflow](./RecreateWorkflow)
* [ReportNotification](./ReportNotification)
* [SpatialNotificationsWindowsService](./SpatialNotificationsWindowsService)


## Instructions

1. Download the samples.
2. Open the solution of the sample you want, and compile it.
3. Follow the readme file for that sample to install it

### Server Deployment
**.NET / C++**
<br>
  Some sample steps that do not have a UI component or do not require user interactions can be run on Server.
1. Follow the instructions for compiling and running the sample step code on Desktop.
2. Compile the sample step code using Workflow Manager Server DLLs: <br>`<ArcGISServerInstallFolder>\bin\JTX*.dll` <br>`<ArcGISServerInstallFolder>\WMX\Server<Version>\DotNet\*.dll`
3. Register the sample step code DLL with ArcGIS Server using the `ESRIRegAsm` utility in `C:\Program Files\Common Files\ArcGIS\bin`
4. Restart ArcGIS Server

Workflow Manager Server should now be able to find and run the sample step.

## Requirements

* VS 2012 or later
* .NET Framework 4.5

## Issues

Find a bug or want to request a new feature?  Please let us know by submitting an issue.

## Contributing

Esri welcomes contributions from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).

## Licensing
Copyright 2018 Esri

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

A copy of the license is available in the repository's [License.txt](License.txt) file.
