// Validation/TimeValidationRule.cs
using System;
using System.Globalization;
using System.Windows.Controls;

namespace BDAS2_SEM.Validation
{
    public class TimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string timeString = (value ?? "").ToString();

            if (string.IsNullOrWhiteSpace(timeString))
            {
                return new ValidationResult(false, "Time is required.");
            }

            // Supports formats HH:MM or HH:MM:SS
            if (TimeSpan.TryParseExact(timeString, new[] { @"hh\:mm", @"hh\:mm\:ss" }, CultureInfo.InvariantCulture, out _))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Invalid time format. Use HH:MM or HH:MM:SS.");
        }
    }
}