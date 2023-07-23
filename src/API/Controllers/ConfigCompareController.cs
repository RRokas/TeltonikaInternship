using System;
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
        /// <param name="filter">Optional. Comparison result type or start of parameter IDs can be filtered.</param>
        /// <param name="filterValue">Optional if filter is not used. Value to filter.</param>
        /// <returns>Metadata of the configuration files and the comparison results (id, value, comparison result)</returns>
        [ProducesResponseType(typeof(DeviceConfigurationComparisonDto), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [HttpPost]
        public IActionResult CompareConfigs([Required]IFormFile sourceConfig, [Required]IFormFile targetConfig, [FromQuery]ComparisonFilterDto filter)
        {
            if(filter != null && string.IsNullOrEmpty(filter.FilterValue))
                return BadRequest("Filter value is specified, but filter is not set.");

            try
            {
                var source = new DeviceConfiguration().LoadFromStream(sourceConfig.OpenReadStream());
                var target = new DeviceConfiguration().LoadFromStream(targetConfig.OpenReadStream());

                var comparison = new DeviceConfigurationComparison(source, target);

                if (filter == null) return Ok(_mapper.Map<DeviceConfigurationComparisonDto>(comparison));
                
                switch (filter.FilterType)
                {
                    case FilterType.ComparisonResult:
                        return Ok(comparison.GetParameterComparisonsByResult(filter.FilterValue));
                        break;
                    case FilterType.ParameterIdStartsWith:
                        return Ok(comparison.GetParameterComparisonsByParameterIdStart(filter.FilterValue));
                        break;
                }
                
                return Ok(_mapper.Map<DeviceConfigurationComparisonDto>(comparison));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}