namespace Application.Features.Properties.Common;

public static class PropertyImageUrlsNormalizer
{
    public static IReadOnlyList<string> Normalize(
        string? mainImageUrl,
        IEnumerable<string>? imageUrls)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static string? Clean(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }

        void Add(string? value)
        {
            var cleaned = Clean(value);

            if (cleaned is null)
            {
                return;
            }

            if (seen.Add(cleaned))
            {
                result.Add(cleaned);
            }
        }

        Add(mainImageUrl);

        if (imageUrls is not null)
        {
            foreach (var imageUrl in imageUrls)
            {
                Add(imageUrl);
            }
        }

        return result;
    }
}