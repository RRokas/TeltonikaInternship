using System.IO;
using System.Linq;
using Core.Entities;
using Xunit;

namespace Core.Tests
{
    public class ReadingTests
    {
        [Fact]
        public void ReadValidConfig()
        {
            var configFile = new FileInfo("../../../../Core.Tests/test_data/FMB001-default.cfg");
            var config = new DeviceConfiguration().LoadFromFile(configFile);
            
            Assert.NotNull(config.Parameters);
        }
        
        [Fact]
        public void ReadEmptyConfig()
        {
            var config = new DeviceConfiguration().LoadFromString("");
            
            Assert.Empty(config.Parameters);
        }
        
        [Fact]
        public void MetadataIsRead()
        {
            var config = new DeviceConfiguration().LoadFromString("DeviceName:FMP100;FirmwareVersion:1.0.1;1:20;2:30");
            var metadataIds = config.Metadata.Select(x => x.Id).ToList();
            
            Assert.Equal("DeviceName", config.Metadata[0].Id);
            Assert.Equal("FMP100", config.Metadata[0].Value);
            Assert.Equal("FirmwareVersion", config.Metadata[1].Id);
            Assert.Equal("1.0.1", config.Metadata[1].Value);
            Assert.DoesNotContain("1", metadataIds);
            Assert.DoesNotContain("2", metadataIds);
        }
    }
}