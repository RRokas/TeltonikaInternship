﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Core.Entities;
using Core.DTOs;
using System.Threading.Tasks;
using AutoMapper;
using Core;
using Core.Enums;
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
        
        
        /// <summary>
        /// Compares two device configurations
        /// </summary>
        /// <param name="sourceConfig">Configuration file in .cfg format</param>
        /// <param name="targetConfig">Configuration file in .cfg format</param>
        /// <returns>Metadata of the configuration files and the comparison results (id, value, comparison result)</returns>
        [ProducesResponseType(typeof(DeviceConfigurationComparisonDto), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [HttpPost]
        public IActionResult CompareConfigs([Required]IFormFile sourceConfig, [Required]IFormFile targetConfig, [FromQuery, Required]ComparisonFilterDto filter)
        {
            if(filter.FilterType != FilterType.None && string.IsNullOrEmpty(filter.FilterValue))
                return BadRequest("Filter value is specified, but filter is not set.");

            try
            {
                var source = new DeviceConfiguration().LoadFromStream(sourceConfig.OpenReadStream(), sourceConfig.FileName);
                var target = new DeviceConfiguration().LoadFromStream(targetConfig.OpenReadStream(), targetConfig.FileName);

                var comparison = new DeviceConfigurationComparison(source, target);

                if (filter.FilterType == FilterType.ComparisonResult)
                    comparison.ApplyResultFilter(filter.FilterValue);
                else if (filter.FilterType == FilterType.ParameterIdStartsWith) 
                    comparison.ApplyIdStartFilter(filter.FilterValue);

                return Ok(_mapper.Map<DeviceConfigurationComparisonDto>(comparison));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}