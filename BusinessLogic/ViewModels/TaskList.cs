using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLogic.Models;

namespace BusinessLogic.ViewModels
{
    public class TaskList
    {
        /// <summary>
        /// the main class, containing most of the business logic
        /// any interaction with the business logic goes through this class
        /// </summary>

        private List<ManagableTask> _tasks = new List<ManagableTask>();
        public List<ManagableTask> Tasks { get { return _tasks; } set { _tasks = value; } }

        private bool _changesMade = false;
        public bool ChangesMade
        {
            get { return _changesMade || (_tasks != null && _tasks.Any(task => task.HasChanges)); }
            private set
            {
                if (!value)
                {
                    if (_tasks != null)
                        _tasks.ForEach(task => task.HasChanges = false);
                }
                _changesMade = value;
            }
        }

        private IDB _dbManager;

        public TaskList(IDB dbManager)
        {
            _dbManager = dbManager;
        }

        public bool AddTask(string title, string description, int priority, out Exception ex)
        {
            ManagableTask task = new ManagableTask();
            task.ID = NextTaskId();
            task.Title = title;
            task.Description = description;

            //check&fix any possible issues with the priority
            if (priority < 0 || priority >= _tasks.Count)//priority -1 is default(which means we assign last priority to this task), a above _tasks.Count is illegal
                priority = _tasks.Count;

            task.Priority = priority;

            //the actual addition of the task
            ex = null;
            try
            {
                if (_tasks.Any(existingTask => existingTask.ID == task.ID))
                    return false;

                //if the task is inserted in a priority that isn't last, tasks in lower priority need to be deprioritized to 'make room'
                if (priority <= _tasks.Count)
                    deprioritizeTasks(_tasks.Where(tsk => tsk.Priority >= task.Priority));
                _tasks.Add(task);

                ChangesMade = true;

                return true;
            }
            catch (Exception ex2)
            {
                ex = ex2;
                return false;
            }
        }
        public bool DeleteTask(ManagableTask task, out Exception ex)
        {
            ex = null;
            try
            {
                int indexToDelete = GetTaskIndexById(task.ID);
                if (indexToDelete >= 0 && indexToDelete < _tasks.Count)
                {
                    _tasks.RemoveAt(indexToDelete);

                    //if the removed task wasn't last priority, tasks in lower priority need to be prioritized to 'close the gap'
                    if (task.Priority < _tasks.Count - 1)
                        prioritizeTasks(_tasks.Where(tsk => tsk.Priority >= task.Priority));

                    ChangesMade = true;
                    return true;
                }

                return true;
            }
            catch (Exception ex2)
            {
                ex = ex2;
                return false;
            }
        }

        public bool PrioritizeTask(ManagableTask task, out Exception ex)
        {
            //move a task to a higher priority
            ex = null;

            try
            {
                if (task == null || task.Priority <= 0)
                    return false;

                int higherPriority = task.Priority - 1;
                ManagableTask other = _tasks.FirstOrDefault(tsk => tsk.Priority == higherPriority);

                //two tasks aren't allowed to have the same priority, so if the desired priority is already taken we need to swap them
                if (other != null)
                    other.Priority = task.Priority;
                task.Priority = higherPriority;

                return true;
            }
            catch (Exception ex2)
            {
                ex = ex2;
                return false;
            }
        }
        public bool DeprioritizeTask(ManagableTask task, out Exception ex)
        {
            ex = null;

            try
            {
                if (task == null || task.Priority >= _tasks.Count)
                    return false;

                int lowerPriority = task.Priority + 1;
                ManagableTask other = _tasks.FirstOrDefault(tsk => tsk.Priority == lowerPriority);

                //two tasks aren't allowed to have the same priority, so if the desired priority is already taken we need to swap them
                if (other != null)
                    other.Priority = task.Priority;
                task.Priority = lowerPriority;

                return true;
            }
            catch (Exception ex2)
            {
                ex = ex2;
                return false;
            }
        }

        public bool LoadData(out Exception ex)
        {
            ex = null;
            try
            {
                if (_dbManager == null)
                    return false;

                _tasks = _dbManager.LoadData();
                ChangesMade = false;
                return true;
            }
            catch (Exception ex2)
            {
                ex = ex2;
                return false;
            }
        }
        public bool SaveData(out Exception ex)
        {
            ex = null;
            try
            {
                if (_dbManager == null)
                    return false;

                _dbManager.SaveData(_tasks);
                ChangesMade = false;
                return true;
            }
            catch (Exception ex2)
            {
                ex = ex2;
                return false;
            }
        }

        public bool ExportToPdf(string path, out Exception ex)
        {
            ex = null;

            try
            {
                PDFHandler.ExportToPDF(path, _tasks);
                return true;
            }
            catch (Exception ex2)
            {
                ex = ex2;
                return false;
            }
        }

        private int NextTaskId()
        {
            if (_tasks == null || _tasks.Count == 0)
                return 0;
            return _tasks.Max(task => task.ID) + 1;
        }
        private int GetTaskIndexById(int taskID)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].ID == taskID)
                    return i;
            }

            return -1;
        }

        private void prioritizeTasks(IEnumerable<ManagableTask> tasks)
        {
            foreach (ManagableTask task in tasks)
            {
                prioritizeSingleTask(task);
            }
        }
        private void deprioritizeTasks(IEnumerable<ManagableTask> tasks)
        {
            foreach (ManagableTask task in tasks)
            {
                deprioritizeSingleTask(task);
            }
        }
        private void prioritizeSingleTask(ManagableTask task)
        {
            task.Priority--;
            ChangesMade = true;
        }
        private void deprioritizeSingleTask(ManagableTask task)
        {
            task.Priority++;
            ChangesMade = true;
        }
    }
}