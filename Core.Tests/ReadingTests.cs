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
    }
}