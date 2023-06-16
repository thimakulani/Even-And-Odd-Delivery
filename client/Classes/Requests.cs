using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client.Classes
{
    public class Requests
    {
        public string Id { get; set; }
        //parcle to be collected
        public string ItemType { get; set; }
        public string PickupAddress { get; set; }
        public double PickupLat { get; set; }
        public double PickupLong { get; set; }
        public string DestinationAddress { get; set; }
        public double DestinationLat { get; set; }
        public double DestinationLong { get; set; }
        //public string Date { get; set; }
        public string PersonName { get; set; }
        public string PersonContact { get; set; }
        public string PaymentType { get; set; }
        public DateTime RequestTime { get; set; }
        public string UserId { get; set; }
        //public ApplicationUser User { get; set; }
        public string Status { get; set; }
        public double Distance { get; set; }
        public double Price { get; set; }
        public string DriverId { get; set; }
        // public ApplicationUser Driver { get; set; }
    }
}