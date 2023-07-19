using System.Collections.Generic;
using System.Linq;

namespace Core.DTOs
{
    public class DeviceConfigurationComparisonDto
    {
        public List<ConfigurationParameterComparisonDto> Results { get; }
        public DeviceConfigurationDto? Source { get; init; }
        public DeviceConfigurationDto? Target { get; init; }

        // Private parameterless constructor for AutoMapper
        private DeviceConfigurationComparisonDto()
        {
            Results = Enumerable.Empty<ConfigurationParameterComparisonDto>().ToList();
        }


    }
}