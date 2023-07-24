using System.Diagnostics;
using Core.DTOs;
using System.Linq;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Tests.Utilities;
using Xunit;

namespace Core.Tests
{
    public class CoreMapperTests
    {
        [Fact]
        public void MapDeviceConfigurationComparisonToDto_withAddedParameter()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DtoMapProfile>()).CreateMapper();
            var dto = mapper.Map<DeviceConfigurationComparisonDto>(comparison);
            
            Assert.Single(dto.Results);
            Assert.Equal("0", dto.Results[0].Id);
            Assert.Equal("1", dto.Results[0].TargetValue);
            Assert.Equal(ComparisonResult.Added, dto.Results[0].ComparisonResult);
        }

        [Fact]
        public void MapDeviceConfigurationComparisonToDto_withRemovedParameter()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DtoMapProfile>()).CreateMapper();
            var dto = mapper.Map<DeviceConfigurationComparisonDto>(comparison);
            
            Assert.Single(dto.Results);
            Assert.Equal("0", dto.Results[0].Id);
            Assert.Equal("1", dto.Results[0].SourceValue);
            Assert.Null(dto.Results[0].TargetValue);
        }
        
        [Fact]
        public void MapDeviceConfigurationComparisonToDto_withModifiedParameter()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:2";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DtoMapProfile>()).CreateMapper();
            var dto = mapper.Map<DeviceConfigurationComparisonDto>(comparison);
            
            Assert.Single(dto.Results);
            Assert.Equal("0", dto.Results[0].Id);
            Assert.Equal("1", dto.Results[0].SourceValue);
            Assert.Equal("2", dto.Results[0].TargetValue);
            Assert.Equal(ComparisonResult.Modified, dto.Results[0].ComparisonResult);
        }
        
        [Fact]
        public void MapDeviceConfigurationToDto()
        {
            var configString = "Name:SomeName;Version:1;42:1.2";
            var config = new DeviceConfiguration().LoadFromString(configString);
            
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DtoMapProfile>()).CreateMapper();
            var dto = mapper.Map<DeviceConfigurationDto>(config);
            
            Assert.Equal(2, dto.Metadata!.Count);
            Assert.Equal("Name", dto.Metadata[0].Id);
            Assert.Equal("SomeName", dto.Metadata[0].Value);
            Assert.Equal("Version", dto.Metadata[1].Id);
            Assert.Equal("1", dto.Metadata[1].Value);
            Assert.DoesNotContain("42", dto.Metadata.Select(x=>x.Id));
        }

        [Fact]
        public void MapDeviceConfigurationComparisonToDto_sourceMetadataAccessible()
        {
            var sourceConfigString = "Name:SomeName;Version:1;5:1.2";
            var targetConfigString = "Name:AnotherName;Version:2;6:1.2;0:1";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DtoMapProfile>()).CreateMapper();
            var dto = mapper.Map<DeviceConfigurationComparisonDto>(comparison);
            
            Assert.Equal(2, dto.Source!.Metadata!.Count);
            Assert.Equal("Name", dto.Source.Metadata[0].Id);
            Assert.Equal("SomeName", dto.Source.Metadata[0].Value);
            Assert.Equal("Version", dto.Source.Metadata[1].Id);
            Assert.Equal("1", dto.Source.Metadata[1].Value);
            Assert.Equal(2, dto.Target!.Metadata!.Count);
            Assert.Equal("Name", dto.Target.Metadata[0].Id);
            Assert.Equal("AnotherName", dto.Target.Metadata[0].Value);
            Assert.Equal("Version", dto.Target.Metadata[1].Id);
            Assert.Equal("2", dto.Target.Metadata[1].Value);
        }

        [Fact]
        public void DeviceConfigurationDto_fileNameGetsMapped()
        {
            var configFile = TestDataDirectory.GetFile("FMB001-default.cfg");
            var config = new DeviceConfiguration().LoadFromFile(configFile);
            
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DtoMapProfile>()).CreateMapper();
            var dto = mapper.Map<DeviceConfigurationDto>(config);
            
            Assert.Equal("FMB001-default.cfg", dto.Filename);
        }
    }
}