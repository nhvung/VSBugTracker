using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSBugTracker.ADO;

namespace VSBugTracker.Models
{
    public class TSprintInfo : TSprintADO
    {
        public List<TTaskADO> Tasks { get; set; }
        public TSprintInfo() : base() { Tasks = new List<TTaskADO>(); }
        public TSprintInfo(TSprintADO dbObj)
        {
            Tasks = new List<TTaskADO>();
            SprintID = dbObj.SprintID;
            SprintName = dbObj.SprintName;
            StartDate = dbObj.StartDate;
            EndDate = dbObj.EndDate;
        }
    }
}
