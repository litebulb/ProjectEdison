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

namespace Edison.Mobile.Admin.Client.Droid.Dialogs
{
    public class SetupCompleteDialog : Dialog, Views.View.IOnClickListener
    {
        public SetupCompleteDialog(Activity activity)
            :base(activity)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.setup_complete_dialog);
        }

        public void OnClick(Views.View v)
        {
            this.OwnerActivity.Finish();
        }
    }
}