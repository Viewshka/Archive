using Archive.Core.Collections.Identity;

namespace Archive.Application.Extensions
{
    public static class ApplicationUserExtension
    {
        public static string GetFullName(this ApplicationUser user)
        {
            var result = $"{user.FirstName} {user.MiddleName}";

            if (!string.IsNullOrWhiteSpace(user.LastName))
                result += $" {user.LastName}";

            return result;
        }
    }
}