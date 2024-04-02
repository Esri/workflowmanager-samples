using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Workflow.Client.Steps;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WorkflowManagerSampleAddIn
{
    internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("WorkflowManagerSampleAddIn_Module");

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        /// <summary>
        /// Override this method to allow execution of DAML commands specified in this module.
        /// This is needed to run commands using the Open Pro Project Items step.
        /// </summary>
        /// <param name="id">The DAML control identifier.</param>
        /// <returns>A user defined function, with arguments, that will execute asynchronously when invoked.</returns>
        protected override Func<Object[], Task> ExecuteCommandArgs(string id)
        {
            return (object[] args) => RunCommand(id, args);
        }
        #endregion Overrides

        internal string JobId { get; private set; }
        private Task RunCommand(string id, object[] args)
        {
            return QueuedTask.Run(() =>
            {
                try
                {
                    // Get the jobId property from the ProMappingStep arguments
                    ProMappingStepCommandArgs stepArgs = (ProMappingStepCommandArgs)args[0];
                    JobId = stepArgs.JobId;
                    // ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"Got job id from ProMappingStep args: {JobId}", "Project Info");

                    // Run the command specified by the id
                    IPlugInWrapper wrapper = FrameworkApplication.GetPlugInWrapper(id);
                    var command = wrapper as ICommand;
                    if ((command != null) && command.CanExecute(null))
                        command.Execute(null);
                }
                catch (System.Exception e)
                {
                    // ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"ERROR: {e}", "Error running command");
                }
            });
        }
    }
}
