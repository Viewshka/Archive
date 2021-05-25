using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Role.Queries.GetAllRoles
{
    [BsonIgnoreExtraElements]
    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}