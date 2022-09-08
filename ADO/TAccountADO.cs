using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker.ADO
{
    public class TAccountADO
    {
        public int AccountID { get; set; }
        public string AccountLogin { get; set; }
        public string AccountName { get; set; }
        public string EncryptedPassword { get; set; }
        public int AccountType { get; set; }
        public int AccountRight { get; set; }
    }
}
