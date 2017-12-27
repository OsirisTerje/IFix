using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace IFix
{
    public class Runsettings
    {
        private List<string> runsettingsFileContent;

        public Runsettings()
        {
            RetrieveRunSettings();
        }

        public IEnumerable<string> FileContent => runsettingsFileContent;

        private void RetrieveRunSettings()
        {
            var temp = Path.GetTempPath();
            var tempRunsettings = temp + "/default.runsettings";
            if (!File.Exists(tempRunsettings) || (DateTime.Now.Subtract(File.GetLastWriteTime(tempRunsettings)).Hours > 24))
                DownloadRunsettings(tempRunsettings);
            runsettingsFileContent = new List<string>(File.ReadAllLines(tempRunsettings));

        }

        public void DownloadRunsettings(string path)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/OsirisTerje/RunSettings/master/AllTemplate/AllRunSettings.runsettings",
                    path);
            }
        }

    }
}
