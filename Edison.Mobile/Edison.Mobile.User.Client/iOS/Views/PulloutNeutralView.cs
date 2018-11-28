using System;
using CoreGraphics;
using Edison.Mobile.User.Client.Core.Shared;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.DataSources;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class PulloutNeutralView : UIView
    {
        readonly UICollectionView collectionView;
        readonly PulloutMinimizedCollectionViewSource viewSource;

        public ChatViewModel ChatViewModel;

        public PulloutNeutralView(ChatViewModel chatViewModel)
        {
            ChatViewModel = chatViewModel;

            var cellWidth = UIScreen.MainScreen.Bounds.Width / 2;
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

            viewSource = new PulloutMinimizedCollectionViewSource(chatViewModel);

            viewSource.OnChatPromptSelected += HandleOnChatPromptSelected;

            collectionView = new UICollectionView(CGRect.Empty, collectionViewLayout)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Source = viewSource,
                AlwaysBounceHorizontal = true,
                BackgroundColor = Shared.Constants.Color.White,
            };

            viewSource.WeakCollectionView = new WeakReference<UICollectionView>(collectionView);

            collectionView.RegisterClassForCell(typeof(PulloutLargeButtonCollectionViewCell), typeof(PulloutLargeButtonCollectionViewCell).Name);

            AddSubview(collectionView);
            collectionView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            collectionView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            collectionView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            collectionView.HeightAnchor.ConstraintEqualTo(Shared.Constants.PulloutBottomMargin).Active = true;

            ChatViewModel.ChatPromptTypes.CollectionChanged += HandleChatPromptTypesCollectionChanged;
            ChatViewModel.OnChatPromptActivated += HandleOnChatPromptActivated;
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
            collectionView.PerformBatchUpdates(() =>
            {
                var safetyCheckAdded = e.NewItems != null && e.NewItems.Count > 0;
                if (safetyCheckAdded)
                {
                    collectionView.InsertItems(new NSIndexPath[] { NSIndexPath.FromItemSection(e.NewStartingIndex, 0) });
                }
                else
                {
                    collectionView.DeleteItems(new NSIndexPath[] { NSIndexPath.FromItemSection(2, 0) });
                }

                ((UICollectionViewFlowLayout)collectionView.CollectionViewLayout).ItemSize = new CGSize
                {
                    Height = Shared.Constants.PulloutBottomMargin,
                    Width = UIScreen.MainScreen.Bounds.Width / (safetyCheckAdded ? 3 : 2),
                };

                collectionView.CollectionViewLayout.InvalidateLayout();
            }, null);

        }

        async void HandleOnChatPromptSelected(object sender, ChatPromptEventArgs e)
        {
            await ChatViewModel.ActivateChatPrompt(e.ChatPromptType);
        }

        void HandleOnChatPromptActivated(object sender, ChatPromptType chatPromptType) 
        {
            if (chatPromptType == ChatPromptType.SafetyCheck) 
            {
                //ChatViewModel.IsSafe = !ChatViewModel.IsSafe;
            }
        }

        public override void MovedToWindow()
        {
            if (Window == null)
            {
                ChatViewModel.ChatPromptTypes.CollectionChanged -= HandleChatPromptTypesCollectionChanged;
                viewSource.OnChatPromptSelected -= HandleOnChatPromptSelected;
                viewSource.UnbindEventHandlers();
                ChatViewModel.OnChatPromptActivated -= HandleOnChatPromptActivated;
                ChatViewModel = null;
            }
        }
    }
}
