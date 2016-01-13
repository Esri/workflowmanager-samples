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
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;
using ESRI.ArcGIS.JTXUI;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace JTXSamples
{
    public class ImportADUsers
    {
        private static string[] argNames = new string [] { "db", "domain", "username", "password", "userGroup", "groupGroup" };

        [STAThread]
        static void Main(string[] args)
        {

            // Convert the arguments to objects
            object[] argv = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                argv[i] = args[i];
            }

            // Check if they want to display the usage message
            string tmpOut;
            if (StepUtilities.GetArgument(ref argv, "h", true, out tmpOut) || StepUtilities.GetArgument(ref argv, "?", true, out tmpOut))
            {
                PrintUsageMessage();
                return;
            }
            
            if (CheckoutLicense())
            {
                try
                {

                    IJTXDatabaseManager2 dbMgr = new JTXDatabaseManagerClass() as IJTXDatabaseManager2;
                    IJTXDatabase ipDB = null;
                    if (StepUtilities.GetArgument(ref argv, argNames[0], true, out tmpOut))
                    {
                        // Database was specified
                        try
                        {
                            ipDB = dbMgr.GetDatabase(tmpOut);
                        }
                        catch (COMException)
                        {
                            Console.WriteLine("{0} is not a valid Workflow Manager database alias", tmpOut);
                        }
                        Console.WriteLine("Using database {0}", tmpOut);
                    }
                    else
                    {
                        // Use the default database
                        Console.WriteLine("Using default database");
                        ipDB = dbMgr.GetActiveDatabase(false);
                    }

                    // Get the domain. If one is not specified, use the current domain
                    string strDomain;
                    if (StepUtilities.GetArgument(ref argv, argNames[1], true, out tmpOut))
                        strDomain = tmpOut;
                    else
                        strDomain = Environment.UserDomainName;

                    // Get the username
                    string strUsername = "";
                    if (StepUtilities.GetArgument(ref argv, argNames[2], true, out tmpOut))
                        strUsername = tmpOut;
                    
                    // Get the password
                    string strPassword = "";
                    if (strUsername != "" && StepUtilities.GetArgument(ref argv, argNames[3], true, out tmpOut))
                        strPassword = tmpOut;

                    // Get the userGroup.  If one is not specified, check the registry for the value the UI stored
                    string strUserGroup;
                    if (StepUtilities.GetArgument(ref argv, argNames[4], true, out tmpOut))
                        strUserGroup = tmpOut;
                    else
                        strUserGroup = GetGroupFromReg("UserADGroup");

                    // Get the groupGroup.  If one is not specified, check the registry for the value the UI stored
                    string strGroupGroup;
                    if (StepUtilities.GetArgument(ref argv, argNames[5], true, out tmpOut))
                        strGroupGroup = tmpOut;
                    else
                        strGroupGroup = GetGroupFromReg("GroupADGroup");

                    if (String.IsNullOrEmpty(strUserGroup) || String.IsNullOrEmpty(strGroupGroup))
                    {
                        Console.WriteLine("Error: Empty userGroup or groupGroup");
                        return;
                    }

                    ConfigurationCache.InitializeCache(ipDB);

                    // Synchronize
                    int groupCount, userCount;
                    ActiveDirectoryHelper.SyncronizeJTXDatabaseWithActiveDirectory(ipDB, strDomain, strUsername, strPassword,
                        strUserGroup, strGroupGroup, out groupCount, out userCount);

                    Console.WriteLine("Successfully imported {0} users in {1} groups", userCount, groupCount);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed with error: " + e.Message + e.StackTrace);
                }
                finally
                {
                    CheckinLicense();
                }
            }
            else
                Console.WriteLine("Could not checkout license... exiting");
        }

        private static void PrintUsageMessage()
        {
            Console.WriteLine("This application will syncronize the ArcGIS Worflow Manager users and groups with the");
            Console.WriteLine("users and groups in the specified ActiveDirectory groups" );
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("\t/{0}:<databaseAlias> - The ArcGIS Workflow Manager database alias to connect to (optional)", argNames[0]);
            Console.WriteLine("\t/{0}:<domain> - The Active Directory domain to connect to (optional)", argNames[1]);
            Console.WriteLine("\t/{0}:<username> - The Active Directory username to connect with (optional)", argNames[2]);
            Console.WriteLine("\t/{0}:<password> - The Active Directory users password (optional)", argNames[3]);
            Console.WriteLine("\t/{0}:<userGroup> - The Active Directory group containing the list of ArcGIS Workflow Manager users (optional)", argNames[4]);
            Console.WriteLine("\t/{0}:<groupGroup> - The Active Directory group containing the list of ArcGIS Workflow Manager groups (optional)", argNames[5]);
        }

        private static string GetGroupFromReg(string propName)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\ESRI\\JTX\\ActiveDirectorySettings", true);
            if (regKey == null)
            {
                return "";
            }

            return (string)regKey.GetValue(propName);
        }


        /// <summary>
        /// Checks out an ArcGIS Desktop license
        /// </summary>
        /// <returns>bool</returns>
        public static bool CheckoutLicense()
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
        public static void CheckinLicense()
        {
            Console.WriteLine("Checking in licenses...");
            new AoInitializeClass().Shutdown();
        }

    }
}
