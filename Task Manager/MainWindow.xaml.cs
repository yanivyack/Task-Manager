using BusinessLogic.Models;
using BusinessLogic.ViewModels;
using Microsoft.Win32;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Task_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private TaskList taskList;

        private enum TaskFilter { All = 0, Done = 1, NotDone = 2 }
        private TaskFilter _taskFilter;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            taskList = new TaskList(new FileDbManager.FileDbManager());
        }

        #region Event Handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadTasks())
            {
                DisplayTasks();
            }
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            bool isCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            if (e.Key == Key.N && isCtrl)
            {
                btnNewTask_Click(this, null);
            }
            if (e.Key == Key.Enter && isCtrl)
            {
                btnEditTask_Click(this, null);
            }
            if (e.Key == Key.Delete)
            {
                btnDeleteTask_Click(this, null);
            }
            if (e.Key == Key.Space)
            {
                ManagableTask task = GetSelectedTask();
                if (task != null)
                {
                    task.Done = !task.Done;
                    DisplayTasks();
                }
            }
            if (_taskFilter == TaskFilter.All)
            {
                //int selectedIndex = lvTaskList.SelectedIndex;
                if (e.Key == Key.Up && isCtrl)
                {
                    PrioritizeTask(GetSelectedTask());
                    //lvTaskList.SelectedIndex = selectedIndex - 1;
                    lvTaskList.SelectedIndex--;
                }
                if (e.Key == Key.Down && isCtrl)
                {
                    DeprioritizeTask(GetSelectedTask());
                    //lvTaskList.SelectedIndex = selectedIndex + 1;
                    lvTaskList.SelectedIndex++;
                }
            }
            if (e.Key == Key.S && isCtrl)
            {
                btnSave_Click(this, null);
            }
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result;
            bool close = false;

            if (taskList.ChangesMade)
            {
                result = MessageBox.Show("Save changes?", string.Empty, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)//save and exit
                {
                    SaveTasks();
                    close = true;
                }
                else if (result == MessageBoxResult.No)//don't save
                {
                    close = true;
                }
                //else-don't allow exit, close is already set to false
            }
            else
            {
                result = MessageBox.Show("Close Program?", string.Empty, MessageBoxButton.YesNo);
                close = result == MessageBoxResult.Yes;
            }

            if (!close)
                e.Cancel = true;
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if (cmbFilter.SelectedItem == null)
                return;

            ComboBoxItem selectedItem = cmbFilter.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
                return;

            string itemName = selectedItem.Name;

            if (itemName.Equals("All"))
                _taskFilter = TaskFilter.All;
            else if (itemName.Equals("Done"))
                _taskFilter = TaskFilter.Done;
            else
                _taskFilter = TaskFilter.NotDone;

            DisplayTasks();
        }

        private void btnPrioritize_Click(object sender, RoutedEventArgs e)
        {
            //this event handler is attached to an event from an additional control(SingleTaskDisplay), so we need to use it to find the relevant task
            if (sender == null)
                return;

            FrameworkElement few = sender as FrameworkElement;
            if (few == null)
                return;

            ManagableTask task = few.DataContext as ManagableTask; ;

            PrioritizeTask(task);
        }
        private void btnDeprioritize_Click(object sender, RoutedEventArgs e)
        {
            //this event handler is attached to an event from an additional control(SingleTaskDisplay), so we need to use it to find the relevant task
            if (sender == null)
                return;

            FrameworkElement few = sender as FrameworkElement;
            if (few == null)
                return;

            ManagableTask task = few.DataContext as ManagableTask; ;

            DeprioritizeTask(task);
        }

        private void cbDone_CheckChanged(object sender, RoutedEventArgs e)
        {
            //a task's status was changed-if the display is filtered to only display done/unfinished tasks it needs to be refreshed

            //this event handler is attached to an event from an additional control(SingleTaskDisplay), so we need to use it to find the relevant task
            if (_taskFilter == TaskFilter.All)//if all tasks are displayer, there's no need to adjust the filter
                return;

            if (sender == null)
                return;

            FrameworkElement few = sender as FrameworkElement;
            if (few == null)
                return;

            ManagableTask task = few.DataContext as ManagableTask; ;
            if (task == null)
                return;

            DisplayTasks();
        }

        private void btnNewTask_Click(object sender, RoutedEventArgs e)
        {
            EditTaskWindow window = new EditTaskWindow();
            bool? dialogResult = window.ShowDialog();

            if (dialogResult == true)
            {
                bool addedAny = false;

                List<ManagableTask> newTasks = window.FinalTasks;
                if (newTasks != null)
                {
                    foreach (ManagableTask task in newTasks)
                    {
                        Exception ex;
                        if (!taskList.AddTask(task.Title, task.Description, -1, out ex))
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            LogManager.LogManager.LogException(ex);
                        }
                        else
                        {
                            addedAny = true;
                        }
                    }
                }

                if (addedAny)
                    DisplayTasks();
            }
        }
        private void btnEditTask_Click(object sender, RoutedEventArgs e)
        {
            ManagableTask task = GetSelectedTask();
            if (task == null)
                return;

            EditTaskWindow window = new EditTaskWindow(task);
            bool? dialogResult = window.ShowDialog();

            if (dialogResult == true)
            {
                if (window.FinalTask != null)
                {
                    task.Title = window.FinalTask.Title;
                    task.Description = window.FinalTask.Description;

                    DisplayTasks();
                }
            }
        }
        private void btnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            ManagableTask task = GetSelectedTask();
            if (task != null)
            {
                Exception ex;
                if (!taskList.DeleteTask(task, out ex))
                {
                    if (ex != null)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LogManager.LogManager.LogException(ex);
                    }
                }
                else
                {
                    DisplayTasks();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveTasks();
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF|*.PDF";
            bool? result = sfd.ShowDialog();
            if (result == true)
            {
                Exception ex;
                if (!taskList.ExportToPdf(sfd.FileName, out ex))
                {
                    if (ex != null)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LogManager.LogManager.LogException(ex);
                    }
                }
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion


        #region Data Functions
        private bool LoadTasks()
        {
            Exception ex;
            if (!taskList.LoadData(out ex))
            {
                if (ex != null)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogManager.LogManager.LogException(ex);
                }
                return false;
            }

            return true;
        }
        private void SaveTasks()
        {
            Exception ex;
            if (!taskList.SaveData(out ex))
            {
                if (ex != null)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogManager.LogManager.LogException(ex);
                }
            }
            else
            {
                MessageBox.Show("Tasks saved successfuly");
            }
        }
        #endregion

        private void PrioritizeTask(ManagableTask task)
        {
            if (task == null)
                return;

            Exception ex;
            if (!taskList.PrioritizeTask(task, out ex))
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogManager.LogManager.LogException(ex);
            }
            else
            {
                DisplayTasks();
            }
        }
        private void DeprioritizeTask(ManagableTask task)
        {
            if (task == null)
                return;

            Exception ex;
            if (!taskList.DeprioritizeTask(task, out ex))
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogManager.LogManager.LogException(ex);
            }
            else
            {
                DisplayTasks();
            }
        }

        #region Graphic Functions
        private void DisplayTasks()
        {
            int selectedIndex = lvTaskList.SelectedIndex;

            lvTaskList.Items.Clear();

            foreach (ManagableTask task in taskList.Tasks.Where(task => TaskFitsFilter(task)).OrderBy(task => task.Priority))
            {
                DisplayTask(task);
            }

            if (selectedIndex >= 0)
                lvTaskList.SelectedIndex = selectedIndex;
        }
        private void DisplayTask(ManagableTask task)
        {
            SingleTaskDisplay disp = new SingleTaskDisplay(task, taskList.Tasks.Count, _taskFilter == TaskFilter.All);
            //changing the tasks' priority when not all of them are displayed can be confusing, so I disabled it unless all of them are displayed

            disp.btnPrioritizeClick += btnPrioritize_Click;
            disp.btnDeprioritizeClick += btnDeprioritize_Click;
            disp.cbDoneCheckChanged += cbDone_CheckChanged;

            disp.DataContext = task;

            //spTaskList.Children.Add(disp);
            lvTaskList.Items.Add(disp);
        }

        private ManagableTask GetSelectedTask()
        {
            FrameworkElement selectedElement = GetSelectedItem();
            if (selectedElement == null)
                return null;

            ManagableTask task = selectedElement.DataContext as ManagableTask; ;
            return task;
        }
        private FrameworkElement GetSelectedItem()
        {
            if (lvTaskList.Items != null && lvTaskList.Items.Count == 1)
                return lvTaskList.Items.GetItemAt(0) as FrameworkElement;

            if (lvTaskList.SelectedItem == null)
                return null;

            return lvTaskList.SelectedItem as FrameworkElement;

        }
        #endregion

        #region Utility Functions
        private bool TaskFitsFilter(ManagableTask task)
        {
            if (_taskFilter == TaskFilter.All)
                return true;
            else if (_taskFilter == TaskFilter.Done)
                return task.Done;
            else
                return !task.Done;
        }
        #endregion
    }
}