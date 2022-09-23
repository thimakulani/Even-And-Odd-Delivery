using Plugin.CloudFirestore.Attributes;
using FieldValue = Plugin.CloudFirestore.FieldValue;
using ServerTimestampAttribute = Plugin.CloudFirestore.Attributes.ServerTimestampAttribute;

namespace client.Classes
{
    public class DeliveryModal
    {
        [Ignored]
        public string Name { get; set; }
        [Ignored]
        public string Surname { get; set; }
        public string DriverName { get; set; }
        [Ignored]
        public string ContactNo { get; set; }
        [Ignored]
        public string AlteContactNo { get; set; }
        //parcle to be collected
        public string DriverId { get; set; }
        public string ItemType { get; set; }
        public string PickupAddress { get; set; }
        public string PickupLat { get; set; }
        public string PickupLong { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationLat { get; set; }
        public string DestinationLong { get; set; }
        //public string Date { get; set; }
        //public string Time { get; set; }
        public string PersonName { get; set; }
        public string PersonContact { get; set; }
        public string PaymentType { get; set; }
        public string RequestTime { get; set; }
        [Id]
        public string KeyId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Distance { get; set; }
        public string Price { get; set; }
        [Ignored]
        [ServerTimestamp]
        public FieldValue TimeStamp { get; set; }
    }
}