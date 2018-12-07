using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Java.Lang;
using Android.Content.Res;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.Content;

using Edison.Mobile.Android.Common;

namespace Edison.Mobile.User.Client.Droid.Adapters
{




    public class NavMenuExpandableListAdapter : BaseExpandableListAdapter
    {

        private Activity _context;

        // header titles
        public List<string> ListDataGroupText { get; set; } = new List<string>();
        // Combined header titles and icons
        public List<TextImageResourcePair> ListDataGroups { get; set; }  = null;



        // child data in format of group title, child title
        public Dictionary<string, List<string>> ListDataItemText {get; set; } = new Dictionary<string, List<string>>();
        // child data in format of group title, combined child title & icon
        public Dictionary<string, List<TextImageResourcePair>> ListDataItems {get; set; } = null;


        public NavMenuExpandableListAdapter(Activity context)

        {
            this._context = context;
        }

        public NavMenuExpandableListAdapter(Activity context, List<string> listDataGroupText, Dictionary<string, List<string>> listDataItemText)
        {
            this._context = context;
            ListDataGroupText = listDataGroupText;
            ListDataItemText = listDataItemText;
            ListDataGroups = null;
            ListDataItems = null;
        }


        public NavMenuExpandableListAdapter(Activity context, List<TextImageResourcePair> listDataGroups, Dictionary<string, List<TextImageResourcePair>> listDataItems)
        {
            this._context = context;
            ListDataGroupText = null;
            ListDataItemText = null;
            ListDataGroups = listDataGroups;
            ListDataItems = listDataItems;
        }



        public override int GroupCount => ListDataGroups?.Count ?? ListDataGroupText?.Count ?? 0;



        public override bool HasStableIds => false;


        // Group Headers

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            if (ListDataGroups == null)
                return ListDataGroupText[groupPosition];
            else
                return ListDataGroups[groupPosition].Text;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            // Get text label and icon resource for the group
            string groupText = null;
            int groupIconResource = 0;
            if (ListDataGroups == null)
                groupText = ListDataGroupText[groupPosition];
            else
            {
                groupText = ListDataGroups[groupPosition].Text;
                groupIconResource = ListDataGroups[groupPosition].ImageResource;
            }

            convertView = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.nav_list_group, null);

            // get the TextView for the label and assign the text
            TextView listGroupTextView = convertView.FindViewById<TextView>(Resource.Id.navlistgroup_text);
            listGroupTextView.Text = groupText;

            // If there is a resource
            if (groupIconResource != 0)
            {
                // get the ImageView for the icon and assign the image resource
                ImageView listGroupImageView = convertView.FindViewById<ImageView>(Resource.Id.navlistgroup_image);
                listGroupImageView.SetImageResource(groupIconResource);
//                var d = _context.Resources.GetDrawable(groupIconResource, null);
//                DrawableCompat.SetTint(d, Color.White);
//                listGroupImageView.SetImageDrawable(d);

            }

            // Indicator
            try
            {
                ImageView indicator = convertView.FindViewById<ImageView>(Resource.Id.navlistgroup_indicator);
                if (indicator != null)
                {
                    if (GetChildrenCount(groupPosition) == 0)
                        indicator.Visibility = ViewStates.Gone;
                    else
                    {
                        indicator.Visibility = ViewStates.Visible;
                        if (isExpanded)
                            indicator.Rotation = 90f;
                        else
                            indicator.Rotation = 0f;
                    }
                }
 //               indicator.Visibility = GetChildrenCount(groupPosition) > 0 ? ViewStates.Visible : ViewStates.Gone;
            }
            catch { }

            // return the view for the group
            return convertView;
        }


        // Items
        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            if (ListDataItems == null)
                return ListDataItemText[ListDataGroupText[groupPosition]][childPosition];
            else
                return ListDataItems[ListDataGroups[groupPosition].Text][childPosition]?.Text;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            if (ListDataItems == null)
                return ListDataItemText[ListDataGroupText[groupPosition]]?.Count ?? 0;
            else
                return ListDataItems[ListDataGroups[groupPosition].Text]?.Count ?? 0;
        }


        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            // Get text label and icon resource for the item
            string childText = null;
            int childIconResource = 0;
            if (ListDataItems == null)
                childText = ListDataItemText[ListDataGroupText[groupPosition]][childPosition];
            else
            {
                childText = ListDataItems[ListDataGroups[groupPosition].Text][childPosition].Text;
                childIconResource = ListDataItems[ListDataGroups[groupPosition].Text][childPosition].ImageResource;
            }


            convertView = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.nav_list_item, null);

            // get the TextView for the label and assign the text
            TextView listItemTextView = convertView.FindViewById<TextView>(Resource.Id.navlistitem_text);
            listItemTextView.Text = childText;

            // If there is a resource
            if (childIconResource != 0)
            {
                // get the ImageView for the icon and assign the image resource
                ImageView listItemImageView = convertView.FindViewById<ImageView>(Resource.Id.navlistitem_image);
                listItemImageView.SetImageResource(childIconResource);
     //           var d = _context.Resources.GetDrawable(childIconResource, null);
     //           DrawableCompat.SetTint(d, Color.White);
     //           listItemImageView.SetImageDrawable(d);
            }

            // return the view for the item
            return convertView;
        }


        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}