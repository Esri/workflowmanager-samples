# Name:        Export Workflow Manager diagrams and job templates using the ArcGIS Python API
#
# Purpose:      This tool performs the following functions:
#               - Exports diagram and job template configurations from the workflow item into individual JSON files
#
# Author:      Tiffany Weintraub (tweintraub@esri.com) and Mark Torrey (mtorrey@esri.com)
#
# Created:     11/4/2021
# Updated:     
##-----------------------------------------------------------------------------------------------
# ------------------------------------------ Modules ------------------------------------------------------
import arcpy, os, configparser, json
from arcgis.gis import GIS
from arcgis.gis.workflowmanager import WorkflowManager
import Config
#------------------------------------------ Parameters ---------------------------------------------------------

source_wfm_itemID = arcpy.GetParameterAsText(0)
json_dir = arcpy.GetParameterAsText(1)


# #--------------------------------------------- Main ------------------------------------------------------------
try:
    #
    portal = GIS(
        url=Config.portal_params.portal_url,
        password=Config.portal_params.p_password,
        username=Config.portal_params.p_username,
    )
    arcpy.AddMessage(f"\t--> connected as {portal.properties.user.username}")

    # Get workflow items in the source and destination environments

    arcpy.AddMessage(f'\nGetting workflow items from source Portal...')
    source_wf_item = portal.content.get(source_wfm_itemID)
    source_wm = WorkflowManager(source_wf_item)


    arcpy.AddMessage(f'\nExporting configuration files...')
    if not os.path.exists(json_dir):
        os.mkdir(json_dir)
            
    diagram_ids = [x.diagram_id for x in source_wm.diagrams]
    for id in diagram_ids:
        d = source_wm.diagram(id)
        file_path = os.path.join(json_dir, f"Diagram___{d.diagram_id}.json")
        with open(file_path, 'w') as file:
            s = {
               "diagram_name": d.diagram_name, 
               "steps": d.steps, 
               "display_grid": d.display_grid, 
               "description": d.description,
               "active": True,
               "diagram_version": d.diagram_version, 
               "annotations": d.annotations, 
               "data_sources": d.data_sources,
               "diagram_id": d.diagram_id,
               "initial_step_id": d.initial_step_id,
               "initial_step_name": d.initial_step_name
            }
            json.dump(s, file)

        arcpy.AddMessage(f'\t...Created diagram {d.diagram_name} JSON file.')

    arcpy.AddMessage(f'\n\n')
    template_ids = [x.job_template_id for x in source_wm.job_templates]
    for t_id in template_ids:
        j = source_wm.job_template(t_id)
        template_file_path = os.path.join(json_dir, f"Template___{t_id}.json")
        with open(template_file_path, 'w') as t_file:
            t = {
                "name": j.job_template_name, 
                "priority": j.default_priority_name,
                "id": j.job_template_id,
                "assigned_to": j.default_assigned_to,
                "diagram_id": j.diagram_id, 
                "diagram_name": j.diagram_name,
                "assigned_type": j.default_assigned_type, 
                "description": j.description, 
                "state": j.state, 
                "extended_property_table_definitions": j.extended_property_table_definitions
            }
            json.dump(t, t_file)

        arcpy.AddMessage(f'\t...Created job template {j.job_template_name} JSON. ')

    arcpy.AddMessage(f'\n\nConfiguration files exported successfully!')

except Exception as ex:
    arcpy.AddMessage(str(ex))