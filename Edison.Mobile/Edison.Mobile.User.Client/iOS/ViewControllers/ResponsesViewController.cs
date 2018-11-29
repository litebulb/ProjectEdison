using System;
using System.Collections.Specialized;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.DataSources;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class ResponsesViewController : BaseViewController<ResponsesViewModel>
    {
        readonly nfloat collectionViewVerticalMargin = Constants.Padding;
        readonly float alertCircleDisabledAlpha = 0.5f;

        bool isInitialAppearance = true;
        bool brightnessSliderVisible;

        AlertsCircleView alertsCircleView;
        UICollectionView collectionView;
        ResponsesCollectionViewSource responsesCollectionViewSource;
        UILabel noAlertsLabel;
        UISlider brightnessSlider;
        UIImageView moonImageView;

        UITapGestureRecognizer alertsTapGestureRecognizer;

        public event EventHandler OnMenuTapped;
        public event EventHandler OnViewResponseDetails;
        public event EventHandler OnDismissResponseDetails;

        public bool IsShowingDetails { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundGray;

            Title = "Right Now";

            NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            NavigationController.NavigationBar.Translucent = true;
            NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Constants.Assets.Menu, UIBarButtonItemStyle.Plain, HandleInternalOnMenuTapped);
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(Constants.Assets.Brightness, UIBarButtonItemStyle.Plain, HandleOnBrightnessTapped);
            NavigationController.NavigationBar.TintColor = Constants.Color.Blue;

            alertsCircleView = new AlertsCircleView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                InnerCircleBackgroundColor = Constants.Color.Blue,
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
                TextColor = Constants.Color.DarkGray,
            };

            View.AddSubview(noAlertsLabel);
            noAlertsLabel.TopAnchor.ConstraintEqualTo(alertsCircleView.BottomAnchor, constant: 40).Active = true;
            noAlertsLabel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

            brightnessSlider = new UISlider
            {
                MinValue = 0,
                MaxValue = 1,
                Center = View.Center,
                Alpha = 0,
                Value = (float)UIScreen.MainScreen.Brightness,
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            brightnessSlider.ValueChanged += HandleBrightnessSliderValueChanged;

            View.AddSubview(brightnessSlider);

            moonImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Constants.Assets.BrightnessMoon,
                Alpha = 0,
            };

            View.AddSubview(moonImageView);

            moonImageView.CenterXAnchor.ConstraintEqualTo(brightnessSlider.CenterXAnchor).Active = true;
            moonImageView.WidthAnchor.ConstraintEqualTo(20).Active = true;
            moonImageView.HeightAnchor.ConstraintEqualTo(moonImageView.WidthAnchor).Active = true;

            alertsTapGestureRecognizer = new UITapGestureRecognizer(HandleAlertViewTapped);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.NavigationBar.TintColor = Constants.Color.Blue;
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes { ForegroundColor = Constants.Color.Blue };

            if (!isInitialAppearance)
            {
                IsShowingDetails = false;
                OnDismissResponseDetails?.Invoke(this, new EventArgs());
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!isInitialAppearance)
            {
                if (alertsCircleView.AlertCount > 0) alertsCircleView.StartAnimating();
            }

            isInitialAppearance = false;
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            ViewModel.Responses.CollectionChanged += HandleOnResponsesCollectionChanged;
            alertsCircleView.AddGestureRecognizer(alertsTapGestureRecognizer);
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            ViewModel.Responses.CollectionChanged -= HandleOnResponsesCollectionChanged;
            alertsCircleView.RemoveGestureRecognizer(alertsTapGestureRecognizer);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (collectionView == null)
            {
                responsesCollectionViewSource = new ResponsesCollectionViewSource(ViewModel.Responses);

                responsesCollectionViewSource.OnResponseSelected += HandleOnResponseSelected;

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
                View.SetNeedsLayout();
            }

            if (collectionView?.Frame.Top > 0 && brightnessSlider.Transform == CGAffineTransform.MakeIdentity())
            {
                var sliderHeight = collectionView.Frame.Top - View.SafeAreaInsets.Top - (Constants.Padding * 2);

                brightnessSlider.WidthAnchor.ConstraintEqualTo(sliderHeight).Active = true;
                brightnessSlider.CenterXAnchor.ConstraintEqualTo(View.RightAnchor, constant: (-brightnessSlider.Bounds.Height / 2) - (View.Bounds.Width > 400 ? 15 : 10)).Active = true;
                brightnessSlider.CenterYAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, constant: sliderHeight / 2).Active = true;

                brightnessSlider.Transform = CGAffineTransform.MakeRotation(-(nfloat)Math.PI / 2);

                moonImageView.TopAnchor.ConstraintEqualTo(brightnessSlider.CenterYAnchor, constant: (sliderHeight / 2) + 8).Active = true;
            }
        }

        void HandleInternalOnMenuTapped(object sender, EventArgs e)
        {
            OnMenuTapped?.Invoke(sender, e);
        }

        void HandleOnBrightnessTapped(object sender, EventArgs e)
        {
            brightnessSliderVisible = !brightnessSliderVisible;
            UIView.AnimateNotify(PlatformConstants.AnimationDuration, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
            {
                var alpha = brightnessSliderVisible ? 1 : 0;
                brightnessSlider.Alpha = alpha;
                moonImageView.Alpha = alpha;
            }, null);
        }

        void HandleBrightnessSliderValueChanged(object sender, EventArgs e)
        {
            UIScreen.MainScreen.Brightness = brightnessSlider.Value;
        }

        void HandleOnReceivedResponses()
        {
            if (ViewModel.Responses == null) return;

            ViewModel.Responses.CollectionChanged -= HandleOnResponsesCollectionChanged;
            ViewModel.Responses.CollectionChanged += HandleOnResponsesCollectionChanged;
        }

        void HandleOnResponsesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var anyResponses = ViewModel.Responses.Count > 0;
            noAlertsLabel.Alpha = anyResponses ? 0 : 1;
            alertsCircleView.Alpha = anyResponses ? 1 : alertCircleDisabledAlpha;

            alertsCircleView.AlertCount = ViewModel.Responses.Count;

            if (alertsCircleView.AlertCount > 0) alertsCircleView.StartAnimating();

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

        void HandleAlertViewTapped(UITapGestureRecognizer gestureRecognizer)
        {
            Console.WriteLine(gestureRecognizer);
        }

        void HandleOnResponseSelected(object sender, int index)
        {
            IsShowingDetails = true;

            OnViewResponseDetails?.Invoke(this, new EventArgs());

            var response = ViewModel.Responses[index];
            var viewController = new ResponseDetailsViewController(response);
            NavigationController.PushViewController(viewController, true);
        }
    }
}
