using System;
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
            var config = new DeviceConfiguration(configFile);
            
            Assert.NotNull(config.Parameters);
        }

        [Fact]
        public void CorrectParameterTypesParsed()
        {
            var configFile = new FileInfo("../../../../Core.Tests/test_data/FMB001-default.cfg");
            var config = new DeviceConfiguration(configFile);
            Assert.IsType<string>(config.Parameters[0].Id);
            Assert.IsType<string>(config.Parameters[0].Value);
            Assert.IsType<string>(config.Parameters[1].Id);
            Assert.IsType<string>(config.Parameters[1].Value);
            Assert.IsType<string>(config.Parameters[5].Id);
            Assert.IsType<int>(config.Parameters[5].Value);
            Assert.IsType<int>(config.Parameters[6].Id);
            Assert.IsType<int>(config.Parameters[6].Value);
            var shouldBeDoubleValue = config.Parameters
                .Where(x => x.Id.GetType().ToString() == "System.Int32")
                .Single(x => (int)x.Id == 11004).Value;
            Assert.IsType<double>(shouldBeDoubleValue);
        }
    }
}