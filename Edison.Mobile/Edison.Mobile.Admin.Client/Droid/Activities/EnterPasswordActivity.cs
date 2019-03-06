using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.Droid.Adapters;
using Edison.Mobile.Android.Common;
using System;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using TextTypes = Android.Text.InputTypes;
using Edison.Mobile.Admin.Client.Droid.Toolbars;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/EdisonLight.Base", WindowSoftInputMode =  SoftInput.StateAlwaysVisible|SoftInput.AdjustResize, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/ic_edison_launcher")]
    public class EnterPasswordActivity : BaseActivity<EnterWifiPasswordViewModel>
    {
        private string _ssid;
        private AppCompatEditText _editText;
        private AppCompatButton _connectButton;
        private AppCompatTextView _showPasswordTextView;

        public const string ShowPassword = "Show Password";
        public const string HidePassword = "Hide Password";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
                       
            _ssid = this.Intent.GetStringExtra("ssid");

            SetContentView(Resource.Layout.enter_password);

            BindResources();
            //BindVMEvents();
        }

        private void BindResources()
        {            
            var toolbar = FindViewById<CenteredToolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.edison_device_setup_message);

            var layout = FindViewById<LinearLayout>(Resource.Id.instruction);
            
            var instructionNumber = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_number);
            var instructionText = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_text);
            
            instructionNumber.Text = "5";
            instructionText.SetText(Resource.String.enter_password_instruction_label);
            
            TextView labelView = FindViewById<TextView>(Resource.Id.enter_password_for_network_ssid);
            labelView.Text = _ssid;

            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                        
            this.BackPressed += SelectWifiOnDeviceActivity_BackPressed;

            _editText = (AppCompatEditText)FindViewById(Resource.Id.wifiPasswordEditText);
            _editText.TextChanged += EditText_TextChanged;

            _connectButton = (AppCompatButton)FindViewById(Resource.Id.connect_button);
            _connectButton.Enabled = false;
            _connectButton.Click += _connectButton_Click;

            _showPasswordTextView = (AppCompatTextView)FindViewById(Resource.Id.show_password_textview);
            _showPasswordTextView.Click += _showPasswordTextView_Click;

        }

        private async void _connectButton_Click(object sender, EventArgs e)
        {
            if ((await ViewModel.ConnectDeviceToNetwork(_ssid, _editText.Text)))
            {
                var intent = new Intent(this, typeof(EnterLocationActivity));
                intent.AddFlags(ActivityFlags.NoAnimation);
                StartActivity(intent);
            }
            else
            {
                //failed
            }
        }

        private void _showPasswordTextView_Click(object sender, EventArgs e)
        {
            if(_showPasswordTextView.Text == ShowPassword)
            {
                _showPasswordTextView.Text = HidePassword;
                _editText.InputType = TextTypes.TextVariationVisiblePassword;
            }
            else
            {
                _showPasswordTextView.Text = ShowPassword;
                _editText.InputType = TextTypes.TextVariationPassword | TextTypes.ClassText;
            }
        }

        private void EditText_TextChanged(object sender, global::Android.Text.TextChangedEventArgs e)
        {
            if(e.Text.Count() > 5)
            {
                _connectButton.Enabled = true;
            }
            else
            {
                _connectButton.Enabled = false;
            }
        }

        private void SelectWifiOnDeviceActivity_BackPressed(object sender, EventArgs e)
        {
            Finish();
        }

        
    }
}

