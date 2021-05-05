using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Archive.Core.Collections
{
    public class Department
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        public string ShortName { get; set; }
        public string FullName { get; set; }
        public int ParentId { get; set; }
    }
}