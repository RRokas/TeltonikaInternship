using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Core.Entities
{
    public class DeviceConfiguration
    {
        public List<ConfigurationMetadata> Metadata { get; set; }
        public List<ConfigurationParameter> Parameters { get; set; }

        public DeviceConfiguration(FileInfo configFile)
        {
            var configString = ReadConfigString(configFile.FullName);
            var parsedConfig = ParseConfigString(configString);
            Parameters = parsedConfig.ConfigurationParameters;
            Metadata = parsedConfig.ConfigurationMetadata;
        }

        public DeviceConfiguration(string configString)
        {
            var parsedConfig = ParseConfigString(configString);
            Parameters = parsedConfig.ConfigurationParameters;
            Metadata = parsedConfig.ConfigurationMetadata;
        }

        private string ReadConfigString(string filePath)
        {
            using var compressedFileStream = File.Open(filePath, FileMode.Open);
            using var gzip = new GZipStream(compressedFileStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(gzip);
            return streamReader.ReadToEnd();
        }

        private (List<ConfigurationParameter> ConfigurationParameters, List<ConfigurationMetadata> ConfigurationMetadata
            ) ParseConfigString(string configString)
        {
            var parameters = new List<ConfigurationParameter>();
            var metadata = new List<ConfigurationMetadata>();
            var idValuePairs = configString.Split(';');
            foreach (var idValuePair in idValuePairs)
            {
                if (string.IsNullOrEmpty(idValuePair)) continue;
                
                var splitPair = idValuePair.Split(':');
                if (!double.TryParse(splitPair[0], out _))
                    metadata.Add(new ConfigurationMetadata(splitPair[0], splitPair[1]));
                else
                    parameters.Add(new ConfigurationParameter(splitPair[0], splitPair[1]));
            }

            return (parameters, metadata);
        }
    }
}