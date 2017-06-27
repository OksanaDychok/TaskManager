using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task_manager.Models
{
    public class TaskManager
    {
        public int ID { get; set; }
        [Display(Name = "Task")]
        public string TaskContext { get; set; }
        [Display(Name = "Who share this task")]
        public string WhoSharedTask { get; set; }
        public virtual UserProfile UserProfiles { get; set; }
    }
}