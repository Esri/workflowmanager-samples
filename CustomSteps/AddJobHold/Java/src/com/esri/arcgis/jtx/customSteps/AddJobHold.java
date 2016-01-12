/**
 * 
 */
package com.esri.arcgis.jtx.customSteps;

import java.io.IOException;
import java.util.ArrayList;

import com.esri.arcgis.interop.AutomationException;
import com.esri.arcgis.interop.extn.ArcGISCategories;
import com.esri.arcgis.interop.extn.ArcGISExtension;
import com.esri.arcgis.jtx.IJTXConfiguration;
import com.esri.arcgis.jtx.IJTXCustomStepFeedback;
import com.esri.arcgis.jtx.IJTXCustomStepSafe;
import com.esri.arcgis.jtx.IJTXDatabase;
import com.esri.arcgis.jtx.IJTXHoldType;
import com.esri.arcgis.jtx.IJTXJob;
import com.esri.arcgis.jtx.IJTXJobHold;
import com.esri.arcgis.jtx.IJTXJobHolds;
import com.esri.arcgis.jtx.IJTXJobHoldsProxy;
import com.esri.arcgis.jtx.IJTXJobManager;
import com.esri.arcgis.jtx.utilities.StepUtilities;

@ArcGISExtension(progid="JTXJavaSteps.addJobHold", 
		clsid="3AC3478F-89B7-43be-A7DC-7349E5B305A8", 
		categories={ ArcGISCategories.JTXCustomStep })
public class AddJobHold implements IJTXCustomStepSafe {

	/**
	 * Member variables
	 */
	private static final long serialVersionUID = 1L;
	private IJTXDatabase m_ipDatabase = null;
	private final String [] expectedArgs = new String [] { "HoldType", "HoldRemarks" };

	/* (non-Javadoc)
	 * @see com.esri.arcgis.jtx.IJTXCustomStepSafe#execute(int, int, java.lang.String[], com.esri.arcgis.jtx.IJTXCustomStepFeedback[])
	 */
	public int execute(int jobID, int stepID, String[] argv, IJTXCustomStepFeedback pFeedback) throws IOException,
			AutomationException {
		// Get the arguments
		String sHoldTypeName = StepUtilities.getArgument(argv, expectedArgs[0], true);
		String sHoldRemarks = StepUtilities.getArgument(argv, expectedArgs[1], true);

		Boolean bHoldType = (sHoldTypeName != null);
		if (!bHoldType)
		{
			return -1;
		}
		Boolean bHoldRemarks = (sHoldRemarks != null);

		// Get the hold type
		IJTXConfiguration pJTXConfig = m_ipDatabase.getConfigurationManager();
		IJTXHoldType pHoldType = pJTXConfig.getHoldType(sHoldTypeName);
		if (pHoldType == null)
		{
			return -1;
		}

		// Get the job
		IJTXJobManager pJobManager = m_ipDatabase.getJobManager();
		IJTXJob pJob = pJobManager.getJob(jobID);
		IJTXJobHolds pJobHolds = new IJTXJobHoldsProxy(pJob);

		// Add new job hold
		IJTXJobHold pNewHold = pJobHolds.createHold(pHoldType);
		if (bHoldRemarks)
		{
			pNewHold.setHoldComments(sHoldRemarks);
			pNewHold.store();
		}

		return 0;
	}

	/**
	 * A description of the expected arguments for the step type.  This should
     * include the syntax of the argument, whether or not it is required/optional, 
     * and any return codes coming from the step type.
	 */
	public String getArgumentDescriptions() throws IOException,
			AutomationException {
		final String newLine = System.getProperty("line.separator"); 
		StringBuilder sb = new StringBuilder();
        sb.append("What type of hold are you adding?" + newLine);
        sb.append("\t/HoldType: The name of the hold type to add" + newLine);
        sb.append("   ex:   /HoldType:Resource" + newLine);
        sb.append(newLine);
        sb.append("Remarks" + newLine);
        sb.append("\t/HoldRemarks: The remarks that should be associated with this hold action" + newLine);
        sb.append("   ex:   /HoldRemarks:\"Job on hold till adequate resources identified\"" + newLine);
        sb.append(newLine);
        sb.append("Possible return codes: " + newLine);
        sb.append("  -1: Step failed" + newLine);
        sb.append("   0: Successfully executed" + newLine);

        return sb.toString();
	}

	/* (non-Javadoc)
	 * @see com.esri.arcgis.jtx.IJTXCustomStepSafe#invokeEditor(int, java.lang.String[])
	 */
	public String[] invokeEditor(int arg0, String[] arg1) throws IOException,
			AutomationException {
//		ArgEditor ed = new ArgEditor();
//		ed.ShowForm();
//		return arg1;
		throw new AutomationException(new Exception("No argument editor available"));
	}

	/* (non-Javadoc)
	 * @see com.esri.arcgis.jtx.IJTXCustomStepSafe#onCreate(com.esri.arcgis.jtx.IJTXDatabase)
	 */
	public void onCreate(IJTXDatabase ipDatabase) throws IOException,
			AutomationException {
		m_ipDatabase = ipDatabase;
	}

	/* (non-Javadoc)
	 * @see com.esri.arcgis.jtx.IJTXCustomStepSafe#validateArguments(java.lang.String[])
	 */
	public boolean validateArguments(String[] argv) throws IOException,
	AutomationException {
		// Get the arguments
		Boolean bHoldType = StepUtilities.getArguments(argv, "HoldType", true, new ArrayList<String>());
		if (!bHoldType)
		{
			return false;
		}

		return StepUtilities.areArgumentNamesValid(argv, expectedArgs);
	}

	public String getName() throws IOException,
			AutomationException {
		return "JTXJavaSteps.addJobHold";
	}
	
	public String getCLSID() throws IOException,
	AutomationException {
		return "3AC3478F-89B7-43be-A7DC-7349E5B305A8";
	}
}
