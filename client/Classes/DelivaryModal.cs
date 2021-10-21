using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace client.Classes
{
    public class DeliveryModal
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DriverName { get; set; }
        public string ContactNo { get; set; }
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
        public string KeyId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Distance { get; set; }
        public string Price { get; set; }
        [ServerTimestamp]
        public FieldValue TimeStamp { get; set; }
    }
}