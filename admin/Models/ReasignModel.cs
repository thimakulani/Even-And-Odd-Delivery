using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    class ReasignModel
    {
        public string DatesTime { get; set; }
        public string DriverNames { get; set; }
        public string Address { get; set; }
        [Id]
        public string Key { get; set; }
        public string Status { get; set; }
    }
}