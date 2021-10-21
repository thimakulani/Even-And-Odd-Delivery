using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace client.Classes
{
    public class Queries
    {
        [Id]
        public string Id { get; set; }
        public string Message { get; set; }
        public string Uid { get; set; }
        [ServerTimestamp]
        public Timestamp TimeStamp { get; set; }
    }
}