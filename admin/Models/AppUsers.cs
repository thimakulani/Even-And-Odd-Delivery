using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    public class AppUsers
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [Id]
        public string Uid { get; set; }
        public string Role { get; set; }
        public string RegNo { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Make { get; set; }
    }
}