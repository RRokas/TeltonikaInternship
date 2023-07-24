using System.ComponentModel.DataAnnotations;
using Core.DTOs;
using Core.Enums;

namespace API.Attributes;

public class ComparisonFilterValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        if (value is not ComparisonFilterDto filter) throw new ValidationException("ComparisonFilterValidation can only be used on ComparisonFilterDto properties.");
        if (filter.FilterType != FilterType.None && string.IsNullOrEmpty(filter.FilterValue))
        {
            return new ValidationResult($"Filter value must be provided when filter type is not None.");
        }
        
        return ValidationResult.Success;
        
    }
}