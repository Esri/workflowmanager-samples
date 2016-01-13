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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;


namespace JTXSamples
{
    internal class JTXOverdueNotification
    {
        static void Main(string[] args)
        {
            JTXOverdueNotification prog = new JTXOverdueNotification();
            if (prog.CheckoutLicense())
            {
                try
                {
                    // Arguments list
                    // /NotifType:<Notification type to send>
                    // example: JTXOverdueNotification.exe /NotifType:OverdueJob

                    object[] pArgObjects = args as object[];

                    // Get some variables ready
                    string sNotificationTypeName = "";

                    StepUtilities.GetArgument(ref pArgObjects, "NotifType", true, out sNotificationTypeName);
                    if (sNotificationTypeName == "")
                    {
                        Console.WriteLine("A notification type must be entered.");
                        return;
                    }

                    IJTXDatabaseManager jtxDBMan = new JTXDatabaseManagerClass();
                    IJTXDatabase pJTXDB = jtxDBMan.GetActiveDatabase(false);
                    IJTXConfiguration pJTXConfig = pJTXDB.ConfigurationManager;

                    // Create a simple query to find jobs that were due before today
                    IQueryFilter pQF = new QueryFilterClass();

                    // NOTE #1: Verify the date format matches your selected RDBMS
                    // NOTE #2: Verify the status id for 'Closed' with the JTX Administrator
                    pQF.WhereClause = "DUE_DATE < '" + DateTime.Today.ToString() + "'" + " AND STATUS <> 9";
                    Console.WriteLine(pQF.WhereClause);

                    // Get the notification type for the notification that will be sent
                    IJTXNotificationConfiguration pNotificationConfig = pJTXConfig as IJTXNotificationConfiguration;
                    IJTXNotificationType pNotificationType = pNotificationConfig.GetNotificationType(sNotificationTypeName);
                    if (pNotificationType == null)
                    {
                        Console.WriteLine("Please enter a valid notification type.");
                        return;
                    }

                    // Get the job manager to execute the query and find the jobs in question
                    IJTXJobManager pJobManager = pJTXDB.JobManager;
                    IJTXJobSet pJobs = pJobManager.GetJobsByQuery(pQF);

                    pJobs.Reset();
                    for (int a = 0; a < pJobs.Count; a++)
                    {
                        IJTXJob pJob = pJobs.Next();
                        Console.WriteLine(pJob.Name);

                        // Send it!
                        JTXUtilities.SendNotification(sNotificationTypeName, pJTXDB, pJob, null);
                    }
                }
                catch (Exception except)
                {
                    Console.WriteLine("An error occurred: " + except.Message);
                }
                prog.CheckinLicense();
                Console.WriteLine("Completed.");
            }
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
