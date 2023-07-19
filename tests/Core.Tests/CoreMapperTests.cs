using System.Collections.Generic;
using Core.DTOs;
using System.Linq;
using Core.Entities;
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
            var mapper = new CoreMapper().CreateMapper();
            var dto = mapper.Map<DeviceConfigurationComparisonDto>(comparison);
            
            Assert.Equal(1, dto.Results.Count);
            Assert.Equal("0", dto.Results[0].Id);
            Assert.Equal("1", dto.Results[0].TargetValue);
            Assert.Equal(ComparisonResult.Added.ToString(), dto.Results[0].Result);
        }

        [Fact]
        public void MapDeviceConfigurationComparisonToDto_withRemovedParameter()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var mapper = new CoreMapper().CreateMapper();
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
            var mapper = new CoreMapper().CreateMapper();
            var dto = mapper.Map<DeviceConfigurationComparisonDto>(comparison);
            
            Assert.Single(dto.Results);
            Assert.Equal("0", dto.Results[0].Id);
            Assert.Equal("1", dto.Results[0].SourceValue);
            Assert.Equal("2", dto.Results[0].TargetValue);
            Assert.Equal(ComparisonResult.Modified.ToString(), dto.Results[0].Result);
        }
        
        [Fact]
        public void MapDeviceConfigurationToDto()
        {
            var configString = "Name:SomeName;Version:1;42:1.2";
            var config = new DeviceConfiguration().LoadFromString(configString);
            
            var mapper = new CoreMapper().CreateMapper();
            var dto = mapper.Map<DeviceConfigurationDto>(config);
            
            Assert.Equal(2, dto.Metadata.Count);
            Assert.Equal("Name", dto.Metadata[0].Id);
            Assert.Equal("SomeName", dto.Metadata[0].Value);
            Assert.Equal("Version", dto.Metadata[1].Id);
            Assert.Equal("1", dto.Metadata[1].Value);
            Assert.DoesNotContain("42", dto.Metadata.Select(x=>x.Id));
        }

        [Fact]
        public void MapDeviceConfigurationComparisonToDto_SourceMetadataAccessible()
        {
            var sourceConfigString = "Name:SomeName;Version:1;5:1.2";
            var targetConfigString = "Name:AnotherName;Version:2;6:1.2;0:1";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var mapper = new CoreMapper().CreateMapper();
            var dto = mapper.Map<DeviceConfigurationComparisonDto>(comparison);
            
            Assert.Equal(2, dto.Source.Metadata.Count);
            Assert.Equal("Name", dto.Source.Metadata[0].Id);
            Assert.Equal("SomeName", dto.Source.Metadata[0].Value);
            Assert.Equal("Version", dto.Source.Metadata[1].Id);
            Assert.Equal("1", dto.Source.Metadata[1].Value);
            Assert.Equal(2, dto.Target.Metadata.Count);
            Assert.Equal("Name", dto.Target.Metadata[0].Id);
            Assert.Equal("AnotherName", dto.Target.Metadata[0].Value);
            Assert.Equal("Version", dto.Target.Metadata[1].Id);
            Assert.Equal("2", dto.Target.Metadata[1].Value);
        }
    }
}