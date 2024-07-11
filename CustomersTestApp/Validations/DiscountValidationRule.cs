using System.Globalization;
using System.Windows.Controls;

public class DiscountValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult(false, "Discount should not be null");
        }

        if (int.TryParse(value.ToString(), out int discount))
        {
            if (discount >= 0 && discount <= 30)
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Discount should be between 0 and 30");
        }

        return new ValidationResult(false, "Invalid discount value");
    }
}
