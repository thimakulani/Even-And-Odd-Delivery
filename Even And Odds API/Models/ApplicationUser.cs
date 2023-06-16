using Microsoft.AspNetCore.Identity;

namespace Even_And_Odds_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }

        public string Role { get; set; }
        public string RegNo { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Make { get; set; }

        public string Status { get; set; }
        /*[Ignored]
        public bool IsOnline => Convert.ToBoolean(Status);*/
    }
}
