using System;
using CoreLocation;
using Edison.Core.Common.Models;
using MapKit;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ResponseMapView : UIView
    {
        readonly MKMapView mapView;

        CLLocation eventLocation;
        CLLocation userLocation;

        public CLLocation EventLocation
        {
            get => eventLocation;
            set
            {
                eventLocation = value;
                UpdateMap();
            }
        }

        public CLLocation UserLocation
        {
            get => userLocation;
            set
            {
                userLocation = value;
                UpdateMap();
            }
        }

        public ResponseMapView(CLLocation location = null)
        {
            mapView = new MKMapView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ShowsUserLocation = true,
                //UserInteractionEnabled = false,
            };

            AddSubview(mapView);
            mapView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            mapView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            mapView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            mapView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;

            EventLocation = location;
        }

        void UpdateMap()
        {
            if (eventLocation == null || userLocation == null)
            {
                return;
            }

            var latitudeDelta = Math.Abs(userLocation.Coordinate.Latitude - eventLocation.Coordinate.Latitude) + 0.2;
            var longitudeDelta = Math.Abs(userLocation.Coordinate.Longitude - eventLocation.Coordinate.Longitude) + 0.2;
            var spanRegion = new MKCoordinateRegion(eventLocation.Coordinate, new MKCoordinateSpan(latitudeDelta, longitudeDelta));
            var region = mapView.RegionThatFits(spanRegion);
            mapView.SetRegion(region, false);
            var annotation = new MKPointAnnotation { Coordinate = eventLocation.Coordinate };
            mapView.AddAnnotation(annotation);
        }
    }
}
