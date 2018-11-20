using System;
using CoreGraphics;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.DataSources;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class PulloutNeutralView : UIView
    {
        readonly UICollectionView collectionView;

        public ChatViewModel ChatViewModel;

        public PulloutNeutralView(ChatViewModel chatViewModel)
        {
            ChatViewModel = chatViewModel;

            var cellWidth = UIScreen.MainScreen.Bounds.Width / 3;
            var collectionViewLayout = new UICollectionViewFlowLayout
            {
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                ItemSize = new CGSize
                {
                    Width = cellWidth,
                    Height = Shared.Constants.PulloutBottomMargin,
                },
                MinimumInteritemSpacing = 0,
                MinimumLineSpacing = 0,
            };

            var collectionViewDataSource = new PulloutMinimizedCollectionViewDataSource(chatViewModel.ChatPromptTypes);

            collectionView = new UICollectionView(CGRect.Empty, collectionViewLayout)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                DataSource = collectionViewDataSource,
                AlwaysBounceHorizontal = true,
                BackgroundColor = Shared.Constants.Color.White,
            };

            collectionView.RegisterClassForCell(typeof(PulloutLargeButtonCollectionViewCell), typeof(PulloutLargeButtonCollectionViewCell).Name);

            AddSubview(collectionView);
            collectionView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            collectionView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            collectionView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            collectionView.HeightAnchor.ConstraintEqualTo(Shared.Constants.PulloutBottomMargin).Active = true;


            ChatViewModel.ChatPromptTypes.CollectionChanged += HandleChatPromptTypesCollectionChanged;
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

        void HandleChatPromptTypesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0) 
            {
                collectionView.InsertItems(new NSIndexPath[] { NSIndexPath.FromItemSection(e.NewStartingIndex, 0) });
            }
            else 
            {
                collectionView.ReloadData();
            }
        }

        public override void MovedToWindow()
        {
            if (Window == null) 
            {
                ChatViewModel.ChatPromptTypes.CollectionChanged -= HandleChatPromptTypesCollectionChanged;
                ChatViewModel = null;
            }
        }
    }
}
