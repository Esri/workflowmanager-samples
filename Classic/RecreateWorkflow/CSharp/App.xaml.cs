using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RecreateWorkflowWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

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
                    Console.WriteLine("No Workflow Manager licenses available.");
                    return false;
                }
                else
                {
                    eStatus = ipAo.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeWorkflowManager);

                    if (eStatus != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        Console.WriteLine("Unable to check out Workflow Manager license.");
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CheckoutLicense();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            CheckinLicense();
        }
    }
}
