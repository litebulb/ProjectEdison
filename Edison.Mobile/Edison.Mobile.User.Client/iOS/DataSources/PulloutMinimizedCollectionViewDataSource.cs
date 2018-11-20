using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Edison.Mobile.User.Client.Core.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class PulloutMinimizedCollectionViewDataSource : UICollectionViewSource
    {
        readonly ObservableRangeCollection<ChatPromptType> chatPromptTypes;

        public event EventHandler<ChatPromptEventArgs> OnChatPromptSelected;

        public PulloutMinimizedCollectionViewDataSource(ObservableRangeCollection<ChatPromptType> chatPromptTypes) 
        {
            this.chatPromptTypes = chatPromptTypes;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(typeof(PulloutLargeButtonCollectionViewCell).Name, indexPath) as PulloutLargeButtonCollectionViewCell;

            switch (indexPath.Item)
            {
                case 0:
                    cell.Initialize(Shared.Constants.Color.LightGray, Shared.Constants.Assets.EmergencyRed, "Emergency");
                    break;
                case 1:
                    cell.Initialize(Shared.Constants.Color.LightGray, Shared.Constants.Assets.ChatBlue, "Report Activity");
                    break;
                case 2:
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
    }
}
