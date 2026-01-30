namespace RealEstate.Application.Common.Normalizers;

public static class PhoneNumberNormalizer
{
    public static bool TryNormalize(string? input, out string normalized, out string error)
    {
        normalized = string.Empty;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Phone number is required.";
            return false;
        }

        var raw = input.Trim();

        // count separators (spases and hyphens (-))
        var separators = raw.Count(c => c is ' ' or '-');
        if (separators > 3)
        {
            error = "Phone number may contain at most 3 separators (spaces or hyphens).";
            return false;
        }
        
        var plusCount = raw.Count(c => c == '+');
        if (plusCount > 1 || (plusCount == 1 && raw[0] != '+'))
        {
            error = "Phone number may contain '+' only once and only at the start.";
            return false;
        }
        
        foreach (var ch in raw)
        {
            if (!(char.IsDigit(ch) || ch is ' ' or '-' || ch == '+'))
            {
                error = "Phone number contains invalid characters.";
                return false;
            }
        }
        
        var compact = raw.Replace(" ", "").Replace("-", "");

        // if country code is not provided, we automatically set +47
        if (!compact.StartsWith('+'))
        {
            compact = "+47" + compact;
        }

        // after '+' must be digits only
        if (compact.Length < 2 || compact[0] != '+' || compact.Skip(1).Any(c => !char.IsDigit(c)))
        {
            error = "Phone number must contain digits after country code.";
            return false;
        }

        var digitCount = compact.Length - 1; // exclude '+'
        if (digitCount < 8 || digitCount > 20)
        {
            error = "Phone number must have 8 to 20 digits (excluding '+').";
            return false;
        }

        normalized = compact;
        return true;
    }
}