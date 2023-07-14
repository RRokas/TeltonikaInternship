using System;
using System.Globalization;

namespace Core.Entities
{
    public class ConfigurationParameter
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public ConfigurationParameter(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}