using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.File.Queries
{
    [BsonIgnoreExtraElements]
    public class DocumentDto
    {
        public string Id { get; set; }
        public string Path { get; set; }
    }
}