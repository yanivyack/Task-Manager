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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Task_Manager
{
    /// <summary>
    /// Interaction logic for SingleTaskDisplay.xaml
    /// </summary>
    public partial class SingleTaskDisplay : UserControl
    {
        #region Fields
        private ManagableTask _task;
        private int _tasksCount;//the overall amount of tasks-to determine when to allow the user to raise/lower the task's priority
        private bool _displayPriority;//whether to display the priority buttons or not
        #endregion

        #region Events
        public event RoutedEventHandler btnPrioritizeClick;
        public event RoutedEventHandler btnDeprioritizeClick;
        public event RoutedEventHandler cbDoneCheckChanged;
        #endregion

        #region Constructors
        public SingleTaskDisplay()
        {
            InitializeComponent();
        }
        public SingleTaskDisplay(ManagableTask task, int tasksCount, bool displayPriority = false) : this()
        {
            _task = task;
            _tasksCount = tasksCount;
            _displayPriority = displayPriority;
        }
        #endregion

        #region Event Handlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_task == null)
                return;

            DisplayTask();
        }

        private void btnPrioritize_Click(object sender, RoutedEventArgs e)
        {
            if (btnPrioritizeClick != null)
            {
                btnPrioritizeClick.Invoke(this, null);
            }
        }
        private void btnDeprioritize_Click(object sender, RoutedEventArgs e)
        {
            if (btnDeprioritizeClick != null)
            {
                btnDeprioritizeClick.Invoke(this, null);
            }
        }

        private void cbDone_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)//this event may be triggered when loading the tasks
                return;

            if (cbDoneCheckChanged != null)
            {
                cbDoneCheckChanged.Invoke(this, null);
            }
        }
        #endregion

        #region Graphic Functions
        private void DisplayTask()
        {
            if (!_displayPriority)
            {
                HideButton(btnPrioritize);
                HideButton(btnDeprioritize);
            }
            if (_tasksCount >= 0)
            {
                if (_task.Priority == 0)
                    HideButton(btnPrioritize);
                if (_task.Priority == _tasksCount - 1)
                    HideButton(btnDeprioritize);
            }

            tblTitle.Focus();
        }
        private void HideButton(Button btn)
        {
            btn.Visibility = Visibility.Collapsed;
            btn.IsEnabled = false;
        }
        #endregion
    }
}