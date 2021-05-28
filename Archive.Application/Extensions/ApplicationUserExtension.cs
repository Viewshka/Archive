using System;
using System.Linq;
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
        
        public static string GetBriefNameWithJobTitle(this ApplicationUser user)
        {
            var result = string.Empty;

            if (!string.IsNullOrWhiteSpace(user.JobTitle))
                result += user.JobTitle;
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                result += $" {user.FirstName}";
            if (!string.IsNullOrWhiteSpace(user.MiddleName))
                result += $" {user.MiddleName.First()}.";
            if (!string.IsNullOrWhiteSpace(user.LastName))
                result += $"{user.LastName.First()}.";

            return result;
        }
    }
}