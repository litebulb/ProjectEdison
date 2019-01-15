using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.Android.Common.Controls
{
    public class SimpleModalAlertDialogFragment : DialogFragment
    {

        public EventHandler<DialogClickEventArgs> DialogResponse;


        public string DialogTitle{ get; set; }

        public string DialogMessage { get; set; }

        public string DialogNegitiveText { get; set; }

        public string DialogNeutralText { get; set; }

        public string DialogPositiveText { get; set; }


        public SimpleModalAlertDialogFragment() { }


        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            if (!string.IsNullOrWhiteSpace(DialogTitle))
                builder.SetTitle(DialogTitle);
            if (!string.IsNullOrWhiteSpace(DialogMessage))
                builder.SetMessage(DialogMessage);
            if (!string.IsNullOrWhiteSpace(DialogPositiveText))
                builder.SetPositiveButton(DialogPositiveText, DialogResponse);
            if (!string.IsNullOrWhiteSpace(DialogNegitiveText))
                builder.SetNegativeButton(DialogNegitiveText, DialogResponse);
            if (!string.IsNullOrWhiteSpace(DialogNeutralText))
                builder.SetNegativeButton(DialogNeutralText, DialogResponse);
            if (string.IsNullOrWhiteSpace(DialogPositiveText) && string.IsNullOrWhiteSpace(DialogNegitiveText) && string.IsNullOrWhiteSpace(DialogNeutralText))
                builder.SetPositiveButton(Activity.Resources.GetString(global::Android.Resource.String.Ok), DialogResponse);

            return builder.Create();

        }






    }
}