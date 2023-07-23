using System.Collections.Generic;

namespace Core.DTOs
{
    public class DeviceConfigurationDto
    {
        public string? Filename { get; set; }
        public List<ConfigurationMetadataDto>? Metadata { get; set; }
    }
}