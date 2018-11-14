using System;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class ChatMessageTypeCollectionViewSource : UICollectionViewSource
    {
        readonly ChatMessageType[] messageTypes;

        public ChatMessageTypeCollectionViewSource()
        {
            messageTypes = new ChatMessageType[]
            {
                new ChatMessageType 
                {
                    Title = "EMERGENCY",
                    IconImage = Constants.Assets.EmergencyRed,
                    SelectedIconImage = Constants.Assets.EmergencyWhite,
                    SelectionColor = Constants.Color.Red,
                },
                new ChatMessageType
                {
                    Title = "FIRE",
                    IconImage = Constants.Assets.FireRed,
                    SelectedIconImage = Constants.Assets.FireWhite,
                    SelectionColor = Constants.Color.Red,
                },
                new ChatMessageType 
                {
                    Title = "SHOOTER",
                    IconImage = Constants.Assets.GunRed,
                    SelectedIconImage = Constants.Assets.GunWhite,
                    SelectionColor = Constants.Color.Red,
                },
                new ChatMessageType
                {
                    Title = "SUS. PACKAGE",
                    IconImage = Constants.Assets.SpamYellow,
                    SelectedIconImage = Constants.Assets.SpamWhite,
                    SelectionColor = Constants.Color.YellowWarning,
                },
                new ChatMessageType 
                {
                    Title = "SAFETY",
                    IconImage = Constants.Assets.SafetyCheckBlue,
                    SelectedIconImage = Constants.Assets.SafetyCheckWhite,
                    SelectionColor = Constants.Color.Blue,
                },
                new ChatMessageType 
                {
                    Title = "WELL-BEING",
                    IconImage = Constants.Assets.HealthBlue,
                    SelectedIconImage = Constants.Assets.HealthWhite,
                    SelectionColor = Constants.Color.Blue,
                },
            };
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(typeof(ChatMessageTypeCollectionViewCell).Name, indexPath) as ChatMessageTypeCollectionViewCell;
            var messageType = messageTypes[(int)indexPath.Item];
            cell.Initialize(Constants.ChatMessageTypeHeight, messageType);
            return cell;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return messageTypes.Length;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //var cell = 
        }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath) as ChatMessageTypeCollectionViewCell;
            cell.SetSelected(true);
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath) as ChatMessageTypeCollectionViewCell;
            cell.SetSelected(false);
        }
    }
}
