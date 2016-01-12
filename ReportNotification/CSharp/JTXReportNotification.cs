using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;


namespace JTXSamples
{
    internal class JTXReportNotification
    {
        static void Main(string[] args)
        {
            JTXReportNotification prog = new JTXReportNotification();
            if (prog.CheckoutLicense())
            {
                // Arguments list
                // /ReportID:<Report ID to execute>
                // /NotifType:<Notification type to send>
                // example: JTXReportNotification.exe /NotifType:ReportNotification /ReportID:401

                object[] pArgObjects = args as object[];

                // Get some variables ready
                int iReportID = 0;
                string sReportID = "";
                string sNotificationTypeName = "";

                StepUtilities.GetArgument(ref pArgObjects, "ReportID", true, out sReportID);
                if (!int.TryParse(sReportID, out iReportID))
                {
                    Console.WriteLine("Invalid Report ID entered");
                    return;
                }
                StepUtilities.GetArgument(ref pArgObjects, "NotifType", true, out sNotificationTypeName);
                if (sNotificationTypeName == "")
                {
                    Console.WriteLine("A notification type must be entered.");
                }
                
                IJTXDatabaseManager jtxDBMan = new JTXDatabaseManagerClass();
                IJTXDatabase pJTXDB = jtxDBMan.GetActiveDatabase(false);
                IJTXConfiguration2 jtxConfig = pJTXDB.ConfigurationManager as IJTXConfiguration2;

                string sReportOutput = prog.RunReport(jtxConfig, iReportID);

                // if there's output, send the notification
                if (sReportOutput != "")
                {
                    IJTXNotificationConfiguration pNotificationConfig = (IJTXNotificationConfiguration)jtxConfig;
                    IJTXNotificationType pNotificationType = pNotificationConfig.GetNotificationType(sNotificationTypeName);

                    if (pNotificationType == null)
                    {
                        Console.WriteLine("Please enter a valid notification type.");
                        return;
                    }

                    // Update the message
                    string sMessageBefore = pNotificationType.MessageTemplate;
                    pNotificationType.MessageTemplate = sReportOutput;
                    pNotificationType.Store();
                    
                    // Send it!
                    JTXUtilities.SendNotification(sNotificationTypeName, pJTXDB, null, null);

                    // Set the message back.
                    pNotificationType.MessageTemplate = "";
                    pNotificationType.Store();
                }
                else Console.WriteLine("Please enter a valid report ID.");

                prog.CheckinLicense();
            }
        }

        /// <summary>
        /// Executes the requested Report
        /// </summary>
        /// <param name="pJTXConfig"></param>
        /// <param name="iReportID"></param>
        /// <returns></returns>
        public string RunReport(IJTXConfiguration2 pJTXConfig, int iReportID)
        {
            string sReportOutput = "";

            IJTXReportManager pReportManager = pJTXConfig.ReportManager;
            IJTXReportSet pReports = pReportManager.GetReports();

            if (pReports.Count == 0)
            {
                Console.WriteLine("No reports configured in the JTX system");
                return "";
            }

            pReports.Reset();
            for (int a = 0; a < pReports.Count; a++)
            {
                IJTXReport pReport = pReports.Next();
                if (pReport.ID == iReportID)
                {
                    sReportOutput = pReport.GenerateFormattedReport();
                    return sReportOutput;
                }
            }
            
            return "";
        }

        /// <summary>
        /// Checks out an ArcGIS Desktop license
        /// </summary>
        /// <returns>bool</returns>
        public bool CheckoutLicense()
        {
            bool isBound = ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
            if (!isBound)
            {
                Console.WriteLine("Could not bind to ArcGIS Desktop:");
                return isBound;
            }

            IAoInitialize ipAo = new AoInitializeClass();
            if (ipAo == null)
            {
                ipAo = new AoInitializeClass();
            }

            esriLicenseStatus eStatus = esriLicenseStatus.esriLicenseNotLicensed;
            esriLicenseProductCode eProductCode = esriLicenseProductCode.esriLicenseProductCodeStandard;

            if ((eStatus = ipAo.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeStandard)) == esriLicenseStatus.esriLicenseAvailable)
            {
                eStatus = ipAo.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard);
                eProductCode = esriLicenseProductCode.esriLicenseProductCodeStandard;
            }
            else if ((eStatus = ipAo.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeAdvanced)) == esriLicenseStatus.esriLicenseAvailable)
            {
                eStatus = ipAo.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
                eProductCode = esriLicenseProductCode.esriLicenseProductCodeAdvanced;
            }
            else
            {
                Console.WriteLine("No ArcGIS Licenses available");
                return false;
            }

            if (eStatus != esriLicenseStatus.esriLicenseCheckedOut)
            {
                Console.WriteLine("Unable to check out ArcGIS license.");
                return false;
            }

            if (!ipAo.IsExtensionCheckedOut(esriLicenseExtensionCode.esriLicenseExtensionCodeWorkflowManager))
            {
                eStatus = ipAo.IsExtensionCodeAvailable(eProductCode, esriLicenseExtensionCode.esriLicenseExtensionCodeWorkflowManager);
                if (eStatus != esriLicenseStatus.esriLicenseAvailable)
                {
                    Console.WriteLine("No JTX licenses available.");
                    return false;
                }
                else
                {
                    eStatus = ipAo.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeWorkflowManager);

                    if (eStatus != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        Console.WriteLine("Unable to check out JTX license.");
                        return false;
                    }
                }
            }
            Console.WriteLine("Successfully checked out licenses.");
            return true;
        }

        /// <summary>
        /// Checks in all licenses
        /// </summary>
        public void CheckinLicense()
        {
            Console.WriteLine("Checking in licenses...");
            new AoInitializeClass().Shutdown();
        }

    }
}
