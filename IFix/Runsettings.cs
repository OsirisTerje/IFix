using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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
            using (var client = new HttpClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/OsirisTerje/RunSettings/master/AllTemplate/AllRunSettings.runsettings",
                    path);
            }
        }

    }

    public static class HttpClientExtensions
    {
        public static async Task DownloadFile(this HttpClient client, string requestUri, string filename)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            await using Stream contentStream = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .Result.Content.ReadAsStreamAsync().Result,
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None,
                    8192, true);
            await contentStream.CopyToAsync(stream);
        }
    }
}
