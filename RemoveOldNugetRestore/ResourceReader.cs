using System.Globalization;
using System.IO;
using System.Reflection;

namespace IFix
{
    /// <summary>
    /// Low level Utility Class for reading out resources from this assembly, including a couple of utility methods that are independent too.
    /// </summary>
    public static class ResourceReader
    {
        /// <summary>Last error message, if any. Empty string if none.</summary>
        public static string LastError { get; set; }

        public static string Read(string resource)
        {
            return Read(typeof(ResourceReader).Assembly, resource);
        }

        /// <summary>
        /// Utility method to read resources out of any assembly given by the assembly parameter.  The deviceXml is the last part of the resource name, and must include the extension if a file.
        /// </summary>
        public static string Read(Assembly assembly, string deviceXml)
        {
            LastError = "";
            foreach (var manifestResourceName in assembly.GetManifestResourceNames())
            {
                if (
                    manifestResourceName.ToUpper(CultureInfo.InvariantCulture)
                        .EndsWith(deviceXml.ToUpper(CultureInfo.InvariantCulture)))
                {
                    using (var manifestResourceStream = assembly.GetManifestResourceStream(manifestResourceName))
                    {
                        using (var streamReader = new StreamReader(manifestResourceStream))
                            return streamReader.ReadToEnd();
                    }
                }
            }
            LastError = "Could not find resource for " + deviceXml + " in assembly " +
                                       assembly.GetName().FullName;
            return null;
        }
    }
}
