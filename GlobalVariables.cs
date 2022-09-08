using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSBugTracker
{
    public class GlobalVariables
    {
        static string _connectionString;
        public static string ConnectionString { get { return _connectionString; } }
        public static void Init(string server, string username, string password, string database, string driver)
        {
            _connectionString = string.Format("server={0}; uid={1}; password={2}; database={3}; driver={4};", server, username, password, database, driver);
        }
    }
}
