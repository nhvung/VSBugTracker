using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker.ADO
{
    public class TNotifyADO
    {
        public int NotifyID { get; set; }
        public int AccountID { get; set; }
        public string Email { get; set; }
        public string SkypeID { get; set; }
        public int ScheduleTime { get; set; }
        public byte Status { get; set; }
        public int BugStatus { get; set; }
    }
}
