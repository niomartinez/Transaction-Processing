using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionsWebApp.Helpers.Utilities
{
    public class AppSettingsModel
    {
        public string LogFolder { get; set; }

        public string LogFile { get; set; }
        public string FilePath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), LogFolder); }
        }

        public string File
        {
            get { return Path.Combine(FilePath, DateTime.Now.ToShortDateString() + LogFile); }
        }
    }
}
