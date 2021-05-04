using System.Linq;
using Archive.Application.Feature.User.Quries.GetCurrentUser;

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
    }
}