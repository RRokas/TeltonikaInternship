using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Core.Entities
{
    public class DeviceConfiguration
    {
        public string Filename { get; set; }
        public List<ConfigurationParameter> Parameters { get; set; }

        public DeviceConfiguration(string configPath)
        {
            Filename = Path.GetFileName(configPath);
            var configString = ReadConfigString(configPath);
            Parameters = ParseConfigString(configString);
            var doubleParams = Parameters.Where(x => x.Value.GetType().ToString() == "System.Double");
            Console.WriteLine(Parameters[0].Value.GetType().ToString());
        }

        private string ReadConfigString(string filePath)
        {
            using var compressedFileStream = File.Open(filePath, FileMode.Open);
            using var gzip = new GZipStream(compressedFileStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(gzip);
            return streamReader.ReadToEnd();
        }
        
        private List<ConfigurationParameter> ParseConfigString(string configString)
        {
            var parameters = new List<ConfigurationParameter>();
            var idValuePairs = configString.Split(';');
            foreach (var idValuePair in idValuePairs)
            {
                if (string.IsNullOrEmpty(idValuePair)) continue;

                var splitPair = idValuePair.Split(':');
                parameters.Add(new ConfigurationParameter(splitPair[0], splitPair[1]));
            }

            return parameters;
        }
    }
}