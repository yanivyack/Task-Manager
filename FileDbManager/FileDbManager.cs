using BusinessLogic.Models;
using BusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FileDbManager
{
    public class FileDbManager : IDB
    {
        /// <summary>
        /// an implementation of the tasks DataBase, implemented as a *.json file located in the same directory as the program's *.exe file
        /// </summary>
        private static string DbFilePath { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DbFile.json"); } }

        public List<ManagableTask> LoadData()
        {
            try
            {
                string jsonString = null;

                string filePath = DbFilePath;
                if (File.Exists(filePath))
                    jsonString = File.ReadAllText(filePath);
                else
                {
                    File.Create(filePath).Close();//if the file doesn't exist-create it
                }

                if (!string.IsNullOrEmpty(jsonString))
                {
                    return JsonSerializer.Deserialize<List<ManagableTask>>(jsonString);
                }

                return new List<ManagableTask>();
            }
            catch (Exception ex)
            {
                LogManager.LogManager.LogException(ex);
                throw ex;
            }
        }

        public void SaveData(List<ManagableTask> taskList)
        {
            try
            {
                string filePath = DbFilePath;
                File.WriteAllText(filePath, JsonSerializer.Serialize(taskList));
            }
            catch (Exception ex)
            {
                LogManager.LogManager.LogException(ex);
                throw ex;
            }
        }
    }
}