namespace TaskManager.Utils
{
    public static class InputValidator
    {
        public static bool TryParseInt(string input, out int result)
        {
            return int.TryParse(input, out result) && result > 0;
        }

        public static bool TryParseBool(string input, out bool result)
        {
            result = false;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var lowerInput = input.Trim().ToLower();

            if (lowerInput == "да" || lowerInput == "yes" || lowerInput == "1" || lowerInput == "true" || lowerInput == "y")
            {
                result = true;
                return true;
            }

            if (lowerInput == "нет" || lowerInput == "no" || lowerInput == "0" || lowerInput == "false" || lowerInput == "n")
            {
                result = false;
                return true;
            }

            return false;
        }

        public static bool ValidateTitle(string title)
        {
            return !string.IsNullOrWhiteSpace(title) && title.Trim().Length >= 1 && title.Trim().Length <= 100;
        }

        public static string? ValidateDescription(string description)
        {
            if (description == null) return null;
            var trimmed = description.Trim();
            return trimmed.Length <= 500 ? trimmed : null;
        }
    }
}