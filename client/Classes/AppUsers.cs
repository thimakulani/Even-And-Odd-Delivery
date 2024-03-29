﻿using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace client.Classes
{
    public class AppUsers
    {
        public string Id { get; set; }
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
        public GeoPoint Location { get; set; }


    }
}