using System.Text;
using RealEstate.Application.Common.Normalizers;

public static class EmailNormalizer
{
    public static string NormalizeEmail(string email)
    {
        var s = (email ?? string.Empty).Trim().Normalize(NormalizationForm.FormKC);

        // Remove any whitespace and common zero-width chars (often comes from copy-paste operations)
        var cleaned = new string(s
            .Where(c =>
                    !char.IsWhiteSpace(c) &&
                    c != '\u200B' && // invisible space (might appear when user copy-paste)
                    c != '\uFEFF' // zero-width no-break space (sometimes appears at the start)
            )
            .ToArray());

        return cleaned.ToLowerInvariant();
    }
}