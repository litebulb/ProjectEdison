using System;
using System.Collections.Generic;
using CoreGraphics;
using CoreLocation;
using Edison.Core.Common.Models;
using Edison.Mobile.iOS.Common.Extensions;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.DataSources;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using MapKit;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class ResponseDetailsViewController : BaseViewController<ResponseDetailsViewModel>, IMKMapViewDelegate
    {
        readonly float iconSize = 64;
        readonly float mapViewHeightFactor = 0.4f;
        readonly float headerHeight = 44;

        nfloat mapOverflowConstant;

        bool isInitialRender = true;
        bool isUserControllingMapRegion;

        UIView navBarBackgroundView;
        UIView tableViewBackgroundView;
        MKMapView mapView;
        UIView iconBackgroundView;
        UIImageView iconImageView;
        CLLocationCoordinate2D eventCoordinate;
        CLLocationCoordinate2D lastUserCoordinate;
        ResponseDetailsContainerView containerView;
        UITableView tableView;
        ResponseUpdatesTableViewSource responseUpdatesTableViewSource;
        ResponseUpdateTableViewHeaderView tableHeaderView;

        NSLayoutConstraint mapViewTopConstraint;
        NSLayoutConstraint tableViewBackgroundViewTopConstraint;

        UIPanGestureRecognizer mapPanGestureRecognizer;
        UITapGestureRecognizer iconTapGestureRecognizer;

        public override UIView View
        {
            get => containerView ?? (containerView = new ResponseDetailsContainerView());
        }

        public ResponseDetailsViewController(ResponseCollectionItemViewModel response)
        {
            ViewModel.Response = response.Response;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            mapOverflowConstant = View.Bounds.Height / 2;

            Title = ViewModel.Response?.ActionPlan?.Name;

            View.BackgroundColor = Constants.Color.BackgroundDarkGray;
            View.ClipsToBounds = true;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.CloseX, UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });

            NavigationController.NavigationBar.TintColor = Constants.Color.White;
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes { ForegroundColor = Constants.Color.White };

            var statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
            var navigationBarFrame = NavigationController.NavigationBar.Frame;
            var navBarBackgroundHeight = statusBarFrame.Height + navigationBarFrame.Height;

            navBarBackgroundView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.MapFromActionPlanColor(ViewModel.Response.ActionPlan.Color),
            };

            View.AddSubview(navBarBackgroundView);

            navBarBackgroundView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            navBarBackgroundView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            navBarBackgroundView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            navBarBackgroundView.HeightAnchor.ConstraintEqualTo(navBarBackgroundHeight).Active = true;

            mapView = new MKMapView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ShowsUserLocation = true,
                Delegate = this,
                Alpha = 0,
            };

            View.AddSubview(mapView);
            mapViewTopConstraint = mapView.TopAnchor.ConstraintEqualTo(navBarBackgroundView.BottomAnchor, constant: -mapOverflowConstant);
            mapViewTopConstraint.Active = true;
            mapView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            mapView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            mapView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, multiplier: mapViewHeightFactor, constant: mapOverflowConstant * 2).Active = true;

            iconBackgroundView = new UIView
            {
                BackgroundColor = Constants.Color.MapFromActionPlanColor(ViewModel.Response.ActionPlan.Color) ?? Constants.Color.LightGray,
                Frame = new CGRect
                {
                    Location = CGPoint.Empty,
                    Size = new CGSize(iconSize, iconSize),
                },
                Alpha = 0,
            };

            iconBackgroundView.Layer.CornerRadius = iconSize / 2;
            iconBackgroundView.Layer.ShadowColor = Constants.Color.Black.CGColor;
            iconBackgroundView.Layer.ShadowRadius = 4;
            iconBackgroundView.Layer.ShadowOpacity = 0.2f;
            iconBackgroundView.Tag = 1;

            View.AddSubview(iconBackgroundView);

            iconImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Constants.Assets.MapFromActionPlanIcon(ViewModel.Response.ActionPlan.Icon),
            };

            iconBackgroundView.AddSubview(iconImageView);
            iconImageView.CenterXAnchor.ConstraintEqualTo(iconBackgroundView.CenterXAnchor).Active = true;
            iconImageView.CenterYAnchor.ConstraintEqualTo(iconBackgroundView.CenterYAnchor).Active = true;
            iconImageView.WidthAnchor.ConstraintEqualTo(iconBackgroundView.WidthAnchor, multiplier: 0.5f).Active = true;
            iconImageView.HeightAnchor.ConstraintEqualTo(iconBackgroundView.HeightAnchor, multiplier: 0.5f).Active = true;

            responseUpdatesTableViewSource = new ResponseUpdatesTableViewSource();

            tableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                RowHeight = UITableView.AutomaticDimension,
                BackgroundColor = UIColor.Clear,
                TableFooterView = new UIView(),
                BackgroundView = null,
                Source = responseUpdatesTableViewSource,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
            };

            tableHeaderView = new ResponseUpdateTableViewHeaderView
            {
                Frame = new CGRect(0, 0, View.Bounds.Width, headerHeight),
                UserInteractionEnabled = false,
            };

            tableHeaderView.Layer.ShadowColor = Constants.Color.Black.CGColor;
            tableHeaderView.Layer.ShadowRadius = 4;
            tableHeaderView.Layer.ShadowOpacity = 0.2f;

            tableView.RegisterClassForCellReuse(typeof(ResponseUpdateTableViewCell), typeof(ResponseUpdateTableViewCell).Name);

            View.InsertSubviewBelow(tableView, iconBackgroundView);

            tableView.TopAnchor.ConstraintEqualTo(navBarBackgroundView.BottomAnchor).Active = true;
            tableView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            tableView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            tableView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            View.InsertSubviewBelow(tableHeaderView, iconBackgroundView);

            View.BringSubviewToFront(navBarBackgroundView);

            tableViewBackgroundView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = Constants.Color.BackgroundDarkGray };
            View.InsertSubviewBelow(tableViewBackgroundView, tableView);
            tableViewBackgroundViewTopConstraint = tableViewBackgroundView.TopAnchor.ConstraintEqualTo(tableView.TopAnchor);
            tableViewBackgroundViewTopConstraint.Active = true;
            tableViewBackgroundView.LeftAnchor.ConstraintEqualTo(tableView.LeftAnchor).Active = true;
            tableViewBackgroundView.RightAnchor.ConstraintEqualTo(tableView.RightAnchor).Active = true;
            tableViewBackgroundView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            var lineView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = Constants.Color.DarkGray };
            tableViewBackgroundView.AddSubview(lineView);
            lineView.LeftAnchor.ConstraintEqualTo(tableViewBackgroundView.LeftAnchor, constant: ResponseUpdateTableViewCell.CellPadding + (ResponseUpdateTableViewCell.DotSize / 2)).Active = true;
            lineView.TopAnchor.ConstraintEqualTo(tableViewBackgroundView.TopAnchor).Active = true;
            lineView.BottomAnchor.ConstraintEqualTo(tableViewBackgroundView.BottomAnchor).Active = true;
            lineView.WidthAnchor.ConstraintEqualTo(ResponseUpdateTableViewCell.LineWidth).Active = true;

            mapPanGestureRecognizer = new UIPanGestureRecognizer(HandleMapPan)
            {
                ShouldRecognizeSimultaneously = (g1, g2) => true,
            };

            iconTapGestureRecognizer = new UITapGestureRecognizer(HandleIconAction);
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            ViewModel.OnLocationChanged += HandleOnLocationChanged;
            responseUpdatesTableViewSource.OnTableViewScrolled += HandleOnTableViewScrolled;
            mapView.AddGestureRecognizer(mapPanGestureRecognizer);
            iconBackgroundView.AddGestureRecognizer(iconTapGestureRecognizer);
            ViewModel.Notifications.CollectionChanged += HandleNotificationsCollectionChanged;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            ViewModel.OnLocationChanged -= HandleOnLocationChanged;
            responseUpdatesTableViewSource.OnTableViewScrolled -= HandleOnTableViewScrolled;
            mapView.RemoveGestureRecognizer(mapPanGestureRecognizer);
            iconBackgroundView.RemoveGestureRecognizer(iconTapGestureRecognizer);
            responseUpdatesTableViewSource.Notifications = null;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            UpdateMapView();
            SetIconBackgroundViewFrame(tableView.ContentOffset.Y);
            UIView.Animate(PlatformConstants.AnimationDuration, () => 
            {
                mapView.Alpha = 1;
                iconBackgroundView.Alpha = 1;
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            View.LayoutIfNeeded();
            tableView.ContentInset = new UIEdgeInsets((View.Bounds.Height * mapViewHeightFactor) + headerHeight, 0, Constants.PulloutMinBottomMargin + Constants.Padding, 0);
            iconBackgroundView.Frame = new CGRect
            {
                Location = new CGPoint(View.Bounds.Width - Constants.Padding - iconSize, tableHeaderView.Frame.Top + (iconSize / 2)),
                Size = iconBackgroundView.Frame.Size,
            };
            tableView.SetContentOffset(new CGPoint(0, -(View.Bounds.Height * mapViewHeightFactor) + -headerHeight), false);
            tableHeaderView.Frame = new CGRect(0, tableView.Frame.Top + tableView.ContentInset.Top - headerHeight, View.Bounds.Width, headerHeight);
        }

        void HandleOnLocationChanged(object sender, Common.Geo.LocationChangedEventArgs e)
        {
            if (ViewModel?.Response?.Geolocation != null)
            {
                eventCoordinate = new CLLocationCoordinate2D(ViewModel.Response.Geolocation.Latitude, ViewModel.Response.Geolocation.Longitude);
            }

            lastUserCoordinate = new CLLocationCoordinate2D(e.CurrentLocation.Latitude, e.CurrentLocation.Longitude);
            UpdateMapView(true);
        }

        void UpdateMapView(bool animated = false)
        {
            if (CoordinateIsEmpty(eventCoordinate) && !isUserControllingMapRegion)
            {
                mapView.SetCenterCoordinate(lastUserCoordinate, false);
            }
            else if (!isUserControllingMapRegion)
            {
                SetMapRegion(animated);
            }

            if (isInitialRender && !CoordinateIsEmpty(eventCoordinate))
            {
                var annotation = new MKPointAnnotation { Coordinate = eventCoordinate };
                mapView.AddAnnotation(annotation);
                isInitialRender = false;
            }
        }

        void SetMapRegion(bool animated = false) 
        {
            var deltaPaddingFactor = 1.1;
            var latitudeDelta = Math.Abs(lastUserCoordinate.Latitude - eventCoordinate.Latitude);
            var longitudeDelta = Math.Abs(lastUserCoordinate.Longitude - eventCoordinate.Longitude);
            var spanRegion = new MKCoordinateRegion(lastUserCoordinate.GetMidpointCoordinate(eventCoordinate), new MKCoordinateSpan(latitudeDelta * deltaPaddingFactor, longitudeDelta * deltaPaddingFactor));
            var region = mapView.RegionThatFits(spanRegion);
            mapView.SetRegion(region, false);
        }

        void HandleMapPan()
        {
            isUserControllingMapRegion = true;
        }

        void HandleIconAction()
        {
            isUserControllingMapRegion = false;
            SetMapRegion(true);
        }

        void HandleNotificationsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            responseUpdatesTableViewSource.Notifications = new List<NotificationModel>(ViewModel.Notifications);
            tableView.ReloadData();
        }

        void HandleOnTableViewScrolled(object sender, TableViewScrolledEventArgs e)
        {
            var yOffset = e.ContentOffset.Y;

            tableHeaderView.Frame = new CGRect
            {
                Size = tableHeaderView.Frame.Size,
                Location = new CGPoint
                {
                    X = 0,
                    Y = tableView.Frame.Top - yOffset - headerHeight,
                },
            };

            SetIconBackgroundViewFrame(yOffset);

            tableViewBackgroundViewTopConstraint.Constant =  - yOffset - headerHeight;

            if (yOffset < 0)
            {
                var constant = (-yOffset - tableView.ContentInset.Top) * 0.5f;
                mapViewTopConstraint.Constant = constant - mapOverflowConstant;
            }

            View.LayoutIfNeeded();
        }

        void SetIconBackgroundViewFrame(nfloat yOffset) // TODO: this, and call it from viewdidappear or something
        {
            var iconY = -yOffset + tableView.Frame.Top - headerHeight - (iconBackgroundView.Frame.Height / 2);

            iconBackgroundView.Frame = new CGRect
            {
                Size = iconBackgroundView.Frame.Size,
                Location = new CGPoint
                {
                    X = iconBackgroundView.Frame.X,
                    Y = iconY,
                },
            };
        }

        bool CoordinateIsEmpty(CLLocationCoordinate2D coordinate) => Math.Abs(coordinate.Latitude) < double.Epsilon && Math.Abs(coordinate.Longitude) < double.Epsilon;
    }
}
