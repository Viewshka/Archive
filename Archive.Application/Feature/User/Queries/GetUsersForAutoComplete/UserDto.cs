using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.User.Queries.GetUsersForAutoComplete
{
    [BsonIgnoreExtraElements]
    public class UserDto
    {
        public string UserId { get; set; }
        public string BriefName { get; set; }
    }
}