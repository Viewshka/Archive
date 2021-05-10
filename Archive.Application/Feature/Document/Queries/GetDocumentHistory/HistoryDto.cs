using System;

namespace Archive.Application.Feature.Document.Queries.GetDocumentHistory
{
    public class HistoryDto
    {
        public DateTime DateOfGiveOut { get; set; }
        public DateTime? DateOfReturn { get; set; }
        public string UsageType { get; set; }
        public string Giver { get; set; }
        public string Recipient { get; set; }
    }
}