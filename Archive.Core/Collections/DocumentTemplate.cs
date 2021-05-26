using Archive.Core.Collections.Common;
using Archive.Core.Enums;

namespace Archive.Core.Collections
{
    public class DocumentTemplate : Entity
    {
        public string Path { get; set; }
        public DocumentTemplateEnum Type { get; set; }

        public string FileName { get; set; }
    }
}