using Android.Gms.Location;
using Android.Locations;
using System;

namespace driver.MapsHelper
{
    public class LocationCallBackHelper : LocationCallback
    {
        public event EventHandler<OnLocationCapturedEventArgs> CurrentLocation;
        public class OnLocationCapturedEventArgs : EventArgs
        {
            public Location Location_ { get; set; }
        }
        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            base.OnLocationAvailability(locationAvailability);
        }
        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Count != 0)
            {
                CurrentLocation?.Invoke(this, new OnLocationCapturedEventArgs { Location_ = result.Locations[0] });
            }
        }
    }
}