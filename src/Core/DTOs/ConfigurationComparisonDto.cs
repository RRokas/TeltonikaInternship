using System.Collections.Generic;
using Core.Entities;

namespace Core.DTOs
{
    public class ConfigurationComparisonDto
    {
        public List<ConfigurationParameterComparison> Results { get; }
        public DeviceConfigurationDto Source { get; init; }
        public DeviceConfigurationDto Target { get; init; }
    }
}