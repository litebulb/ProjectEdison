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
        ActionPlanListModel selectedActionPlan;

        public ObservableRangeCollection<ActionPlanListModel> ActionPlans;

        public WeakReference<UICollectionView> CollectionView { get; set; }

        public ActionPlanListModel SelectedActionPlan
        {
            get => selectedActionPlan;
            set 
            {
                selectedActionPlan = value;
                var index = ActionPlans.IndexOf(selectedActionPlan);
                if (index > -1) 
                {
                    var indexPath = NSIndexPath.FromItemSection(index, 0);
                    CollectionView.TryGetTarget(out var collectionView);
                    InvokeOnMainThread(() => collectionView.ReloadSections(NSIndexSet.FromIndex(0)));
                }
            }
        }

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
            var actionPlan = ActionPlans[(int)indexPath.Item];

            if (SelectedActionPlan == actionPlan) return;

            SelectedActionPlan = actionPlan;

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
