using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Views = Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace Edison.Mobile.Admin.Client.Droid.Dialogs
{
    public class SetupCompleteDialog : Dialog, Views.View.IOnClickListener
    {
        public event EventHandler GoToHome;
        public event EventHandler GoToManageDevices;

        public SetupCompleteDialog(Activity activity)
            :base(activity)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.setup_complete_dialog);

            ((AppCompatButton)FindViewById(Resource.Id.go_to_home_screen_button)).SetOnClickListener(this);
            ((AppCompatButton)FindViewById(Resource.Id.manage_this_device_button)).SetOnClickListener(this);

        }
               
        public void OnClick(Views.View v)
        {
            switch(v.Id)
            {
                case Resource.Id.go_to_home_screen_button:
                    GoToHome?.Invoke(this, new EventArgs());
                    break;
                case Resource.Id.manage_this_device_button:
                    GoToManageDevices?.Invoke(this, new EventArgs());
                    break;
                default:
                    Dismiss();
                    break;
            }

        }
    }
}