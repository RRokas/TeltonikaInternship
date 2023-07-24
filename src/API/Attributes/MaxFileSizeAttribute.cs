using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace API.Attributes;

/// <summary>
/// Attribute for validating the size of an uploaded file.
/// </summary>
public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;
    /// <summary>
    /// Limits the size of the uploaded file to the specified number of bytes.
    /// </summary>
    /// <param name="maxFileSize">Maximum file size in bytes</param>
    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }
    
    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
            throw new ValidationException("Attribute can only be used on IFormFile properties.");
        
        if (file.Length > _maxFileSize)
            return new ValidationResult(GetErrorMessage());

        return ValidationResult.Success;

    }

    public string GetErrorMessage()
    {
        return $"Maximum allowed file size is { _maxFileSize} bytes.";
    }
}