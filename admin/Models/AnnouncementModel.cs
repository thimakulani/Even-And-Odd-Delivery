using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    public class AnnouncementModel
    {
        public string Message { get; set; }
        [Id]
        public string Id { get; set; }
        [ServerTimestamp]
        public Timestamp TimeStamp { get; set; }
    }
}