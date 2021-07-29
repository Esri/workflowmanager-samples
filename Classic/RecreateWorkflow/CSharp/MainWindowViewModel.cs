using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.JTX;

namespace RecreateWorkflowWPF
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public string Query { get; set; }
        public TrulyObservableCollection<WorkflowsToRecreate> WorkflowsToRecreate { get; set; } = new TrulyObservableCollection<WorkflowsToRecreate>();
        private bool showError = true;

        public bool ShowError
        {
            get { return showError; }
            set
            {
                showError = value;
                NotifyPropertyChanged("ShowError");
            }
        }


        public RelayCommand RecreateWorkflowCommand { get; private set; }
        public RelayCommand QueryCommand { get; private set; }
        public RelayCommand SelectCommand { get; private set; }

        private RecreateWorkflowModel model;

        public MainWindowViewModel()
        {
            try
            {
                model = new RecreateWorkflowModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to WMX database: " + ex.Message);
            }

            // Wire up commands
            QueryCommand = new RelayCommand(() =>
            {
                try
                {
                    var jobs = model.QueryJobs(Query);
                    WorkflowsToRecreate.Clear();
                    foreach (var job in jobs)
                    {
                        WorkflowsToRecreate.Add(new WorkflowsToRecreate() { Name = job.Name, Job = job });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                RecreateWorkflowCommand.OnCanExecuteChanged();
                SelectCommand.OnCanExecuteChanged();
            }, () => model != null);

            RecreateWorkflowCommand = new RelayCommand(() =>
            {
                foreach (var w in WorkflowsToRecreate.Where(x => x.Selected))
                {
                    var result = model.RecreateWorkflow(w.Job);
                    w.Recreated = result.Item1;
                    w.Reset = result.Item2;
                    w.Details = result.Item3;
                }

            }, () => WorkflowsToRecreate?.Where(x => x.Selected)?.Count() > 0);

            SelectCommand = new RelayCommand((param) =>
            {
                bool sel;
                if (!(param is string) || !bool.TryParse((string)param, out sel))
                    return;

                foreach (var w in WorkflowsToRecreate.Where(x => x.Selected != sel))
                {
                    w.Selected = sel;
                }
            }, (param) => { bool x; return param is string && bool.TryParse((string)param, out x) && WorkflowsToRecreate?.Count > 0; });
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


    }

    public class WorkflowsToRecreate : INotifyPropertyChanged
    {
        private bool selected = true;
        private bool recreated = false;
        private bool reset = false;
        private string details = null;

        public IJTXJob Job { get; set; }
        public string Name { get; set; }

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                NotifyPropertyChanged("Selected");
            }
        }

        public bool Recreated
        {
            get { return recreated; }
            set
            {
                recreated = value;
                NotifyPropertyChanged("Recreated");
            }
        }

        public bool Reset
        {
            get { return reset; }
            set
            {
                reset = value;
                NotifyPropertyChanged("Reset");
            }
        }


        public string Details
        {
            get { return details; }
            set
            {
                details = value;
                NotifyPropertyChanged("Details");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = (t) => execute();
            this.canExecute = (t) => canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? false;
        }

        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}