using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Edison.Mobile.User.Client.Core.Shared;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class PulloutMinimizedCollectionViewSource : UICollectionViewSource
    {
        readonly ObservableRangeCollection<ChatPromptType> chatPromptTypes;
        readonly WeakReference<ChatViewModel> weakChatViewModel;

        public WeakReference<UICollectionView> WeakCollectionView { get; set; }

        public event EventHandler<ChatPromptEventArgs> OnChatPromptSelected;

        public PulloutMinimizedCollectionViewSource(ChatViewModel chatViewModel) 
        {
            chatPromptTypes = chatViewModel.ChatPromptTypes;
            weakChatViewModel = new WeakReference<ChatViewModel>(chatViewModel);
            chatViewModel.OnIsSafeChanged += HandleOnIsSafeChanged;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(typeof(PulloutLargeButtonCollectionViewCell).Name, indexPath) as PulloutLargeButtonCollectionViewCell;

            var chatPromptType = chatPromptTypes[(int)indexPath.Item];
            switch (chatPromptType)
            {
                case ChatPromptType.Emergency:
                    cell.Initialize(Shared.Constants.Color.LightGray, Shared.Constants.Assets.EmergencyRed, "Emergency");
                    break;
                case ChatPromptType.ReportActivity:
                    cell.Initialize(Shared.Constants.Color.LightGray, Shared.Constants.Assets.ChatBlue, "Report Activity");
                    break;
                case ChatPromptType.SafetyCheck:
                    cell.Initialize(Shared.Constants.Color.LightGray, Shared.Constants.Assets.PersonBlue, "I'm Safe");
                    break;
            }

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return chatPromptTypes.Count;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var chatPromptType = chatPromptTypes[(int)indexPath.Item];
            OnChatPromptSelected?.Invoke(this, new ChatPromptEventArgs
            {
                ChatPromptType = chatPromptType,
            });
        }

        void HandleOnIsSafeChanged(object sender, bool isSafe) 
        {
            if (chatPromptTypes.Count < 3) return;

            WeakCollectionView.TryGetTarget(out var collectionView);
            var cell = collectionView.CellForItem(NSIndexPath.FromItemSection(2, 0)) as PulloutLargeButtonCollectionViewCell;
            cell.Initialize(
                isSafe ? Shared.Constants.Color.Green : Shared.Constants.Color.BackgroundGray, 
                isSafe ? Shared.Constants.Assets.PersonCheckWhite : Shared.Constants.Assets.PersonBlue, 
                "I'm Safe"
            );
        }

        public void UnbindEventHandlers() 
        {
            weakChatViewModel.TryGetTarget(out var viewModel);
            viewModel.OnIsSafeChanged -= HandleOnIsSafeChanged;
        }
    }
}
