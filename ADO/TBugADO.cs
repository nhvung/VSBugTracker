using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker.ADO
{
    public class TBugADO
    {
        public int BugID { get; set; }
        public string Description { get; set; }
        public int TaskID { get; set; }
        public int SprintID { get; set; }
        public int AddedAccountID { get; set; }
        public int OpenedAccountID { get; set; }
        public int VerifiedAccountID { get; set; }
        public int AssignedAccountID { get; set; }
        public int PriorityID { get; set; }
        public int StatusID { get; set; }
        public int CreatedDate { get; set; }
        public int ResolvedDate { get; set; }
        public int OpenedDate { get; set; }
        public int VerifiedDate { get; set; }
        public string Comment { get; set; }
        public string Version { get; set; }
        public string ResolveVersion { get; set; }
    }
}
