using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker.ADO
{
    public class TPriorityADO
    {
        public int PriorityID { get; set; }
        public string PriorityName { get; set; }
        public override string ToString()
        {
            return string.Format("{0}: {1}", PriorityID, PriorityName);
        }
    }
}
