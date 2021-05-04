using AspNetCore.Identity.Mongo.Model;

namespace Archive.Core.Entities.Identity
{
    public class ApplicationUser : MongoUser<string>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DepartmentId { get; set; }
    }
}