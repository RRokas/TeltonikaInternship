using System;
using System.ComponentModel.DataAnnotations;
using Core.Entities;
using Core.DTOs;
using API.Attributes;
using AutoMapper;
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
        /// <param name="filter">Filter options for the comparison results</param>
        /// <returns>Metadata of the configuration files and the comparison results (id, value, comparison result)</returns>
        [ProducesResponseType(typeof(DeviceConfigurationComparisonDto), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [HttpPost]
        public IActionResult CompareConfigs(
            [Required, MaxFileSize(5 * 1024 * 1024), AllowedExtensions(new []{".cfg"})] IFormFile sourceConfig,
            [Required, MaxFileSize(5 * 1024 * 1024), AllowedExtensions(new []{".cfg"})] IFormFile targetConfig,
            [FromQuery, Required, ComparisonFilterValidation] ComparisonFilterDto filter
            )
        {
            try
            {
                // log the requesters information with unique session identifier
                _logger.LogInformation($"Request from {Request.HttpContext.Connection.RemoteIpAddress} to compare {sourceConfig.FileName} and {targetConfig.FileName} received");
                var source = new DeviceConfiguration().LoadFromStream(sourceConfig.OpenReadStream(), sourceConfig.FileName);
                var target = new DeviceConfiguration().LoadFromStream(targetConfig.OpenReadStream(), targetConfig.FileName);

                var comparison = new DeviceConfigurationComparison(source, target);

                if (filter.FilterType == FilterType.ComparisonResult)
                    comparison.ApplyResultFilter(filter.FilterValue!);
                else if (filter.FilterType == FilterType.ParameterIdStartsWith) 
                    comparison.ApplyIdStartFilter(filter.FilterValue!);

                return Ok(_mapper.Map<DeviceConfigurationComparisonDto>(comparison));
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, $"Invalid argument supplied in {e.ParamName}");
                return BadRequest("One or more input parameters are invalid.");
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, $"There's an issue with the comparison");
                return BadRequest("An error occured during comparison operation.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Request from {Request.HttpContext.Connection.RemoteIpAddress} to compare {sourceConfig.FileName} and {targetConfig.FileName} failed");
                return BadRequest("An unexpected error occurred.");
            }
        }
    }
}