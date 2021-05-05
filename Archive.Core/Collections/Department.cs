using Archive.Core.Collections.Common;

namespace Archive.Core.Collections
{
    public class Department : Entity
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string ParentId { get; set; }
    }
}