using Archive.Core.Collections.Common;

namespace Archive.Core.Collections
{
    public class Nomenclature : Entity
    {
        public string Name { get; set; }
        public string Index { get; set; }
        public string DepartmentId { get; set; }
        public int Year { get; set; }
        public bool IsEdit { get; set; }
    }
}