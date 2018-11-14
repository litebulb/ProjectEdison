using System;
using System.Collections.Generic;
using CoreGraphics;
using Edison.Mobile.User.Client.iOS.DataSources;
using Edison.Mobile.User.Client.iOS.Views;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Shared
{
    public class ChatCollectionViewLayout : UICollectionViewLayout
    {
        List<UICollectionViewLayoutAttributes> layoutCache;

        bool isPreparing;

        nfloat contentHeight = 0;
        nfloat contentWidth = UIScreen.MainScreen.Bounds.Width;

        public override CGSize CollectionViewContentSize => new CGSize(contentWidth, contentHeight);

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
        {
            return isPreparing ? null : layoutCache[(int)indexPath.Item];
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var visibleLayoutAttributes = new List<UICollectionViewLayoutAttributes>();

            foreach (var attributes in layoutCache) 
            {
                if (attributes.Frame.IntersectsWith(rect)) 
                {
                    visibleLayoutAttributes.Add(attributes);
                }
            }

            return visibleLayoutAttributes.ToArray();
        }

        public override void PrepareLayout()
        {
            isPreparing = true;
            layoutCache = new List<UICollectionViewLayoutAttributes>();
            var numberOfItems = CollectionView.NumberOfItemsInSection(0);

            nfloat currentY = 0;
            for (var i = 0; i < numberOfItems; i++)
            {
                var viewSource = CollectionView.Source as ChatCollectionViewSource;
                var indexPath = NSIndexPath.FromItemSection(i, 0);
                var cell = viewSource.GetCell(CollectionView, indexPath) as ChatMessageCollectionViewCell;
                var size = cell.ContentView.SystemLayoutSizeFittingSize(UIScreen.MainScreen.Bounds.Size);

                CGRect frame = new CGRect
                {
                    X = 0,
                    Y = currentY,
                    Width = size.Width,
                    Height = size.Height,
                };

                var attributes = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                attributes.Frame = frame;

                layoutCache.Add(attributes);

                currentY += frame.Height;
            }

            contentHeight = currentY;

            isPreparing = false;
        }
    }
}
