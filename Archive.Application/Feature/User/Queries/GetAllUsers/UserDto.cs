using System.Collections.Generic;
using Archive.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.User.Queries.GetAllUsers
{
    [BsonIgnoreExtraElements]
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DepartmentId { get; set; }
        public IList<string> Roles { get; set; }
        public string BriefName { get; set; }
        public bool IsUserArchivist { get; set; }
        public Priority Priority { get; set; }
    }
}