using System;
using Archive.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Document.Queries.GetAkts
{
    [BsonIgnoreExtraElements]
    public class AktDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DocumentDate { get; set; }
        public DocumentTypeEnum Type { get; set; }
        public string Path { get; set; }
        public string NomenclatureId { get; set; }
        public Priority Priority { get; set; }
        public MediaType MediaType { get; set; }
        public string Number { get; set; }
    }
}