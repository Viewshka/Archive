using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Requisition.Queries.GetRequisitionPreview
{
    [BsonIgnoreExtraElements]
    public class RequisitionDto
    {
        public string Id { get; set; }
        public string Path { get; set; }
    }
}