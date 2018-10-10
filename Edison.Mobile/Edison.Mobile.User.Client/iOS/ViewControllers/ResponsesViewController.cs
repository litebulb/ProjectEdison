using System;
using System.Collections.Specialized;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.DataSources;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class ResponsesViewController : BaseViewController<ResponsesViewModel>
    {
        readonly nfloat collectionViewVerticalMargin = Constants.Padding;
        readonly float alertCircleDisabledAlpha = 0.5f;

        AlertsCircleView alertsCircleView;
        UICollectionView collectionView;
        ResponsesCollectionViewSource responsesCollectionViewSource;
        UILabel noAlertsLabel;

        public event EventHandler OnMenuTapped;

        public bool IsShowingDetails { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = PlatformConstants.Color.BackgroundGray;

            Title = "Right Now";

            NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            NavigationController.NavigationBar.Translucent = true;
            NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.Menu, UIBarButtonItemStyle.Plain, InternalOnMenuTapped);
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(Constants.Assets.Brightness, UIBarButtonItemStyle.Plain, OnBrightnessTapped);
            NavigationController.NavigationBar.TintColor = PlatformConstants.Color.Blue;


            alertsCircleView = new AlertsCircleView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                InnerCircleBackgroundColor = PlatformConstants.Color.Blue,
                Alpha = alertCircleDisabledAlpha,
            };

            View.AddSubview(alertsCircleView);

            alertsCircleView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            alertsCircleView.TopAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.TopAnchor, constant: Constants.Padding).Active = true;
            alertsCircleView.WidthAnchor.ConstraintEqualTo(View.Bounds.Height * .20f).Active = true;
            alertsCircleView.HeightAnchor.ConstraintEqualTo(alertsCircleView.WidthAnchor).Active = true;
            alertsCircleView.AlertCount = ViewModel.Responses.Count;

            noAlertsLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "Alert details will display here when active",
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Twelve),
                TextAlignment = UITextAlignment.Center,
                TextColor = PlatformConstants.Color.DarkGray,
            };

            View.AddSubview(noAlertsLabel);
            noAlertsLabel.TopAnchor.ConstraintEqualTo(alertsCircleView.BottomAnchor, constant: 40).Active = true;
            noAlertsLabel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            IsShowingDetails = false;
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            ViewModel.Responses.CollectionChanged += OnResponsesCollectionChanged;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            ViewModel.Responses.CollectionChanged -= OnResponsesCollectionChanged;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (collectionView == null)
            {
                responsesCollectionViewSource = new ResponsesCollectionViewSource(ViewModel.Responses);

                responsesCollectionViewSource.OnResponseSelected += OnResponseSelected;

                var collectionViewFrameTop = alertsCircleView.Frame.Bottom + collectionViewVerticalMargin;
                var collectionViewFrameBottom = View.Bounds.Bottom - Constants.PulloutBottomMargin - collectionViewVerticalMargin;
                var cellHeight = collectionViewFrameBottom - collectionViewFrameTop - (Constants.Padding * 2);
                var cellWidth = cellHeight * 1.111111111f;
                var screenWidth = View.Bounds.Width;
                var leftInset = (screenWidth / 2) - (cellWidth / 2);

                collectionView = new UICollectionView(CGRect.Empty, new UICollectionViewFlowLayout
                {
                    ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                    MinimumLineSpacing = Constants.Padding,
                    MinimumInteritemSpacing = Constants.Padding,
                    ItemSize = new CGSize(cellWidth, cellHeight),
                })
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = UIColor.Clear,
                    ContentInset = new UIEdgeInsets(0, leftInset, 0, leftInset),
                    Source = responsesCollectionViewSource,
                    PrefetchingEnabled = true,
                    ShowsHorizontalScrollIndicator = false,
                    AlwaysBounceHorizontal = true,
                };

                collectionView.RegisterClassForCell(typeof(ResponseCollectionViewCell), typeof(ResponseCollectionViewCell).Name);

                View.AddSubview(collectionView);

                collectionView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
                collectionView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
                collectionView.TopAnchor.ConstraintEqualTo(alertsCircleView.BottomAnchor, constant: collectionViewVerticalMargin).Active = true;
                collectionView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, constant: (-Constants.PulloutBottomMargin - collectionViewVerticalMargin)).Active = true;
            }
        }

        void InternalOnMenuTapped(object sender, EventArgs e)
        {
            OnMenuTapped?.Invoke(sender, e);
        }

        void OnBrightnessTapped(object sender, EventArgs e)
        {

        }

        void OnReceivedResponses()
        {
            if (ViewModel.Responses == null) return;

            ViewModel.Responses.CollectionChanged -= OnResponsesCollectionChanged;
            ViewModel.Responses.CollectionChanged += OnResponsesCollectionChanged;
        }

        void OnResponsesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var anyResponses = ViewModel.Responses.Count > 0;
            noAlertsLabel.Alpha = anyResponses ? 0 : 1;
            alertsCircleView.Alpha = anyResponses ? 1 : alertCircleDisabledAlpha;

            alertsCircleView.AlertCount = ViewModel.Responses.Count;
            collectionView.ReloadData();

            // TODO: more detailed insertion/deletion of cells, rather than a blanket rerender
            //collectionView.PerformBatchUpdates(() =>
            //{
            //    if (e.OldItems.Count > 0) 
            //    {

            //    }

            //    if (e.NewItems.Count > 0)
            //    {

            //    }

            //}, null);
        }

        void OnResponseSelected(object sender, int index)
        {
            IsShowingDetails = true;

            var response = ViewModel.Responses[index];
            var viewController = new ResponseDetailsViewController(response);
            NavigationController.PushViewController(viewController, false);
            //PresentViewController(new UINavigationController(viewController), true, null);
        }
    }
}
