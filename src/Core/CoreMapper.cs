using AutoMapper;
using Core.DTOs;

namespace Core
{
    public class CoreMapper
    {
        public IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Entities.ConfigurationParameterComparison, ConfigurationParameterComparisonDto>();
                cfg.CreateMap<Entities.ConfigurationMetadata, ConfigurationMetadataDto>();
                cfg.CreateMap<Entities.DeviceConfiguration, DeviceConfigurationDto>();
                cfg.CreateMap<Entities.DeviceConfigurationComparison, DeviceConfigurationComparisonDto>();
                cfg.AddProfile<DtoMapProfile>();
            });

            return config.CreateMapper();
        }
    }
}