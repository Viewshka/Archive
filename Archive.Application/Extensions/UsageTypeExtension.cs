using Archive.Core.Enums;

namespace Archive.Application.Extensions
{
    public static class UsageTypeExtension
    {
        public static string GetString(this DocumentUsageType type)
        {
            return type switch
            {
                DocumentUsageType.Копирование => "Копирование",
                DocumentUsageType.Просмотр => "Просмотр",
                DocumentUsageType.Выписки => "Выписки",
                _ => string.Empty
            };
        }
    }
}