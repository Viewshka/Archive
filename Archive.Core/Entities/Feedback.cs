using System;
using Archive.Core.Entities.Identity;

namespace Archive.Core.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
    }
}