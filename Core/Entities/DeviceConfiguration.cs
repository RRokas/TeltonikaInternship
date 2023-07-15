using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Core.Entities
{
    public class DeviceConfiguration
    {
        public List<ConfigurationMetadata> Metadata { get; private set; } = new();
        public List<ConfigurationParameter> Parameters { get; private set; } = new();

        private string ReadConfigString(string filePath)
        {
            using var compressedFileStream = File.Open(filePath, FileMode.Open);
            using var gzip = new GZipStream(compressedFileStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(gzip);
            return streamReader.ReadToEnd();
        }
        
        public DeviceConfiguration LoadFromFile(FileInfo configFile)
        {
            var configString = ReadConfigString(configFile.FullName);
            return LoadFromString(configString);
        }

        public DeviceConfiguration LoadFromString(string configString)
        {
            var idValuePairs = configString.Split(';');

            if (idValuePairs.Length == 0)
                return new DeviceConfiguration();

            var parameters = new List<ConfigurationParameter>();
            var metadata = new List<ConfigurationMetadata>();  
            foreach (var idValuePair in idValuePairs)
            {
                if (string.IsNullOrEmpty(idValuePair)) continue;
                
                var splitPair = idValuePair.Split(':');
                if (!double.TryParse(splitPair[0], out _))
                    metadata.Add(new ConfigurationMetadata(splitPair[0], splitPair[1]));
                else
                    parameters.Add(new ConfigurationParameter(splitPair[0], splitPair[1]));
            }

            return new DeviceConfiguration
            {
                Metadata = metadata,
                Parameters = parameters
            };
        }
    }
}