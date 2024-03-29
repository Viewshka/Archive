﻿using System;
using Archive.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Archive.Application.Feature.Document.Queries.GetDocumentsByNomenclature
{
    [BsonIgnoreExtraElements]
    public class DocumentsByNomenclatureDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Note { get; set; }
        public DateTime DocumentDate { get; set; }
        public DocumentTypeEnum Type { get; set; }
        public string NomenclatureId { get; set; }
        public string ParentId { get; set; }
        public Priority Priority { get; set; }
    }
}