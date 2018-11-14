using System;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.iOS.DataSources;
using Edison.Mobile.User.Client.iOS.Shared;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class PulloutNeutralView : UIView
    {
        readonly UICollectionView collectionView;

        public PulloutNeutralView()
        {
            var cellWidth = UIScreen.MainScreen.Bounds.Width / 3;
            var collectionViewLayout = new UICollectionViewFlowLayout
            {
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                ItemSize = new CGSize
                {
                    Width = cellWidth,
                    Height = Constants.PulloutBottomMargin,
                },
                MinimumInteritemSpacing = 0,
                MinimumLineSpacing = 0,
            };

            var collectionViewDataSource = new PulloutMinimizedCollectionViewDataSource(); // TODO: wire up item selected events

            collectionView = new UICollectionView(CGRect.Empty, collectionViewLayout)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                DataSource = collectionViewDataSource,
                AlwaysBounceHorizontal = true,
                BackgroundColor = Constants.Color.White,
            };

            collectionView.RegisterClassForCell(typeof(PulloutLargeButtonCollectionViewCell), typeof(PulloutLargeButtonCollectionViewCell).Name);

            AddSubview(collectionView);
            collectionView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            collectionView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            collectionView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            collectionView.HeightAnchor.ConstraintEqualTo(Constants.PulloutBottomMargin).Active = true;
        }

        public void SetPercentMinimized(nfloat minimizedPercent) 
        {
            var cells = collectionView.VisibleCells;
            foreach (var cell in cells) 
            {
                if (cell is PulloutLargeButtonCollectionViewCell buttonCell) 
                {
                    buttonCell.SetPercentMinimized(minimizedPercent);
                }
            }
        }
    }
}
