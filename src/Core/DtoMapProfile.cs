using System;
using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core
{
    public class DtoMapProfile : Profile
    {
        public DtoMapProfile()
        {
            CreateMap<ConfigurationParameterComparison, ConfigurationParameterComparisonDto>();
            CreateMap<ConfigurationMetadata, ConfigurationMetadataDto>();
            CreateMap<DeviceConfiguration, DeviceConfigurationDto>();
            CreateMap<DeviceConfigurationComparison, DeviceConfigurationComparisonDto>();
            CreateMap<ConfigurationParameterComparison, ConfigurationParameterComparisonDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(MapIdValueFromSourceOrTarget))
                .ForMember(dto => dto.SourceValue, opt => opt.MapFrom(src => src.Source!.Value))
                .ForMember(dto => dto.TargetValue, opt => opt.MapFrom(src => src.Target!.Value))
                .ForMember(dto => dto.ComparisonResult, opt => opt.MapFrom(src => src.Result));

        }

        private string MapIdValueFromSourceOrTarget(ConfigurationParameterComparison entity,
            ConfigurationParameterComparisonDto dto)
        {
            if (entity.Target != null) return entity.Source != null ? entity.Source.Id : entity.Target.Id;
            throw new ArgumentException("Both source and target are null");
        }
    }
}