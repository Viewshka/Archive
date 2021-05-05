using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Nomenclature.Queries.GetAllNomenclatures
{
    [BsonIgnoreExtraElements]
    public class NomenclatureDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Index { get; set; }
        public string DepartmentId { get; set; }
        public int Year { get; set; }
    }
}