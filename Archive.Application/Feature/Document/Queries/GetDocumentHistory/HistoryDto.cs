using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Document.Queries.GetDocumentHistory
{
    [BsonIgnoreExtraElements]
    public class HistoryDto
    {
        public DateTime? DateOfGiveOut { get; set; }
        public DateTime? DateOfReturn { get; set; }
        public string UsageType { get; set; }
        public string Giver { get; set; }
        public string Recipient { get; set; }

        public string Status { get; set; }
    }
}