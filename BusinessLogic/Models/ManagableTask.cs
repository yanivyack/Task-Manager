using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace BusinessLogic.Models
{
    public class ManagableTask
    {
        //since this class is entirely comprised of fields, it seems self-explanatory to me so I don't have any comments to add
        private int _id;
        private string _title;
        private string _description;
        private int _priority;
        private bool _done;

        public bool HasChanges = false;

        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if (!value.Equals(_title))
                    HasChanges = true;
                _title = value;
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (!value.Equals(_description))
                    HasChanges = true;
                _description = value;
            }
        }
        public int Priority
        {
            get { return _priority; }
            set
            {
                if (_priority != value)
                    HasChanges = true;
                _priority = value;
            }
        }
        public bool Done
        {
            get { return _done; }
            set
            {
                if (_done != value)
                    HasChanges = true;
                _done = value;
            }
        }
    }
}