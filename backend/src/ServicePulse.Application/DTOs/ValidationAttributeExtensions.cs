using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ServicePulse.Application.DTOs;

public static class ValidationAttributeExtensions
{
    public static bool TryValidate(this object model)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
    }

    public static bool IsValidId(string? value)
    {
        if (value is null)
            return false;

        var digits = Regex.Replace(value, @"[^0-9]", string.Empty);
        return digits.Length == 14 && int.TryParse(digits, out _);
    }
}