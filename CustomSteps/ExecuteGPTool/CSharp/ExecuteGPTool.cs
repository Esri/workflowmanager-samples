/*Copyright 2015 Esri
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.?*/
using System;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

#if (!SERVER)
using ESRI.ArcGIS.GeoprocessingUI;
#endif


namespace JTXSamples
{
    [Guid("76F3BC8F-7941-46b9-94F9-0856A6E86E87")]
    public class ExecuteGPTool : IJTXCustomStep
    {
        internal const string ARG_TOOLBOXPATH = "toolboxpath";
        internal const string ARG_TOOL = "tool";
        internal const string ARG_PARAM = "param";
        internal const string ARG_ATTACH = "attach";

        #region Registration Code
        [ComRegisterFunction()]
        static void Reg(String regKey)
        {
            ESRI.ArcGIS.JTX.Utilities.JTXUtilities.RegisterJTXCustomStep(regKey);
        }

        [ComUnregisterFunction()]
        static void Unreg(String regKey)
        {
            ESRI.ArcGIS.JTX.Utilities.JTXUtilities.UnregisterJTXCustomStep(regKey);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////
        // DECLARE: Data Members
        private readonly string[] m_expectedArgs = { ARG_TOOLBOXPATH, ARG_TOOL, ARG_PARAM, ARG_ATTACH };
        private IJTXDatabase m_ipDatabase = null;
        private StringBuilder m_pStrLogMessages = null;

        #region IJTXCustomStep Members

        /// <summary>
        /// A description of the expected arguments for the step type.  This should
        /// include the syntax of the argument, whether or not it is required/optional, 
        /// and any return codes coming from the step type.
        /// </summary>
        public string ArgumentDescriptions
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"Toolbox Path (explicit full path to the tbx file combined with toolbox name):");
                sb.AppendFormat("\t/{0}:<{0}> (required)\r\n", ARG_TOOLBOXPATH);
                sb.AppendLine(@"Display Name of the Tool in the toolbox:");
                sb.AppendFormat("\t/{0}:<{0}> (required)\r\n", ARG_TOOL);
                sb.AppendLine(@"Parameter to override on the tool (can be specified multiple times):");
                sb.AppendFormat("\t/{0}:<ParamName>:<ParamValue> (optional)\r\n", ARG_PARAM);
                sb.AppendLine(@"Flag to attach the log to the job once the tool has completed:");
                sb.AppendFormat("\t/{0} (optional)\r\n", ARG_ATTACH);


                return sb.ToString();
            }
        }

        /// <summary>
        /// Called when a step of this type is executed in the workflow.
        /// </summary>
        /// <param name="JobID">ID of the job being executed</param>
        /// <param name="StepID">ID of the step being executed</param>
        /// <param name="argv">Array of arguments passed into the step's execution</param>
        /// <param name="ipFeedback">Feedback object to return status messages and files</param>
        /// <returns>Return code of execution for workflow path traversal</returns>
        public int Execute(int jobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {
#if (!SERVER)
            StatusForm sfd = null;
#endif
            bool success = true;

            // Reset the message logging
            this.ClearCachedLogMessages();

            try
            {
                Log("ExecuteGPTool.Execute beginning..");

                // Define variables
                string strToolboxPath = "";
                string strTool = "";
                string strToolboxRoot = "";
                string strToolboxName = "";

                // Get the toolbox name (check for errors)
                if (!StepUtilities.GetArgument(ref argv, "toolboxpath", true, out strToolboxPath) || strToolboxPath.Equals(String.Empty))
                {
                    success = false;
                    Log("Error getting 'toolboxpath' argument");
#if (!SERVER)
                    MessageBox.Show("Missing toolbox argument.");
#endif
                }
                // Get the tool name (check for errors)
                else if (!StepUtilities.GetArgument(ref argv, "tool", true, out strTool) || strTool.Equals(String.Empty))
                {
                    success = false;
                    Log("Error getting 'tool' argument");
#if (!SERVER)
                    MessageBox.Show("Missing tool argument.");
#endif
                }
                else
                {
#if (!SERVER)
                    // Display status dialog
                    sfd = new StatusForm();
                    sfd.ToolName = strTool;
                    sfd.Show();
                    sfd.Refresh();
#endif

                    // Get directory of toolbox
                    strToolboxRoot = Path.GetDirectoryName(strToolboxPath);
                    // Get toolbox name without .tbx extension
                    strToolboxName = Path.GetFileNameWithoutExtension(strToolboxPath);

                    // Open toolbox and tool
                    IWorkspaceFactory pToolboxWorkspaceFactory = new ToolboxWorkspaceFactoryClass();
                    IToolboxWorkspace pToolboxWorkspace = pToolboxWorkspaceFactory.OpenFromFile(strToolboxRoot, 0) as IToolboxWorkspace;
                    IGPToolbox pGPToolbox = pToolboxWorkspace.OpenToolbox(strToolboxName);
                    IGPTool pGPTool = pGPToolbox.OpenTool(strTool);

                    Log("ExecuteGPTool.Execute successfully opened Tool " + strTool + " in Toolbox " + strToolboxPath + "..");

                    // Generate the arrays for parameters
                    IVariantArray parameters = new VarArrayClass();  // For Execute method
                    ESRI.ArcGIS.esriSystem.IArray pParameterArray = pGPTool.ParameterInfo;  // For Validate and InvokeModal methods

                    // Get parameter "pairs"; "GetDoubleArguments" supports the GP-style,
                    // two-colon argument strings, like "/param:tool_param_name:tool_param_value"
                    string[] argNames;
                    string[] argValues;
                    if (StepUtilities.GetDoubleArguments(ref argv, "param", true, out argNames, out argValues))
                    {
                        // Stash away the variables that were passed in
                        Dictionary<string, string> argTable = new Dictionary<string, string>();
                        for (int i = 0; i < argNames.Length; i++)
                        {
                            string uppercaseName = argNames[i].ToUpper();
                            argTable[uppercaseName] = argValues[i];
                        }

                        // The GP tool's parameter list may include parameters that weren't specified in the
                        // JTX step arguments.  So iterate through the tool's parameters, setting the values
                        // from the input where you can.  Where no value was passed in, just skip the
                        // parameter and go with the default.
                        for (int i = 0; i < pParameterArray.Count; i++)
                        {
                            IGPParameter pGPParam = (IGPParameter)pParameterArray.get_Element(i);
                            IGPParameterEdit pGPParamEdit = (IGPParameterEdit)pGPParam;
                            string uppercaseName = pGPParam.Name.ToUpper();

                            // Override the default value, if something was passed in
                            if (argTable.ContainsKey(uppercaseName))
                            {
                                IGPDataType pGPType = pGPParam.DataType;
                                string strValue = argTable[uppercaseName];
                                pGPParamEdit.Value = pGPType.CreateValue(strValue);

                                Log("ExecuteGPTool.Execute Tool Parameter = " + uppercaseName + ", Value = " + strValue + "..");
                            }

                            // Always stash away the current parameter value, since we need the complete list
                            // for "Execute" below.
                            parameters.Add(pGPParam.Value);
                        }
                    }

                    Log("ExecuteGPTool.Execute successfully setup parameter values..");


                    // Initialize the geoprocessor
                    IGeoProcessor pGP = new GeoProcessorClass();

                    // Create callback object for GP messages
                    JTXGPCallback pCallback = new JTXGPCallback();

                    // Set up the geoprocessor
                    pGP.AddToolbox(strToolboxPath);
                    pGP.RegisterGeoProcessorEvents(pCallback);

                    Log("ExecuteGPTool.Execute created and registered GeoProcessor..");

                    // Create the messages object and a bool to pass to InvokeModal method
                    IGPMessages pGPMessages = pGPTool.Validate(pParameterArray, true, null);
                    IGPMessages pInvokeMessages = new GPMessagesClass();
                    string strMessage = "";

                    // Check for error messages
                    if (pGPMessages.MaxSeverity == esriGPMessageSeverity.esriGPMessageSeverityError)
                    {
#if (!SERVER)
                        // Only want to invoke a modal dialog if we're running on a workstation
                        // Set a reference to IGPCommandHelper2 interface
                        IGPToolCommandHelper2 pToolHelper = new GPToolCommandHelperClass() as IGPToolCommandHelper2;
                        pToolHelper.SetTool(pGPTool);
                        bool pOK = true;

                        // Open tool GUI
                        pToolHelper.InvokeModal(0, pParameterArray, out pOK, out pInvokeMessages);
                        if (pOK == true)
                        {
                            bool bFailureMessages;
                            strMessage = ConvertGPMessagesToString(pInvokeMessages, out bFailureMessages);
                            success = !bFailureMessages;
                        }
                        else
                        {
                            success = false;
                        }
#else
                        Log("ExecuteGPTool.Execute Tool Validate failed..");

                        // If we're running on a server, then just indicate a failure.  (Someone will
                        // have to use the JTX application to fix the step arguments, if they can be
                        // fixed.)
                        success = false;
#endif
                    }
                    else  // If there are no error messages, execute the tool
                    {
                        Log("ExecuteGPTool.Execute successfully validated parameter values, About to Execute..");
                        IGPMessages ipMessages = new GPMessagesClass();
                        try
                        {
                            pGPTool.Execute(pParameterArray, null, new GPEnvironmentManagerClass(), ipMessages);
                            Log("ExecuteGPTool.Execute completed call to pGPTool.Execute()");
                        }
                        catch (System.Runtime.InteropServices.COMException ex)
                        {
                            success = false;
                            Log("ExecuteGPTool.Execute Tool Execute failed, Message = " + ex.Message + ", DataCode = " + ex.ErrorCode + ", Data = " + ex.StackTrace);
                        }

                        // Get Messages
                        bool bFailureMessages = false;
                        strMessage += this.ConvertGPMessagesToString(ipMessages, out bFailureMessages);

                        Log("ExecuteGPTool.Execute got messages from tool");
                        Log("*** GP MESSAGES ***" + System.Environment.NewLine + strMessage + System.Environment.NewLine);
                        Log("*** END GP MESSAGES ***");

                        // If tool failed during execution, indicate a failure
                        if (pGP.MaxSeverity == (int)esriGPMessageSeverity.esriGPMessageSeverityError)
                        {
                            success = false;
                            Log("ExecuteGPTool.Execute Found Error messages from the tool..");
                        }
                    }

                    // Call AttachMsg
                    try
                    {
                        IJTXJobManager ipJobManager = m_ipDatabase.JobManager;
                        IJTXJob ipJob = ipJobManager.GetJob(jobID);
                        AttachMsg(ipJob, strTool, this.GetCachedLogMessages());
                        this.ClearCachedLogMessages();
                    }
                    catch (Exception ex)
                    {
                        string strEx = ex.Message;
                        Log("Caught exception: " + ex.Message);
                    }
                    
                    pGP.UnRegisterGeoProcessorEvents(pCallback);
                }
            }
            catch (Exception ex2)
            {
                success = false;
                Log("Caught exception: " + ex2.Message);
#if (!SERVER)
                string msg = "";
                if (ex2.InnerException == null)
                {
                    msg = "Inner Exception is null";
                }
                else
                {
                    msg = "Inner Exception is not null";
                }

                MessageBox.Show("Stack Trace: " + ex2.StackTrace + Environment.NewLine + "Message: " + ex2.Message + Environment.NewLine + "Source: " + ex2.Source + Environment.NewLine + msg);
#endif
            }

            // Clean up
#if (!SERVER)
            if (sfd != null)
            {
                sfd.Close();
            }
#endif

            // Indicate success or failure
            if (success == true)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Invoke an editor tool for managing custom step arguments.  This is
        /// an optional feature of the custom step and may not be implemented.
        /// </summary>
        /// <param name="hWndParent">Handle to the parent application window</param>
        /// <param name="argsIn">Array of arguments already configured for this custom step</param>
        /// <returns>Returns a list of newely configured arguments as specified via the editor tool</returns>
        public object[] InvokeEditor(int hWndParent, object[] argsIn)
        {
            throw new NotImplementedException("No edit dialog available for this step type");
        }

        /// <summary>
        /// Called when the step is instantiated in the workflow.
        /// </summary>
        /// <param name="ipDatabase">Database connection to the JTX repository.</param>
        public void OnCreate(IJTXDatabase ipDatabase)
        {
            m_ipDatabase = ipDatabase;
            m_pStrLogMessages = new StringBuilder();

            Log("JTXTempLog: ExecuteGPTool.OnCreate Initializing logging..");
        }

        /// <summary>
        /// Method to validate the configured arguments for the step type.  The
        /// logic of this method depends on the implementation of the custom step
        /// but typically checks for proper argument names and syntax.
        /// </summary>
        /// <param name="argv">Array of arguments configured for the step type</param>
        /// <returns>Returns 'true' if arguments are valid, 'false' if otherwise</returns>
        public bool ValidateArguments(ref object[] argv)
        {
            return true;
        }

        #endregion

        #region Helper Methods

        private void AttachMsg(IJTXJob ipJob, string strTool, string strMessage)
        {
            // Attach the messages as a file
            string strFileName = strTool + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_Results.log";
            string strPath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), strFileName);

            using (TextWriter pTextWriter = new StreamWriter(strPath))
            {
                pTextWriter.Write(strMessage);
                pTextWriter.Close();
            }

            if (File.Exists(strPath))
            {
                try
                {
                    Log("JTXTempLog: ExecuteGPTool.AttachMsg adding Job Attachment..");
                    ipJob.AddAttachment(strPath, jtxFileStorageType.jtxStoreInDB, "");
                    Log("JTXTempLog: ExecuteGPTool.AttachMsg added Job Attachment..");
                }
                catch (Exception ex)
                {
                    String strMsg = ex.Message;
                }
                File.Delete(strPath);
            }
        }

        /// <summary>
        /// Helper function to log messages out tools running through this tool.
        /// </summary>
        /// <param name="s">The logging string to be captured.</param>
        private void Log(string s)
        {
            string dateString = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            string msgString = dateString + ": ExecuteGPTool: " + s;

            m_pStrLogMessages.AppendLine(msgString);
            m_ipDatabase.LogMessage(3, 1000, msgString);
        }

        /// <summary>
        /// Retrieves the logging messages cached by this tool.
        /// </summary>
        /// <returns>A single string containing the messages.</returns>
        private string GetCachedLogMessages()
        {
            return m_pStrLogMessages.ToString();
        }

        /// <summary>
        /// Clears the logging messages captured by this tool.
        /// </summary>
        private void ClearCachedLogMessages()
        {
            m_pStrLogMessages.Remove(0, m_pStrLogMessages.Length);
        }

        /// <summary>
        /// Takes an IGPMessages object and converts it to a nicely-formatted string.
        /// </summary>
        /// <param name="messages">An IGPMessages object containing one or more messages.</param>
        /// <param name="bFailureMessages">Set to true if any failure messages (aborts or errors) were detected; false otherwise</param>
        /// <returns>A string formatted in the GP-style message.</returns>
        private string ConvertGPMessagesToString(IGPMessages messages, out bool bFailureMessages)
        {
            string msgsAsString = String.Empty;
            StringBuilder sb = new StringBuilder();
            bFailureMessages = false;

            if (messages != null)
            {
                // Iterate through each of the messages
                for (int i = 0; i < messages.Count; i++)
                {
                    IGPMessage message = messages.GetMessage(i);

                    if ((message != null) && !string.IsNullOrEmpty(message.Description))
                    {
                        string strType = "";
                        switch (message.Type)
                        {
                            case esriGPMessageType.esriGPMessageTypeAbort:
                                strType = "Abort:";
                                bFailureMessages = true;
                                break;
                            case esriGPMessageType.esriGPMessageTypeEmpty:
                                strType = "Empty:";
                                break;
                            case esriGPMessageType.esriGPMessageTypeError:
                                strType = "Error:";
                                bFailureMessages = true;
                                break;
                            case esriGPMessageType.esriGPMessageTypeInformative:
                                strType = "Info:";
                                break;
                            case esriGPMessageType.esriGPMessageTypeProcessDefinition:
                                strType = "ProcessDef:";
                                break;
                            case esriGPMessageType.esriGPMessageTypeProcessStart:
                                strType = "ProcessStart:";
                                break;
                            case esriGPMessageType.esriGPMessageTypeProcessStop:
                                strType = "ProcessStop:";
                                break;
                            case esriGPMessageType.esriGPMessageTypeWarning:
                                strType = "Warning:";
                                break;
                        }

                        sb.AppendLine(strType + " " + message.Description);
                    }
                }
            }

            return sb.ToString();
        }
        #endregion

        // Private callback class
        private class JTXGPCallback : IGeoProcessorEvents
        {
            private string m_strMessages = "";
            private bool bFailureMessages = false;

            public string Messages
            {
                get { return m_strMessages; }
            }

            public bool FailureMessages
            {
                get
                {
                    return bFailureMessages;
                }
            }

            #region IGeoProcessorEvents Members

            public void OnMessageAdded(IGPMessage message)
            {
                if ((message != null) && !string.IsNullOrEmpty(message.Description))
                {
                    string strType = "";
                    switch (message.Type)
                    {
                        case esriGPMessageType.esriGPMessageTypeAbort:
                            strType = "Abort:";
                            bFailureMessages = true;
                            break;
                        case esriGPMessageType.esriGPMessageTypeEmpty:
                            strType = "Empty:";
                            break;
                        case esriGPMessageType.esriGPMessageTypeError:
                            strType = "Error:";
                            bFailureMessages = true;
                            break;
                        case esriGPMessageType.esriGPMessageTypeInformative:
                            strType = "Info:";
                            break;
                        case esriGPMessageType.esriGPMessageTypeProcessDefinition:
                            strType = "ProcessDef:";
                            break;
                        case esriGPMessageType.esriGPMessageTypeProcessStart:
                            strType = "ProcessStart:";
                            break;
                        case esriGPMessageType.esriGPMessageTypeProcessStop:
                            strType = "ProcessStop:";
                            break;
                        case esriGPMessageType.esriGPMessageTypeWarning:
                            strType = "Warning:";
                            break;
                    }

                    m_strMessages += strType + " " + message.Description + Environment.NewLine;
                }
            }

            public void PostToolExecute(IGPTool Tool, IArray Values, int result, IGPMessages Messages)
            {

            }

            public void PreToolExecute(IGPTool Tool, IArray Values, int processID)
            {

            }

            public void ToolboxChange()
            {

            }

            #endregion
        }

    }	// End Class
}	// End Namespace
