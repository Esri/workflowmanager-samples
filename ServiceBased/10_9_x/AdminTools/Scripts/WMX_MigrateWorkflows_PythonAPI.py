# Name:        Migrate Workflow Manager diagrams and job templates using the ArcGIS Python API
#
# Purpose:      This tool performs the following functions:
#               - Gets the source and destination workflow items
#               - Creates a list of diagram id's and job template id's that do not currently exist in the destination workflow instance
#               - Creates the missing diagrams and job templates in the destination workflow instance. Extended property table and field set up carries over as well.
#
# Author:      Tiffany Weintraub (tweintraub@esri.com)
# Created:     2/10/2021
# Updated:     2/11/2021
#
# COPYRIGHT © 2022 Esri
# All material copyright ESRI, All Rights Reserved, unless otherwise specified.
# See https://github.com/Esri/workflowmanager-samples/blob/master/License.txt for details.
#
##-----------------------------------------------------------------------------------------------
# ------------------------------------------ Modules ------------------------------------------------------
import arcpy
from arcgis.gis import GIS
from arcgis.gis.workflowmanager import WorkflowManager
import Config

#------------------------------------------ Parameters ---------------------------------------------------------

source_wfm_itemID = arcpy.GetParameterAsText(0)
dest_wfm_itemID = arcpy.GetParameterAsText(1)

# #--------------------------------------------- Main ------------------------------------------------------------
try:
    #
    # Get workflow items in the source and destination environments
    source_gis = GIS(
        url=Config.portal_params.portal_url,
        password=Config.portal_params.p_password,
        username=Config.portal_params.p_username,
    )
    arcpy.AddMessage(f"\t--> connected as {source_gis.properties.user.username}")

    dest_gis = GIS(
        url=Config.dest_portal_params.dest_portal_url,
        password=Config.dest_portal_params.dest_password,
        username=Config.dest_portal_params.dest_username,
    )
    arcpy.AddMessage(f"\t--> connected as {dest_gis.properties.user.username}")

    arcpy.AddMessage(f'\n...Getting workflow items from source and destination Portals...')
    source_wf_item = source_gis.content.get(source_wfm_itemID)
    dest_wf_item = dest_gis.content.get(dest_wfm_itemID)
    source_wm = WorkflowManager(source_wf_item)
    dest_wm = WorkflowManager(dest_wf_item)

    arcpy.AddMessage(f'\n...Creating diagrams in destination workflow item...')
    existing_diagrams = [x.diagram_id for x in dest_wm.diagrams]
    diagram_ids = [x.diagram_id for x in source_wm.diagrams if not(x.diagram_id in existing_diagrams)]
    diagram_id_map = {}
    for id in diagram_ids:
        d = source_wm.diagram(id)
        new_id = dest_wm.create_diagram(d.diagram_name, d.steps, d.display_grid, d.description, d.diagram_version >= 0, d.annotations, d.data_sources) # Remove datasources here if you don't want to copy
        diagram_id_map.update({id: new_id})
        arcpy.AddMessage(f'\t...Created diagram {d.diagram_name}. ID = {new_id}')


    arcpy.AddMessage(f'\n...Creating job templates in destination workflow item...')
    existing_templates = [x.job_template_id for x in dest_wm.job_templates]
    template_ids = [x.job_template_id for x in source_wm.job_templates if not(x.job_template_id in existing_templates)]
        
    for t_id in template_ids:
        j = source_wm.job_template(t_id)
        source_diagramID = j.diagram_id
        dest_diagramID = diagram_id_map.get(source_diagramID, None)

        if dest_diagramID is not None:
            new_template_id = dest_wm.create_job_template(j.job_template_name, j.default_priority_name, t_id,assigned_to=j.default_assigned_to, diagram_id=dest_diagramID, 
            diagram_name=j.diagram_name, assigned_type=j.default_assigned_type, description=j.description, state=j.state, extended_property_table_definitions=j.extended_property_table_definitions)
            arcpy.AddMessage(f'\t...Created job template {j.job_template_name}. ID = {new_template_id} and assigned appropriate diagram.')
        else:
            new_template_id = dest_wm.create_job_template(j.job_template_name, j.default_priority_name, t_id,assigned_to=j.default_assigned_to, assigned_type=j.default_assigned_type, 
            description=j.description, state=j.state, extended_property_table_definitions=j.extended_property_table_definitions)
            arcpy.AddMessage(f'\t...Created job template {j.job_template_name}. ID = {new_template_id}.')
            

except Exception as ex:
    arcpy.AddMessage(str(ex))