# Job Progress Cleanup

Script to allow deleting extraneous internal records for Jobs that have been 
closed for some minimum period of time. This will avoid potential performance 
issues in Job search.

## Instructions

1. Ensure the requirements below are met on the machine where the admin tools will be run.
2. Download the JobProgressCleanup.py script

## Sample Usage

To get detailed usage information
> python JobProgressCleanup.py --help

To run for the first time and delete job progress for jobs closed at least 7 days (with verbose output)
> python JobProgressCleanup.py --endAge 7 --username _username_ --password _password_ --profile _profileName_ -v _portalUrl_ _itemId_

To subsequently run and delete job progress for jobs closed between 21 and 7 days
> python JobProgressCleanup.py --startAge --endAge 7 --profile _profileName_ _portalUrl_ _itemId_ 

## Requirements

- ArcGIS API for Python >= 1.9.0 (>= 2.x is recommended)

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