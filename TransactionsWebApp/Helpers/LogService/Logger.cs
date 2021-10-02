﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TransactionsWebApp.Helpers.Utilities;

namespace TransactionsWebApp.Helpers.LogService
{
    public class Logger
    {

        private readonly IOptions<AppSettingsModel> _settings;
        public Logger(IOptions<AppSettingsModel> settings)
        {
            _settings = settings;
        }
        public void Log(string message)
        {
            SetLogPathAndFile();
            var file = Path.Combine(_settings.Value.FilePath, _settings.Value.LogFile);
            WriteLog(file, message);
        }

        private void SetLogPathAndFile()
        {
            var filePath = Path.Combine(_settings.Value.FilePath);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var file = Path.Combine(filePath, _settings.Value.LogFile);
            if (!File.Exists(file))
            {
                FileStream fs = File.Create(file);
                fs.Close();

                using StreamWriter sw = new(file, true);
                sw.WriteLine("Transaction Service Logs: ");
            }
        }
        private static void WriteLog(string file, string message)
        {
            var log = string.Format("{0} {1}", DateTime.Now.ToShortTimeString(), message);

            using StreamWriter sw = new(file, true);
            sw.WriteLine(log);
        }
    }
}