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
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;

using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;

namespace JTXSamples
{
    [Guid("162c7096-4546-4e62-9e8a-081fd202152b")]
    public class CreatePDF : IJTXCustomStep
    {
        // Arguments for this step are
        // initdir (optional) - initial dir to place the folder
        //      usage: /initdir:"C:\temp"
        // attach (optional) - if specified, file will be attached to job
        //      usage: /attach
        // resolution (optional) - if specified, the resolution used to pdf the doc
        //      usage: /resolution:600

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
        private IJTXDatabase m_ipDatabase = null;
        private readonly string[] m_expectedArgs = { "initdir", "attach", "resolution" };

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
                sb.AppendLine(@"Initial Directory:");
                sb.AppendFormat("\t/{0}:<initial directory> (optional)\r\n\r\n", m_expectedArgs[0]);
                sb.AppendLine(@"Attach File To Job:");
                sb.AppendFormat("\t/{0} (optional)\r\n\r\n", m_expectedArgs[1]);
                sb.AppendLine(@"Resolution (dpi):");
                sb.AppendFormat("\t/{0}:<dpi> (optional)\r\n", m_expectedArgs[2]);

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
        public int Execute(int JobID, int stepID, ref object[] argv, ref IJTXCustomStepFeedback ipFeedback)
        {
            if (JobID <= 0)
            {
                throw new ArgumentOutOfRangeException("JobID", JobID, "Job ID must be a positive value");
            }

            try
            {
                string strTemp = "";
                bool bAttach = false;
                if (StepUtilities.GetArgument(ref argv, m_expectedArgs[1], false, out strTemp))
                {
                    bAttach = true;
                }

                string strResolution;
                if (!StepUtilities.GetArgument(ref argv, m_expectedArgs[2], false, out strResolution))
                {
                    strResolution = "1200";
                }

                int iResolution = 1200;
                if (!Int32.TryParse(strResolution, out iResolution))
                {
                    iResolution = 1200;
                }

                string outputPath = "";
                if (!bAttach)
                {
                    SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                    pSaveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                    pSaveFileDialog.Title = "Choose output location...";

                    string strInitDir = "";
                    if (StepUtilities.GetArgument(ref argv, m_expectedArgs[0], true, out strInitDir))
                    {
                        if (System.IO.Directory.Exists(strInitDir))
                        {
                            pSaveFileDialog.InitialDirectory = strInitDir;
                        }
                    }

                    if (pSaveFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return -1;
                    }

                    outputPath = pSaveFileDialog.FileName;
                }

                string inputPath = JTXUtilities.SaveJobMXD(JobID);

                if (bAttach)
                {
                    outputPath = SystemUtilities.GetTemporaryFileLocation(inputPath, "pdf");
                }

                // delete output file if it already exists
                System.IO.FileInfo fi = new System.IO.FileInfo(outputPath);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                MapDocumentClass map = new MapDocumentClass();

                if (!map.get_IsMapDocument(inputPath)) throw new ApplicationException("Invalid map or specified map not found.");

                map.Open(inputPath, null);

                IActiveView pActiveView = (IActiveView)map.PageLayout;
                IExport pExport = new ExportPDFClass();

                pExport.ExportFileName = outputPath;

                tagRECT deviceFrameRect;
                deviceFrameRect.left = 0;
                deviceFrameRect.right = 800;
                deviceFrameRect.top = 0;
                deviceFrameRect.bottom = 600;

                pActiveView.ScreenDisplay.DisplayTransformation.set_DeviceFrame(ref deviceFrameRect);

                int iOutputResolution = iResolution;
                int iScreenResolution = 96;
                pExport.Resolution = iOutputResolution;
                IOutputRasterSettings pOutputRasterSettings = (IOutputRasterSettings)pExport;
                pOutputRasterSettings.ResampleRatio = 1;

                tagRECT exportRect;

                exportRect.left = pActiveView.ExportFrame.left * (iOutputResolution / iScreenResolution);
                exportRect.top = pActiveView.ExportFrame.top * (iOutputResolution / iScreenResolution);
                exportRect.right = pActiveView.ExportFrame.right * (iOutputResolution / iScreenResolution);
                exportRect.bottom = pActiveView.ExportFrame.bottom * (iOutputResolution / iScreenResolution);

                IEnvelope pPixelBoundsEnv = new EnvelopeClass();

                pPixelBoundsEnv.PutCoords(exportRect.left, exportRect.top, exportRect.right, exportRect.bottom);
                pExport.PixelBounds = pPixelBoundsEnv;

                int hdc = pExport.StartExporting();
                pActiveView.Output(hdc, iOutputResolution, ref exportRect, null, null);
                pExport.FinishExporting();
                pExport.Cleanup();

                if (bAttach)
                {
                    JTXUtilities.AddAttachmentToJob(JobID, outputPath, jtxFileStorageType.jtxStoreInDB);
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
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
            return StepUtilities.AreArgumentNamesValid(ref argv, m_expectedArgs);
        }

        #endregion

    }	// End Class
}	// End Namespace
