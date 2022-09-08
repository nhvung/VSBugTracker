using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSBugTracker.ADO;

namespace VSBugTracker.Models
{
    public class TBugFullInfo : TBugADO
    {
        public List<TAttachmentADO> Attachments { get; set; }
        public TBugFullInfo() : base() { Attachments = new List<TAttachmentADO>(); }
        public TBugFullInfo(TBugADO dbObj)
        {
            AddedAccountID = dbObj.AddedAccountID;
            AssignedAccountID = dbObj.AssignedAccountID;
            BugID = dbObj.BugID;
            CreatedDate = dbObj.CreatedDate;
            Description = dbObj.Description;
            OpenedAccountID = dbObj.OpenedAccountID;
            OpenedDate = dbObj.OpenedDate;
            PriorityID = dbObj.PriorityID;
            ResolvedDate = dbObj.ResolvedDate;
            SprintID = dbObj.SprintID;
            StatusID = dbObj.StatusID;
            TaskID = dbObj.TaskID;
            VerifiedAccountID = dbObj.VerifiedAccountID;
            VerifiedDate = dbObj.VerifiedDate;
            Comment = dbObj.Comment;
            Version = dbObj.Version;
            ResolveVersion = dbObj.ResolveVersion;
        }
    }
}
