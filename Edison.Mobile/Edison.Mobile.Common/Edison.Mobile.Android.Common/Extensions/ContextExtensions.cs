using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.Android.Common
{
    public static class ContextExtensions
    {
        public static int GetDrawableId(this Context ctx, string drawableName)
        {
            if (ctx == null || string.IsNullOrWhiteSpace(drawableName)) return 0;
            return ctx.Resources.GetIdentifier(drawableName, "drawable", ctx.PackageName);
        }

    }
}