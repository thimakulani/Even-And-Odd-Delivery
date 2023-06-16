using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client.Classes
{
    public class DeliveryRequestModel
    {

        //parcle to be collected
        public string DriverId { get; set; }
        public string DestinationAddress { get; set; }
        public string RequestTime { get; set; }
        public string KeyId { get; set; }
        public string Status { get; set; }
        public string Distance { get; set; }
        public string Price { get; set; }
        public string PickupAddress { get; internal set; }
    }
}