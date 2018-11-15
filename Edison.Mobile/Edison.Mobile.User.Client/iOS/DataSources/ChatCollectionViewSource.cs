using System;
using System.Collections.ObjectModel;
using Edison.Mobile.User.Client.Core.Chat;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class ChatCollectionViewSource : UICollectionViewSource
    {
        public ObservableRangeCollection<ChatMessage> Messages { get; set; }
        public string Initials { get; set; }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(typeof(ChatMessageCollectionViewCell).Name, indexPath) as ChatMessageCollectionViewCell;
            var message = Messages[(int)indexPath.Item];
            cell.Initialize(message, Initials);
            return cell;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Messages?.Count ?? 0;
        }
    }
}
