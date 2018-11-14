using System;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class PulloutMinimizedCollectionViewDataSource : UICollectionViewDataSource
    {
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(typeof(PulloutLargeButtonCollectionViewCell).Name, indexPath) as PulloutLargeButtonCollectionViewCell;

            switch (indexPath.Item)
            {
                case 0:
                    cell.Initialize(Constants.Color.LightGray, Constants.Assets.EmergencyRed, "Emergency");
                    break;
                case 1:
                    cell.Initialize(Constants.Color.LightGray, Constants.Assets.ChatBlue, "Report Activity");
                    break;
                case 2:
                    cell.Initialize(Constants.Color.LightGray, Constants.Assets.PersonBlue, "I'm Safe");
                    break;
            }

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return 3;
        }
    }
}
