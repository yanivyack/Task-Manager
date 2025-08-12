using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public interface IDB
    {
        /// <summary>
        /// an interface to define what functions TaskList needs to have to interact with the DB
        /// Any class that handles the DB needs to implement this interface
        /// </summary>
        public void SaveData(List<ManagableTask> taskList);
        public List<ManagableTask> LoadData();
    }
}