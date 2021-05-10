using System;
using System.Collections.Generic;
using Archive.Core.Collections.Identity;
using Archive.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Document.Queries.GetDocumentHistory
{
    [BsonIgnoreExtraElements]
    public class RequisitionInfoDto
    {
        public DateTime DateOfGiveOut { get; set; }
        public DateTime? DateOfReturn { get; set; }
        public DocumentUsageType UsageType { get; set; }
        public IList<string> Documents { get; set; }   
        public IList<ApplicationUser> Giver { get; set; }
        public IList<ApplicationUser> Recipient { get; set; }
    }
}