using System.Linq;
using Core.DTOs;
using Core.Enums;
using Core.FluentValidations;
using Core.FluentValidations.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Core.Tests
{
    public class ComparisonRequestValidatorTests
    {
        [Fact]
        public void NoneFilterTypeAllowsNullFilterValue()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("config.cfg");
            fileMock.Setup(f => f.Length).Returns(1024);

            var settings = new ComparisonRequestSettings
            {
                MaxSize = 100000,
                AllowedExtensions = new[] { ".cfg" }
            };

            var validator = new ComparisonRequestValidator(Options.Create(settings));
            var request = new ComparisonRequest
            {
                Filter = new ComparisonFilterDto
                {
                    FilterType = FilterType.None,
                    FilterValue = null
                },
                SourceFile = fileMock.Object,
                TargetFile = fileMock.Object
            };

            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ResultStatusFilterAllowsEnumValuesOnly()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("config.cfg");
            fileMock.Setup(f => f.Length).Returns(1024);

            var settings = new ComparisonRequestSettings
            {
                MaxSize = 100000,
                AllowedExtensions = new[] { ".cfg" }
            };

            var validator = new ComparisonRequestValidator(Options.Create(settings));
            var request = new ComparisonRequest
            {
                Filter = new ComparisonFilterDto
                {
                    FilterType = FilterType.ComparisonResult,
                    FilterValue = "SomeInvalidValue"
                },
                SourceFile = fileMock.Object,
                TargetFile = fileMock.Object
            };

            var result = validator.Validate(request);
            Assert.NotNull(result.Errors.First(error =>
                ((ComparisonFilterDto)error.AttemptedValue).FilterValue == "SomeInvalidValue"));
        }
        
        [Fact]
        public void ParameterIdStartsWithFilterDoesNotAllowNullFilterValue()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("config.cfg");
            fileMock.Setup(f => f.Length).Returns(1024);

            var settings = new ComparisonRequestSettings
            {
                MaxSize = 100000,
                AllowedExtensions = new[] { ".cfg" }
            };

            var validator = new ComparisonRequestValidator(Options.Create(settings));
            var request = new ComparisonRequest
            {
                Filter = new ComparisonFilterDto
                {
                    FilterType = FilterType.ParameterIdStartsWith,
                    FilterValue = null
                },
                SourceFile = fileMock.Object,
                TargetFile = fileMock.Object
            };

            var result = validator.Validate(request);
            Assert.NotNull(result.Errors.First(error =>
                ((ComparisonFilterDto)error.AttemptedValue).FilterType == FilterType.ParameterIdStartsWith));
        }
    }
}