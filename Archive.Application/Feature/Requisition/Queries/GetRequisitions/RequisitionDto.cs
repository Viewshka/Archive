using System;
using System.Collections.Generic;
using Archive.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Requisition.Queries.GetRequisitions
{
    [BsonIgnoreExtraElements]
    public class RequisitionDto
    {
        public string Id { get; set; }
        public string RecipientId { get; set; }
        public string GiverId { get; set; }
        public IList<string> Documents { get; set; }
        public DateTime DateOfCreated { get; set; }
        public DateTime? DateOfGiveOut { get; set; }
        public DateTime? DateOfReturn { get; set; }
        public DocumentUsageType UsageType { get; set; }
        public RequisitionStatusEnum Status { get; set; }
    }
}