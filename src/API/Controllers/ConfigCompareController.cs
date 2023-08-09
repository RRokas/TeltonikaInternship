using System;
using System.IO;
using Core.Entities;
using Core.DTOs;
using AutoMapper;
using Core.Enums;
using FluentValidation;
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
        private readonly IValidator<ComparisonRequest> _validator;

        public ConfigCompareController(IMapper mapper, ILogger<ConfigCompareController> logger, IValidator<ComparisonRequest> validator)
        {
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }
        
        /// <summary>
        /// Compares two device configuration files and returns the result, optionally filtered by a filter.
        /// </summary>
        /// <param name="request">Wrapper containing source and target definition, filter type. Filter value is required if any filtering is needed.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(DeviceConfigurationComparisonDto), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [HttpPost]
        public IActionResult CompareConfigs([FromForm]ComparisonRequest request)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Request from {Request.HttpContext.Connection.RemoteIpAddress} failed validation");
                return BadRequest(validationResult.ToDictionary());
            }
            
            var sourceConfig = request.SourceFile;
            var targetConfig = request.TargetFile;
            
            try
            {
                _logger.LogInformation(
                    $"Request from {Request.HttpContext.Connection.RemoteIpAddress} to compare {sourceConfig.FileName} and {targetConfig.FileName} received");
                var source =
                    new DeviceConfiguration().LoadFromStream(sourceConfig.OpenReadStream(), sourceConfig.FileName);
                var target =
                    new DeviceConfiguration().LoadFromStream(targetConfig.OpenReadStream(), targetConfig.FileName);

                var comparison = new DeviceConfigurationComparison(source, target);
                
                var filter = request.Filter;
                if (filter.FilterType == FilterType.ComparisonResult)
                    comparison.ApplyResultFilter(filter.FilterValue!);
                else if (filter.FilterType == FilterType.ParameterIdStartsWith)
                    comparison.ApplyIdStartFilter(filter.FilterValue!);

                return Ok(_mapper.Map<DeviceConfigurationComparisonDto>(comparison));
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "One or more input parameters are invalid.");
                return BadRequest("One or more input parameters are invalid.");
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "An error occured during comparison operation.");
                return BadRequest("An error occured during comparison operation.");
            }
            catch (InvalidDataException e)
            {
                _logger.LogError(e, "Invalid data. {errorMessage}", e.Message);
                return BadRequest($"Invalid data. {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Request from {Request.HttpContext.Connection.RemoteIpAddress} to compare {sourceConfig.FileName} and {targetConfig.FileName} failed");
                return BadRequest("An unexpected error occurred.");
            }
        }
    }
}