using System;
using Archive.Core.Enums;

namespace Archive.Application.Feature.Document.Queries.GetAllDocuments
{
    public class DocumentDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Note { get; set; }
        public DateTime DocumentDate { get; set; }
        public DocumentTypeEnum Type { get; set; }
        public string NomenclatureId { get; set; }
    }
}