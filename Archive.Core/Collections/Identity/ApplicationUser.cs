using Archive.Core.Enums;
using AspNetCore.Identity.Mongo.Model;

namespace Archive.Core.Collections.Identity
{
    public class ApplicationUser : MongoUser<string>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DepartmentId { get; set; }
        public Priority Priority { get; set; }
        public string JobTitle { get; set; }
    }
}