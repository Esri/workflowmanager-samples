# Workflow Manager ArcGIS Pro Add-In Sample

This ArcGIS Pro Add-In sample utilizes the Workflow Manager Pro SDK to perform various operations against the ArcGIS Workflow Manager workflow items.

## Prerequisite
Setup a development environment for building a Pro Add-In.

Refer to the ArcGIS Pro SDK [Build your first add-in - Prerequisites](https://developers.arcgis.com/documentation/arcgis-add-ins-and-automation/arcgis-pro/tutorials/build-your-first-add-in/#prerequisites) doc for installing the necessary prerequisites.

## Build
1. Open the project in Visual Studio
2. Update the ArcGIS Pro installation path in the following files if necessary. This is needed if ArcGIS Pro is installed in a different location than _C:\Program Files\ArcGIS\Pro_.
   * WorkflowManagerSampleAddIn.csproj
   * Config.daml
   * launchSettings.json
3. Build and run the project in Visual Studio
   * The Workflow Manager Add-In should now be visible in ArcGIS Pro under the _Workflow Manager Add-In_ tab
   * The built add-in will be located at _C:\Users\<username>\Documents\ArcGIS\AddIns\ArcGISPro_

## References

* [Pro SDK API](https://pro.arcgis.com/en/pro-app/latest/sdk/api-reference) - Refer to the **ArcGIS.Desktop.Workflow.Client** Assembly 

## Issues

Find a bug or want to request a new feature?  Please let us know by submitting an issue.

## Contributing

Esri welcomes contributions from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).

## Licensing
Copyright 2023 Esri

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
