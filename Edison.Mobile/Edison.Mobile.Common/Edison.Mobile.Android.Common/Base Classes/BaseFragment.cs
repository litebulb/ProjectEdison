using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Autofac;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.ViewModels;

using AppCompatFragment = global::Android.Support.V4.App.Fragment;

namespace Edison.Mobile.Android.Common
{
    public class BaseFragment<T> : AppCompatFragment where T : BaseViewModel
    {

        private T _viewModel;

        protected T ViewModel => _viewModel ?? (_viewModel = Container.Instance.Resolve<T>());


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override void OnDestroy()
        {

            base.OnDestroy();
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ViewModel?.ViewCreated();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            UnBindEventHandlers();
            ViewModel?.ViewDestroyed();
        }



        public override void OnStart()
        {
            base.OnStart();
            BindEventHandlers();
            ViewModel?.ViewAppearing();
        }

        public override void OnStop()
        {
            base.OnStart();
            UnBindEventHandlers();
            ViewModel?.ViewDisappeared();
        }

        public override void OnPause()
        {
            base.OnPause();
            UnBindEventHandlers();
            ViewModel?.ViewDisappearing();
        }

        // Note: There is no equivilent to ViewDidAppear in Android
        public override void OnResume()
        {
            base.OnResume();
            BindEventHandlers();
            ViewModel?.ViewAppeared();
        }


        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
        }
        public override void OnDetach()
        {
            base.OnDetach();
        }










        protected virtual void BindEventHandlers()
        {
            UnBindEventHandlers();
        }

        protected virtual void UnBindEventHandlers()
        {

        }

    }
}