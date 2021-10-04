using System;
using System.IO;

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
