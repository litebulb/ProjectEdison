using System.Collections.Generic;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.ViewModels;
using System;
using Edison.Mobile.Android.Common.Controls;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Content.Res;
using Edison.Mobile.User.Client.Core.Shared;

namespace Edison.Mobile.User.Client.Droid.Fragments
{
    public class ChatFragment : BaseFragment<ChatViewModel>
    {

        private LinearLayout _quick_chat_holder;
        private List<CircularImageButton> _imageButtons = new List<CircularImageButton>();

        private bool _safeButtonSelected = false;
        private Color _origSafeButtonColor;
        private Color _selectedSafeButtonColor;
        private Color _origSafeIconColor;
        private Color _selectedSafeIconColor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.chat_fragment, container, false);


            _quick_chat_holder = root.FindViewById<LinearLayout>(Resource.Id.quick_chat_holder);
            _imageButtons.Add(root.FindViewById<CircularImageButton>(Resource.Id.qc_emergency));
            _imageButtons.Add(root.FindViewById<CircularImageButton>(Resource.Id.qc_activity));
            _imageButtons.Add(root.FindViewById<CircularImageButton>(Resource.Id.qc_safe));
            foreach (var button in _imageButtons)
            {
                button.Click += OnButtonClick;
            }
            _origSafeButtonColor = new Color(ContextCompat.GetColor(Context, Resource.Color.icon_background_grey));
            _selectedSafeButtonColor = new Color(ContextCompat.GetColor(Context, Resource.Color.app_green));
            _origSafeIconColor = new Color(ContextCompat.GetColor(Context, Resource.Color.icon_blue));
            _selectedSafeIconColor = new Color(ContextCompat.GetColor(Context, Resource.Color.white));

            return root;
        }



        private async void OnButtonClick(object sender, EventArgs e)
        {
            if (sender is CircularImageButton imgButton && Activity != null)
            {

                ChatPromptType cpt = ChatPromptType.ReportActivity;
                switch ((string)imgButton.Tag)
                {
                    case "qc_safe":
                        // Change the color of the button
                        _safeButtonSelected = !_safeButtonSelected;
                        imgButton.SetBackgroundTint(_safeButtonSelected ? _selectedSafeButtonColor: _origSafeButtonColor);
                        imgButton.SetIconResource(_safeButtonSelected ? Resource.Drawable.personal_check : Resource.Drawable.user);
                        var iconCsl = ColorStateList.ValueOf(_safeButtonSelected ? _selectedSafeIconColor: _origSafeIconColor);
                        imgButton.IconTint = iconCsl;
                        cpt = ChatPromptType.SafetyCheck;
                        break;
                }




                Toast.MakeText(Activity, (string)imgButton.Tag, ToastLength.Short).Show();

                await ViewModel.ActivateChatPrompt(cpt);

            }

        }



    }
}