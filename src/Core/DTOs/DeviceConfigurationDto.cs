using System.Collections.Generic;

namespace Core.DTOs
{
    public class DeviceConfigurationDto
    {
        public string? Filename { get; init; }
        public List<ConfigurationMetadataDto>? Metadata { get; init; }
    }
}