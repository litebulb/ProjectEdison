using System;
using System.Collections.ObjectModel;
using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.DataSources
{
    public class ActionPlanSelectedEventArgs : EventArgs 
    {
        public ActionPlanListModel SelectedActionPlan { get; set; }
    }

    public class ChatMessageTypeCollectionViewSource : UICollectionViewSource
    {
        public ObservableRangeCollection<ActionPlanListModel> ActionPlans;
        public ActionPlanListModel SelectedActionPlan;

        public event EventHandler<ActionPlanSelectedEventArgs> OnActionPlanSelected;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(typeof(ChatMessageTypeCollectionViewCell).Name, indexPath) as ChatMessageTypeCollectionViewCell;
            var actionPlan = ActionPlans[(int)indexPath.Item];
            cell.Initialize(Constants.ChatMessageTypeHeight, actionPlan);
            cell.SetSelected(actionPlan == SelectedActionPlan);
            return cell;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return ActionPlans?.Count ?? 0;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            SelectedActionPlan = ActionPlans[(int)indexPath.Item];

            var cell = collectionView.CellForItem(indexPath) as ChatMessageTypeCollectionViewCell;
            cell.SetSelected(true);

            foreach (var otherCell in collectionView.VisibleCells) 
            {
                if (otherCell is ChatMessageTypeCollectionViewCell chatCell)
                {
                    chatCell.SetSelected(otherCell == cell);
                }
            }

            OnActionPlanSelected?.Invoke(this, new ActionPlanSelectedEventArgs
            {
                SelectedActionPlan = SelectedActionPlan,
            });
        }
    }
}
