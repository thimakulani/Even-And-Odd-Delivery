using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    public class Replies
    {
        public string Message { get; set; }
        [Id]
        public string Id { get; set; }
        public string Uid { get; set; }
        [ServerTimestamp]
        public Timestamp TimeStamp { get; set; }
    }
}