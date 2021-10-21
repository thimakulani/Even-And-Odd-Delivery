using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using System;

namespace client.Classes
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