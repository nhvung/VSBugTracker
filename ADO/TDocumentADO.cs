using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker.ADO
{
    public class TDocumentADO
    {
        public int DocumentID { get; set; }
        public int SprintID { get; set; }
        public int TaskID { get; set; }
        public int AddedAccountID { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileData { get; set; }
    }
}
