using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Entities;
using Core.DTOs;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigCompareController : ControllerBase
    {
        private readonly IMapper _mapper;
        
        public ConfigCompareController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpPost]
        public DeviceConfigurationComparisonDto CompareConfigs(IFormFile sourceConfig, IFormFile targetConfig)
        {
            // save files to disk temporarily
            var sourceConfigFile = new FileInfo(Path.GetTempFileName());
            var targetConfigFile = new FileInfo(Path.GetTempFileName());
            
            using (var stream = new FileStream(sourceConfigFile.FullName, FileMode.Create))
            {
                sourceConfig.CopyTo(stream);
            }
            
            using (var stream = new FileStream(targetConfigFile.FullName, FileMode.Create))
            {
                targetConfig.CopyTo(stream);
            }
            
            // load files into DeviceConfiguration objects
            var source = new DeviceConfiguration().LoadFromFile(sourceConfigFile);
            var target = new DeviceConfiguration().LoadFromFile(targetConfigFile);
            var comparison = new DeviceConfigurationComparison(source, target);
            return _mapper.Map<DeviceConfigurationComparisonDto>(comparison);
        }
    }
}