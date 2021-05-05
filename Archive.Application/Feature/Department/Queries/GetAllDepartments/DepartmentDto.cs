using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Department.Queries.GetAllDepartments
{
    [BsonIgnoreExtraElements]
    public class DepartmentDto
    {
        public string Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string ParentId { get; set; }
    }
}