using Android.Gms.Location;
using Android.Locations;
using System;

namespace client.MapsHelper
{
    public class LocationCallBackHelper : LocationCallback
    {
        public event EventHandler<OnLocationCapturedEventArgs> CurrentLocation;
        public class OnLocationCapturedEventArgs : EventArgs
        {
            public Location location { get; set; }
        }
        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            base.OnLocationAvailability(locationAvailability);
        }
        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Count != 0)
            {
                CurrentLocation?.Invoke(this, new OnLocationCapturedEventArgs { location = result.Locations[0] });
            }
        }
    }
}