/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.esri.arcgis.jtx.customSteps;

import com.esri.arcgis.geodatabase.esriRelCardinality;
import com.esri.arcgis.interop.AutomationException;
import com.esri.arcgis.interop.extn.ArcGISCategories;
import com.esri.arcgis.interop.extn.ArcGISExtension;
import com.esri.arcgis.jtx.IJTXActivityType;
import com.esri.arcgis.jtx.IJTXAuxProperties;
import com.esri.arcgis.jtx.IJTXAuxPropertiesProxy;
import com.esri.arcgis.jtx.IJTXAuxRecordContainer;
import com.esri.arcgis.jtx.IJTXConfigurationProperties;
import com.esri.arcgis.jtx.IJTXCustomStepFeedback;
import com.esri.arcgis.jtx.IJTXDatabase;
import com.esri.arcgis.jtx.IJTXCustomStepSafe;
import com.esri.arcgis.jtx.IJTXJob;
import com.esri.arcgis.jtx.IJTXJobManager;
import com.esri.arcgis.jtx.IJTXJobType;
import com.esri.arcgis.jtx.jtxAssignmentType;
import com.esri.arcgis.jtx.utilities.Constants;
import com.esri.arcgis.jtx.utilities.JTXUtilities;
import com.esri.arcgis.jtx.utilities.StepUtilities;
import com.esri.arcgis.jtx.utilities.WorkflowUtilities;
import java.io.IOException;

@ArcGISExtension(progid="JTXJavaSteps.CreateJob",
		clsid="77D1C6BD-4E47-42d5-9D39-31E40B5BE952",
		categories={ ArcGISCategories.JTXCustomStep })
public class CreateJob implements IJTXCustomStepSafe {
    private final String[] m_expectedArgs = {"jobtypeid", "assigngroup", "assignuser"};
    private IJTXDatabase m_ipDatabase = null;

    @Override
    public String getArgumentDescriptions() throws IOException, AutomationException {
        StringBuilder sb = new StringBuilder();
        sb.append("Job Type ID:\r\n");
        sb.append(String.format("\t/%s:<job type id> (required)\r\n\r\n", m_expectedArgs[0]));
        sb.append("Assign To Group\r\n");
        sb.append(String.format("\t/%s:<group to assign to> (optional)\r\n", m_expectedArgs[1]));
        sb.append("Assign To User:\r\n");
        sb.append(String.format("\t/%s:<username to assign to> (optional)\r\n", m_expectedArgs[2]));

        return sb.toString();
    }

    @Override
    public int execute(int jobID, int stepID, String[] argv, IJTXCustomStepFeedback is) throws IOException, AutomationException {
        String strValue = StepUtilities.getArgument(argv, m_expectedArgs[0], true);
        if (strValue == null) {
            //throw new ArgumentNullException(m_expectedArgs[0], String.Format("\nMissing the {0} parameter!", m_expectedArgs[0]));
            m_ipDatabase.logMessage(3, 1000, "Missing the {0} parameter!");
            return -1;
        }

        int jobTypeID;
        try {
            jobTypeID = Integer.parseInt(strValue);
        } catch (NumberFormatException ex) {
            m_ipDatabase.logMessage(3, 1000, ex.getMessage());
            return -1;
        }


        IJTXJobType pJobType = m_ipDatabase.getConfigurationManager().getJobTypeByID(jobTypeID);
        IJTXJobManager pJobMan = m_ipDatabase.getJobManager();
        IJTXJob pNewJob = pJobMan.createJob(pJobType, 0, true);

        IJTXActivityType pActType = m_ipDatabase.getConfigurationManager().getActivityType(Constants.ACTTYPE_CREATE_JOB);
        if (pActType != null) {
            pNewJob.logJobAction(pActType, null, "");
        }

        JTXUtilities.sendNotification(Constants.NOTIF_JOB_CREATED, m_ipDatabase, pNewJob, null);

        // Assign a status to the job if the Auto Assign Job Status setting is enabled
        IJTXConfigurationProperties pConfigProps = (IJTXConfigurationProperties) m_ipDatabase.getConfigurationManager();
        if (pConfigProps.propertyExists(Constants.JTX_PROPERTY_AUTO_STATUS_ASSIGN)) {
            String strAutoAssign = pConfigProps.getProperty(Constants.JTX_PROPERTY_AUTO_STATUS_ASSIGN);
            if (strAutoAssign.equals("TRUE")) {
                pNewJob.setStatus(m_ipDatabase.getConfigurationManager().getStatus("Created"));
            }
        }

        // Associate the current job with the new job with a parent-child relationship
        pNewJob.setParentJob(jobID);

        // Assign the job as specified in the arguments
        String strAssignTo = StepUtilities.getArgument(argv, m_expectedArgs[1], true);
        String strAssignToUser = StepUtilities.getArgument(argv, m_expectedArgs[2], true);
        if (strAssignTo != null)
        {
            m_ipDatabase.logMessage(5, 1000, "Assigning to group " + strAssignTo);
            pNewJob.setAssignedType(jtxAssignmentType.jtxAssignmentTypeGroup);
            pNewJob.setAssignedTo(strAssignTo);
        }
        else if (strAssignToUser != null)
        {
            m_ipDatabase.logMessage(5, 1000, "Assigning to user " + strAssignToUser);
            pNewJob.setAssignedType(jtxAssignmentType.jtxAssignmentTypeUser);
            pNewJob.setAssignedTo(strAssignToUser);
        }
        pNewJob.store();

        // Copy the workflow to the new job
        WorkflowUtilities.copyWorkflowXML(m_ipDatabase, pNewJob);

        // Create 1-1 extended property entries
        IJTXAuxProperties pAuxProps = new IJTXAuxPropertiesProxy(pNewJob);
        String[] contNames = pAuxProps.getContainerNames();
        for (String strContainerName : contNames) {
            IJTXAuxRecordContainer pAuxContainer = pAuxProps.getRecordContainer(strContainerName);
            if (pAuxContainer.getRelationshipType() == esriRelCardinality.esriRelCardinalityOneToOne) {
                pAuxContainer.createRecord();
            }
        }

        return 0;
    }

    @Override
    public String[] invokeEditor(int i, String[] inArgs) throws IOException, AutomationException {
        try {
            ArgEditor editorForm = new ArgEditor(null, m_ipDatabase, m_expectedArgs);
            String[] outArgs = editorForm.showDialog(inArgs);

            m_ipDatabase.logMessage(5, 1000, "There are " + outArgs.length + " arguments");
            return outArgs;
        } catch (IOException e) {
            if (m_ipDatabase != null) {
                m_ipDatabase.logMessage(3, 500, e.getMessage());
            }
            throw e;
        }
    }

    @Override
    public void onCreate(IJTXDatabase ipDatabase) throws IOException, AutomationException {
        m_ipDatabase = ipDatabase;
    }

    @Override
    public boolean validateArguments(String[] argv) throws IOException, AutomationException {
        String strValue = StepUtilities.getArgument(argv, m_expectedArgs[0], true);

        if (strValue == null) {
            return false;
        }
        return StepUtilities.areArgumentNamesValid(argv, m_expectedArgs);
    }

    @Override
    public String getName() throws IOException, AutomationException {
        return "JTXJavaSteps.CreateJob";
    }

    @Override
    public String getCLSID() throws IOException, AutomationException {
        return "{77D1C6BD-4E47-42d5-9D39-31E40B5BE952}";
    }
}
