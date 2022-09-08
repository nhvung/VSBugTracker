using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker.ADO
{
    public class TAttachmentADO
    {
        public int AttachmentID { get; set; }
        public int BugID { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileData { get; set; }
    }
}
