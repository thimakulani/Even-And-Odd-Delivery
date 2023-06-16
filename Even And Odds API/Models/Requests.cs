using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Even_And_Odds_API.Models
{
    public class Requests
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string ContactNo { get; set; }
        //parcle to be collected
        public string ItemType { get; set; }
        public string PickupAddress { get; set; }
        public double PickupLat { get; set; }
        public double PickupLong { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationLat { get; set; }
        public double DestinationLong { get; set; }
        //public string Date { get; set; }
        public string PersonName { get; set; }
        public string PersonContact { get; set; }
        public string PaymentType { get; set; }
        public string RequestTime { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Status { get; set; }
        public string Distance { get; set; }
        public double Price { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string DriverId { get; set; }
        public ApplicationUser Driver { get; set; }
    }
}
