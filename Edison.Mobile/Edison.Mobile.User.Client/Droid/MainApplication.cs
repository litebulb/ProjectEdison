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
using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Droid.Ioc;
using Edison.Mobile.User.Client.Core.Ioc;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;

namespace Edison.Mobile.User.Client.Droid
{
#if DEBUG
	[Application(Debuggable = true)]
#else
	[Application(Debuggable = false)]
#endif
	public partial class MainApplication : BaseApplication
	{
		bool FirstActivityCreated = false;

		public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{

		}

		public override void OnCreate()
		{
			base.OnCreate();
			Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(), new PlatformContainerRegistrar());
		}



		public override void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			base.OnActivityCreated(activity, savedInstanceState);
/*            if (!FirstActivityCreated)
			{
				Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(), new PlatformContainerRegistrar());
				FirstActivityCreated = true;
			}
			else
			{
				Container.Reregister<Activity>(new PlatformCommonContainerRegistrar(activity), activity);
			} */

		}


		public override void OnActivityResumed(Activity activity)
		{
			base.OnActivityResumed(activity);

		}


		public override void OnActivityStarted(Activity activity)
		{
			base.OnActivityStarted(activity);

		}


	}
}