using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace API.Attributes;

/// <summary>
/// Attribute for validating the file extension of an uploaded file.
/// </summary>
public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    /// <summary>
    /// Limits the file extensions of the uploaded file to the specified extensions.
    /// </summary>
    /// <param name="extensions">Allowed extensions</param>
    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }
    
    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        if (value is not IFormFile file) throw new ValidationException("Attribute can only be used on IFormFile properties.");
        
        var extension = Path.GetExtension(file.FileName);
        if (_extensions.Contains(extension.ToLower())) return ValidationResult.Success;
        return new ValidationResult($"Extension {extension} is not allowed. Allowed extensions are: {string.Join(", ", _extensions)}.");

    }
}
