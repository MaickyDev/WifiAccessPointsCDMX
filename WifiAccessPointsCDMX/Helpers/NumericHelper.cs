using System.Globalization;

namespace WifiAccessPointsCDMX.Helpers
{
    public static class NumericHelper
    {
        public static double SanitizeDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Numeric value is empty");

            value = value.Trim();

            // Remove trailing commas
            while (value.EndsWith(",") || value.EndsWith("."))
                value = value[..^1];

            // Remove thousands separators
            value = value.Replace(",", "");

            // Normalize whitespaces
            value = value.Replace(" ", "");

            return double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}