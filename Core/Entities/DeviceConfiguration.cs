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

        public DeviceConfiguration(FileInfo configFile)
        {
            Filename = configFile.Name;
            var configString = ReadConfigString(configFile.FullName);
            Parameters = ParseConfigString(configString);
        }

        public DeviceConfiguration(string configString)
        {
            Parameters = ParseConfigString(configString);
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