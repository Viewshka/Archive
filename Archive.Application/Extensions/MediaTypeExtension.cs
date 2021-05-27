using Archive.Core.Enums;

namespace Archive.Application.Extensions
{
    public static class MediaTypeExtension
    {
        public static string GetString(this MediaType type)
        {
            return type switch
            {
                MediaType.Бумажный => "Бумажный",
                MediaType.Электронный => "Электронный",
                _ => string.Empty
            };
        }
    }
}