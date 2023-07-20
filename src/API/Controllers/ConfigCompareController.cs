using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Entities;
using Core.DTOs;
using System.Threading.Tasks;
using AutoMapper;
using Core;
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
        private readonly ILogger<ConfigCompareController> _logger;

        public ConfigCompareController(IMapper mapper, ILogger<ConfigCompareController> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public DeviceConfigurationComparisonDto CompareConfigs(IFormFile sourceConfig, IFormFile targetConfig)
        {
            var source = new DeviceConfiguration().LoadFromStream(sourceConfig.OpenReadStream());
            var target = new DeviceConfiguration().LoadFromStream(targetConfig.OpenReadStream());

            var comparison = new DeviceConfigurationComparison(source, target);

            return _mapper.Map<DeviceConfigurationComparisonDto>(comparison);
        }
    }
}