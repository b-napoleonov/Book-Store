using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.CustomAttributes
{
    /// <summary>
    /// Custom attribute checking if the range is from filed to current year
    /// </summary>
    public class CurrentYearValueAttribute : ValidationAttribute
    {
        public CurrentYearValueAttribute(int minYear)
        {
            MinYear = minYear;
            this.ErrorMessage = $"Value should be between {minYear} and {DateTime.UtcNow.Year}.";
        }

        public int MinYear { get; }

        /// <summary>
        /// Overrides the default IsValid method
        /// </summary>
        /// <param name="value">Filed value</param>
        /// <returns>True if filed value is int and in the range from current year to filed year, otherwise false</returns>
        public override bool IsValid(object? value)
        {
            if (value is int intValue)
            {
                if (intValue <= DateTime.UtcNow.Year
                    && intValue >= MinYear)
                {
                    return true;
                }
            }

            if (value is DateTime dtValue)
            {
                if (dtValue.Year <= DateTime.UtcNow.Year
                    && dtValue.Year >= MinYear)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
