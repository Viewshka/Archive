using System.Linq;
using Archive.Application.Feature.User.Queries.GetCurrentUser;

namespace Archive.Application.Extensions
{
    public static class CurrentUserDtoExtension
    {
        public static void SetBriefName(this CurrentUserDto user)
        {
            user.BriefName = $"{user.FirstName} {user.MiddleName?.First()}.";

            if (!string.IsNullOrWhiteSpace(user.LastName))
                user.BriefName += $"{user.LastName?.First()}.";
        }

        public static string GetBriefName(this CurrentUserDto user)
        {
            var result = string.Empty;
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                result += user.FirstName;
            
            if (!string.IsNullOrWhiteSpace(user.MiddleName))
                result += $" {user.MiddleName.First()}.";
            
            if (!string.IsNullOrWhiteSpace(user.LastName))
                result += $"{user.LastName.First()}.";
            
            return result;
        }
        
        public static string GetFullName(this CurrentUserDto user)
        {
            var result = string.Empty;
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                result += user.FirstName;
            
            if (!string.IsNullOrWhiteSpace(user.MiddleName))
                result += $" {user.MiddleName}";
            
            if (!string.IsNullOrWhiteSpace(user.LastName))
                result += $" {user.LastName}";
            
            return result;
        }
        
        public static string GetBriefNameWithJobTitle(this CurrentUserDto user)
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