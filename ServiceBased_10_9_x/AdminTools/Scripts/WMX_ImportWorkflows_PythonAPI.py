# Name:        Import Workflow Manager diagrams and job templates using the ArcGIS Python API
#
# Purpose:      This tool performs the following functions:
#               - Creates two lists: a) diagram files  b) template files
#               - Creates the missing diagrams destination workflow instance. Creates a dictionary of original and new diagram id's
#               - Creats the missing job templates in the destination workflow instance. Extended property tables are created as well so long 
#                   as there is not a table with the same name that exists in the service already.
#
# Author:      Tiffany Weintraub (tweintraub@esri.com) and Mark Torrey (mtorrey@esri.com)
# Copyright:   (c) 2022 Esri
#
# Created:     11/4/2021
# Updated:     
##---------------------------------------------------------------------------------------------------------
import arcpy, os, json
from arcgis.gis import GIS
from arcgis.gis.workflowmanager import WorkflowManager
import Config

#------------------------------------------ Parameters ----------------------------------------------------

dest_wfm_itemID = arcpy.GetParameterAsText(0)
folder = arcpy.GetParameterAsText(1)

# #--------------------------------------------- Main -----------------------------------------------------
try:
    #

    portal = GIS(
        url=Config.portal_params.portal_url,
        password=Config.portal_params.p_password,
        username=Config.portal_params.p_username,
    )
    arcpy.AddMessage(f"\t--> connected as {portal.properties.user.username}")

    # Get workflow items in the source and destination environments
    arcpy.AddMessage(f'\nGetting workflow items from destination Portal...')
    dest_wf_item = portal.content.get(dest_wfm_itemID)
    dest_wm = WorkflowManager(dest_wf_item)
    table_defs = [t['tableName'] for t in dest_wm.table_definitions]

    # Create a list of diagram files and a list of job template files
    wmx_files = os.listdir(folder)
    diagram_files = []
    template_files = []

    for f in wmx_files:
        if "Template_" in f:
            template_files.append(os.path.join(folder, f))
        else:
            diagram_files.append(os.path.join(folder, f))


    # Import diagrams
    arcpy.AddMessage(f'\nImporting diagrams into workflow item...')
    diagram_id_map = {}
    existing_diagrams = [x.diagram_name for x in dest_wm.diagrams]
    for df in diagram_files:
        with open(df, 'r') as w:
            d = json.load(w)
            d_name = d['diagram_name']

            if d_name not in existing_diagrams:
                arcpy.AddMessage(f'\tImporting {d_name} diagram...')
                orig_id = d['diagram_id']
                new_id = dest_wm.create_diagram(
                    name=d['diagram_name'], 
                    steps=d['steps'], 
                    display_grid=d['display_grid'], 
                    description=d['description'], 
                    active=d['active'],
                    annotations=d['annotations'], 
                    data_sources=d['data_sources'],
                    )

                arcpy.AddMessage(f'\t\t{d_name} diagram imported successfully! New Diagram ID = {new_id}')
                diagram_id_map.update({orig_id: new_id})
            else:
                arcpy.AddMessage(f'\t...{d_name} diagram already exists in destination workflow item')
                filename = os.path.basename(df).split('___')[1]
                orig_id = filename[:-5]
                new_id = [x.diagram_id for x in dest_wm.diagrams if x.diagram_name == d_name][0]
                diagram_id_map.update({orig_id: new_id})
    
    
    # Import templates
    arcpy.AddMessage(f'\n\n')
    existing_templates = [x.job_template_name for x in dest_wm.job_templates]
    for jt in template_files:
        with open(jt, 'r') as x:
            j = json.load(x)
            source_diagram_id = j['diagram_id']
            new_diagram_id = diagram_id_map.get(source_diagram_id)
            template_name = j['name']

            if template_name not in existing_templates:
                arcpy.AddMessage(f'\tImporting {template_name} job template...')
                existing_defs = j['extended_property_table_definitions']
                if existing_defs:
                    tnames = [t['tableName'] for t in j['extended_property_table_definitions']]
                    for t in tnames:
                        if t not in table_defs: 
                            new_template_id = dest_wm.create_job_template(
                                name=j['name'], 
                                priority=j['priority'],
                                id=j['id'],
                                assigned_to=j['assigned_to'],
                                diagram_id=new_diagram_id, 
                                diagram_name=j['diagram_name'],
                                assigned_type=j['assigned_type'], 
                                description=j['description'], 
                                state=j['state'], 
                                extended_property_table_definitions=j['extended_property_table_definitions']
                                )
                            
                            arcpy.AddMessage(f'\t\t{template_name} job template imported successfully with extended properties table(s)')
                        else:
                            new_template_id = dest_wm.create_job_template(
                                name=j['name'], 
                                priority=j['priority'],
                                id=j['id'],
                                assigned_to=j['assigned_to'],
                                diagram_id=new_diagram_id, 
                                diagram_name=j['diagram_name'],
                                assigned_type=j['assigned_type'], 
                                description=j['description'], 
                                state=j['state']
                                )

                            arcpy.AddMessage(f'\t\t{template_name} imported successfully without extended properties table(s)')
                else:
                    new_template_id = dest_wm.create_job_template(
                                name=j['name'], 
                                priority=j['priority'],
                                id=j['id'],
                                assigned_to=j['assigned_to'],
                                diagram_id=new_diagram_id, 
                                diagram_name=j['diagram_name'],
                                assigned_type=j['assigned_type'], 
                                description=j['description'], 
                                state=j['state'], 
                                extended_property_table_definitions=j['extended_property_table_definitions']
                                )
                            
                    arcpy.AddMessage(f'\t...{template_name} job template imported successfully with extended properties table(s)')
            

except Exception as ex:
    arcpy.AddMessage(str(ex))