using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Task_Manager
{
    /// <summary>
    /// Interaction logic for EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        #region Fields
        private bool _closedByButton = false;//indicates if the window was closed by pressing the "Done"/"Cancal" buttons
        private ManagableTask _task = null;//the task we're editing, if it's null that indicates we're adding new tasks

        public ManagableTask FinalTask { get; private set; } = null;//when editing an existing task-contains the task's details after editing
        public List<ManagableTask> FinalTasks { get; private set; } = null;//when adding new tasks-contains the details of the new tasks

        private string _quitEdit = "Quit editing task?";
        private string _noChanges = "No changes were made";
        private string _taskEmpty = "Saving an empty task isn't allowed";
        private string _newDataNotSaved = "The data you entered will not be saved";
        private string _changesNotSaved = "Any changes will not be saved";
        #endregion

        #region Constructors
        public EditTaskWindow()
        {
            InitializeComponent();
        }
        public EditTaskWindow(ManagableTask task) : this()
        {
            _task = task;
        }
        #endregion

        #region Event Handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayTask();
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            bool isCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            if (e.Key == Key.Escape)
            {
                btnCancel_Click(this, null);
            }
            if (e.Key == Key.Enter && isCtrl)
            {
                btnDone_Click(this, null);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_closedByButton)//closed by user, no buttons pressed
            {
                bool isChanged = IsTaskChanged(out bool isEmpty);

                MessageBoxResult result;
                if (isChanged)//task changed-confirm if user wants to discard changes
                    result = MessageBox.Show(_task == null ? _newDataNotSaved : _changesNotSaved, string.Empty, MessageBoxButton.OKCancel);
                else//task not changed-confirm if user wants to quit editing task
                    result = MessageBox.Show(_quitEdit, string.Empty, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
                    e.Cancel = true;

                if (_task == null)
                    DialogResult = FinalTasks != null && FinalTasks.Count > 0;//if window was closed after adding new tasks(using the "Next" button), closing now only cancels the last one
            }
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            //done editing/adding tasks
            bool isEmpty;
            bool isChanged = IsTaskChanged(out isEmpty);

            if (isEmpty)//task empty-don't allow save
            {
                MessageBox.Show(_taskEmpty);
                return;
            }
            else if (!isChanged)//existing task not changed-treat that as a cancellation
            {
                MessageBox.Show(_noChanges);
                _closedByButton = true;
                DialogResult = false;
                Close();
                return;
            }

            if (_task != null)//editing an existing task
            {
                FinalTask = GenerateTaskFromCurrentData();
            }
            else//new task
            {
                AddTaskToFinalList();
            }

            _closedByButton = true;
            DialogResult = true;
            Close();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //done editing current new task, move on to next one
            bool isEmpty;
            //when the task is new(the only case in which btnNext is displayed), there's no difference between an empty task and an unchanged one-in both cases we don't allow it to be saved
            if (!IsTaskChanged(out isEmpty))
            {
                MessageBox.Show(_task == null ? _taskEmpty : _noChanges);
                return;
            }

            AddTaskToFinalList();
            DisplayTask();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            bool isEmpty;
            if (IsTaskChanged(out isEmpty))
            {
                //confirm if user wants to discard changes
                MessageBoxResult result = MessageBox.Show(_task == null ? _newDataNotSaved : _changesNotSaved, string.Empty, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            _closedByButton = true;
            if (_task == null && FinalTasks != null && FinalTasks.Count > 0)//if the window was opened for new tasks and we already confirmed some, we only cancel the last one
                DialogResult = true;
            else
                DialogResult = false;
            Close();
        }
        #endregion

        #region Graphic Functions
        private void DisplayTask()
        {
            if (_task != null)
            {
                //window opened to edit existing task
                btnNext.Visibility = Visibility.Collapsed;
                btnNext.IsEnabled = false;
                tbTitle.Text = _task.Title;
                tbDescription.Text = _task.Description;
            }
            else
            {
                tbTitle.Text = string.Empty;
                tbDescription.Text = string.Empty;
            }
        }
        #endregion

        #region Utility Functions
        private bool IsTaskChanged(out bool isEmpty)
        {
            isEmpty = IsTaskEmpty();

            if (_task == null)
                return !isEmpty;

            if (!_task.Title.Equals(tbTitle.Text))
                return true;
            if (!_task.Description.Equals(tbDescription.Text))
                return true;

            return false;
        }
        private bool IsTaskEmpty()
        {
            return string.IsNullOrEmpty(tbTitle.Text) && string.IsNullOrEmpty(tbDescription.Text);
        }

        private ManagableTask GenerateTaskFromCurrentData()
        {
            return new ManagableTask() { Title = tbTitle.Text, Description = tbDescription.Text };
        }
        private void AddTaskToFinalList()
        {
            if (FinalTasks == null)
                FinalTasks = new List<ManagableTask>();
            FinalTasks.Add(GenerateTaskFromCurrentData());
        }
        #endregion
    }
}