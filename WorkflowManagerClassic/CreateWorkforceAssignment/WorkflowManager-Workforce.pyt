# Copyright 2016 Esri
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

import arcpy
import json


class Toolbox(object):
    def __init__(self):
        """Define the toolbox (the name of the toolbox is the name of the
        .pyt file)."""
        self.label = "Workforce"
        self.alias = "Tools for integrating Workforce with Workflow Manager"

        # List of tool classes associated with this toolbox
        self.tools = [CreateAssignments]


class CreateAssignments(object):
    def __init__(self):
        """Define the tool (tool name is the name of the class)."""
        self.label = "Create Assignments"
        self.description = "Create a workforce assignment from a Workflow Manager job"
        self.canRunInBackground = False

    def getParameterInfo(self):
        """Define parameter definitions"""
        layerUrlParam = arcpy.Parameter(displayName="Assignment Layer URL",
                                        name="layerUrl",
                                        datatype="GPString",
                                        parameterType="Required",
                                        direction="Input")

        jobIdParam = arcpy.Parameter(displayName="Job ID",
                                     name="jobID",
                                     datatype="GPLong",
                                     parameterType="Required",
                                     direction="Input")

        dispatcherIdParam = arcpy.Parameter(displayName="Dispatcher ID",
                                            name="dispatcherId",
                                            datatype="GPLong",
                                            parameterType="Required",
                                            direction="Input")

        assignmentTypeParam = arcpy.Parameter(displayName="Assignment Type",
                                              name="assignmentType",
                                              datatype="GPLong",
                                              parameterType="Required",
                                              direction="Input")

        descParam = arcpy.Parameter(displayName="Description",
                                    name="description",
                                    datatype="GPString",
                                    parameterType="Optional",
                                    direction="Input")

        jtcParam = arcpy.Parameter(displayName="Input Database Path (.jtc)",
                                   name="jtcPath",
                                   datatype="DEFile",
                                   parameterType="Optional",
                                   direction="Input")
        jtcParam.filter.list = ['jtc']

        outObjectIds = arcpy.Parameter(displayName="Output Object IDs",
                                       name="objectIds",
                                       datatype="GPString",
                                       parameterType="Derived",
                                       direction="Output")

        params = [layerUrlParam, jobIdParam, dispatcherIdParam, assignmentTypeParam, descParam, jtcParam, outObjectIds]
        return params

    def isLicensed(self):
        """Set whether tool is licensed to execute."""
        return True

    def updateParameters(self, parameters):
        """Modify the values and properties of parameters before internal
        validation is performed.  This method is called whenever a parameter
        has been changed."""
        return

    def updateMessages(self, parameters):
        """Modify the messages created by internal validation for each tool
        parameter.  This method is called after internal validation."""
        return

    def execute(self, parameters, messages):
        try:
            import requests
        except ImportError:
            messages.addErrorMessage(
                "This sample relies on Requests. "
                "Please install using pip or see http://docs.python-requests.org/en/latest/ for more info")
            return

        url = parameters[0].valueAsText
        jobId = parameters[1].value
        dispatcher = parameters[2].value
        assignType = parameters[3].value
        description = parameters[4].valueAsText
        jtc = parameters[5].valueAsText
        if not (jtc):
            jtc = None

        messages.addMessage("Getting job LOI")
        layerName = "tempLayer"
        try:
            arcpy.Delete_management(layerName)
        except:
            # Workaround for Pro
            pass
        arcpy.GetJobAOI_wmx(jobId, layerName, jtc)
        # Need to do this otherwise the geometry contains all the features in the FC, not just the selected job
        geoms = arcpy.CopyFeatures_management(layerName, arcpy.Geometry())
        try:
            arcpy.Delete_management(layerName)
        except:
            # Workaround for Pro
            pass

        if len(geoms) == 0:
            messages.addErrorMessage("No LOI associated with job")
            return
        elif len(geoms) > 1:
            messages.addErrorMessage("Unexpected number of LOIs associated with job")
            return
        geom = geoms[0]
        sr = geom.spatialReference
        adds = []

        # Handle AOIs
        if isinstance(geom, arcpy.Polygon):
            if geom.isMultipart:
                geom = [arcpy.Polygon(x).trueCentroid for x in geom]  # Get the centroid of each segment
            else:
                geom = [geom.trueCentroid]

        # Loop through the multipoints. One Workforce assignment will be created for each part
        for pt in geom:
            ptG = arcpy.PointGeometry(pt, sr)
            ptG = ptG.projectAs(arcpy.SpatialReference(102100))

            locationStr = '{}  {}'.format(pt.X, pt.Y)
            projGeom = json.loads(ptG.JSON)

            feature = {"geometry": projGeom,
                       "attributes": {
                           "location": locationStr,
                           "workOrderId": jobId,
                           "priority": 0,
                           "dispatcherId": dispatcher,
                           "description": description,
                           "assignmentType": assignType,
                           "status": 0
                       }
                       }
            adds.append(feature)

        token = arcpy.GetSigninToken()
        if not (token):
            messages.addErrorMessage("Not Signed In. Please sign in to use this script")
            return

        params = {
            "adds": json.dumps(adds),
            "f": "json",
            "token": token['token']
        }

        messages.AddMessage("Creating assignments")
        r = requests.post(url, data=params)
        jsonval = json.loads(r.text)

        if "error" in jsonval:
            messages.addErrorMessage(jsonval["error"])
        else:
            outVals = str([x["objectId"] for x in jsonval["addResults"]])
            messages.addMessage("Created features: " + outVals)
            parameters[6].value = outVals

        return
