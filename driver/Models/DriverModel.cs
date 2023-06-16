using Plugin.CloudFirestore.Attributes;
using System;

namespace driver.Models
{
    public class DriverModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Uid { get; set; }
        public string Role { get; set; }
        public string RegNo { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Make { get; set; }

        public object Status { get; set; }
        [Ignored]
        public bool IsOnline => Convert.ToBoolean(Status);
        public Car Car { get; set; }

    }
}