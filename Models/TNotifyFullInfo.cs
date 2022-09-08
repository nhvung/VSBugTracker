using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker.Models
{

    public class TNotifyFullInfo
    {
        public enum RepeatDayOfWeek : int
        {
            None = 0,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 4,
            Thursday = 8,
            Friday = 16,
            Saturday = 32,
            Sunday = 64
        }
        public int AccountID { get; set; }
        public string Email { get; set; }
        public string SkypeID { get; set; }
        public RepeatDayOfWeek RepeatDays { get; set; }
        public int RepeatTime { get; set; }
        public int BugStatus { get; set; }
        public byte Status { get; set; }
    }
}
