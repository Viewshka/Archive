using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.User.Queries.GetCurrentUser
{
    [BsonIgnoreExtraElements]
    public class CurrentUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DepartmentId { get; set; }
        public IList<string> Roles { get; set; }
        public string BriefName { get; set; }
    }
}