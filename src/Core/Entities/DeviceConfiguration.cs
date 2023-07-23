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
        
        
        public DeviceConfiguration LoadFromFile(FileInfo configFile)
        {
            var streamReader = new StreamReader(configFile.FullName);
            return LoadFromStream(streamReader.BaseStream);
        }
        
        public DeviceConfiguration LoadFromStream(Stream configStream)
        {
            using var gzip = new GZipStream(configStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(gzip);
            try
            {
                var configString = streamReader.ReadToEnd();
                return LoadFromString(configString);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Invalid configuration file format.", e);
            }
        }

        public DeviceConfiguration LoadFromString(string configString)
        {
            var idValuePairs = configString.Split(';');

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