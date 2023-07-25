using System;
using System.Linq;
using System.Linq.Expressions;
using API.Configuration;
using Core.DTOs;
using Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.FluentValidations;

public class FormFileValidator : AbstractValidator<ComparisonRequest>
{
    private readonly ComparisonRequestSettings _settings;
    
    public FormFileValidator(IOptions<ComparisonRequestSettings> options)
    {
        _settings = options.Value; ;
        
        ApplyFileRules(request => request.SourceFile, nameof(ComparisonRequest.SourceFile));
        ApplyFileRules(request => request.TargetFile, nameof(ComparisonRequest.TargetFile));  
        
        RuleFor(request => request.Filter)
            .NotNull()
            .WithMessage("Filter type must be provided.")
            .Must(HaveFilterValueWhenFilterTypeIsNotNone)
            .WithMessage("Filter value must be provided when filter type is not None.")
            .Must(BeValidFilterType)
            .WithMessage($"Filter type must be one of the following: {string.Join(",", Enum.GetNames(typeof(FilterType)))}");
    }
    
    private void ApplyFileRules(Expression<Func<ComparisonRequest, IFormFile>> expression, string propertyName)
    {
        RuleFor(expression)
            .NotNull()
            .WithMessage($"{propertyName} file must be provided.")
            .Must(BeWithinAllowedFileSizeLimit)
            .WithMessage($"{propertyName} file size must be less than {_settings.MaxSize / 1024}KB.")
            .Must(BeAnAllowedFileExtension)
            .WithMessage($"{propertyName} file extension must be one of the following: {string.Join(", ", _settings.AllowedExtensions)}");
    }
    
    private bool BeValidFilterType(ComparisonFilterDto filter)
    {
        return Enum.IsDefined(typeof(FilterType), filter.FilterType);
    }
    
    private bool HaveFilterValueWhenFilterTypeIsNotNone(ComparisonFilterDto filter)
    {
        return filter.FilterType == FilterType.None || !string.IsNullOrEmpty(filter.FilterValue);
    }
    
    private bool BeWithinAllowedFileSizeLimit(IFormFile file)
    {
        return file.Length <= _settings.MaxSize;
    }
    
    private bool BeAnAllowedFileExtension(IFormFile file)
    {
        return _settings.AllowedExtensions.Any(ext => file.FileName.EndsWith(ext));
    }
}