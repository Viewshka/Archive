using System;
using Archive.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Document.Queries.GetInventory
{
    [BsonIgnoreExtraElements]
    public class InventoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public DateTime DocumentDate { get; set; }
        public DocumentTypeEnum Type { get; set; }
        public string Path { get; set; }
        public string NomenclatureId { get; set; }
        public string Number { get; set; }
    }
}