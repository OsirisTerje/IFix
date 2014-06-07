using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitIgnoreTests
{
    public static class ResourceReader
    {
        public static List<string> Read(string resourceName)
        {
            var resource =
                Assembly.GetExecutingAssembly().GetManifestResourceNames()
                    .FirstOrDefault(item => item.ToUpper().EndsWith(resourceName.ToUpper()));
            if (resource==null)
                throw new ArgumentException("No test file resource named : " + resourceName);
            string content = "";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                using (var reader = new StreamReader(stream))
                {
                    content= reader.ReadToEnd();
                }
            }
            var lines = content.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
            return new List<string>(lines);


        }
    }
}
