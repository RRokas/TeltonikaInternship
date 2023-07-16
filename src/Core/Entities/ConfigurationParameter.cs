using System;
using System.Globalization;

namespace Core.Entities
{
    public class ConfigurationParameter
    {
        public string Id { get; }
        public string Value { get; }

        public ConfigurationParameter(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}