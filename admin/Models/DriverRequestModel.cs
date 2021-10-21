using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    public class DriverRequestModel
    {
        public string AltPhoneNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        [Id]
        public string KeyId { get; set; }
        public string Status { get; set; }
        public string Make { get; set; }
        public string RegNo { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}