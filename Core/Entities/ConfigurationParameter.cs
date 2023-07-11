using System;
using System.Globalization;

namespace Core.Entities
{
    public class ConfigurationParameter
    {
        public object Id { get; set; }
        public object Value { get; set; }

        public ConfigurationParameter(string id, string value)
        {
            Id = ParseType(id);
            Value = ParseType(value);
        }
        
        private static object ParseType(string value)
        {
            if(int.TryParse(value, out var valueInt))
                return valueInt;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var valueDouble))
                return valueDouble;
            return value;
        }
    }
}