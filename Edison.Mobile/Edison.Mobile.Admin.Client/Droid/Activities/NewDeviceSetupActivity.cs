using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.Droid.Toolbars;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using System;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/ic_edison_launcher")]
    public class NewDeviceSetupActivity : BaseActivity<ChooseDeviceTypeViewModel>
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.new_device_setup);

            BindResources();
            BindVMEvents();
        }

        private void BindResources()
        {
            var toolbar = FindViewById<CenteredToolbar>(Resource.Id.toolbar_new_device_setup);                        
            toolbar.SetTitle(Resource.String.setup_new_device_label);

            var layout = FindViewById<LinearLayout>(Resource.Id.instruction);

            var instructionNumber = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_number);
            var instructionText = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_text);

            instructionNumber.Text = "1";
            instructionText.SetText(Resource.String.what_type_of_device_label);
            
            var secondLayout = FindViewById<LinearLayout>(Resource.Id.second_instruction);

            var secondInstructionNumber = secondLayout.FindViewById<AppCompatTextView>(Resource.Id.instruction_number);
            var secondInstructionText = secondLayout.FindViewById<AppCompatTextView>(Resource.Id.instruction_text);

            secondInstructionNumber.Text = "2";
            secondInstructionText.SetText(Resource.String.turn_on_device_power_label);

            SetSupportActionBar(toolbar);
            this.BackPressed += NewDeviceSetupActivity_BackPressed;

            AppCompatButton button = FindViewById<AppCompatButton>(Resource.Id.new_device_setup_next_button);
            button.Click += Button_Click;

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var radioGroup = FindViewById<RadioGroup>(Resource.Id.sensorTypeRadioGroup);

            Core.Shared.DeviceType deviceType = Core.Shared.DeviceType.Button;
            switch (radioGroup.CheckedRadioButtonId)
            {
                case Resource.Id.buttonRadioButton:
                default:
                    deviceType = Core.Shared.DeviceType.Button;
                    break;
                case Resource.Id.soundSensorRadioButton:
                    deviceType = Core.Shared.DeviceType.SoundSensor;
                    break;
                case Resource.Id.lightRadioButton:
                    deviceType = Core.Shared.DeviceType.Light;
                    break;
            }

            this.ViewModel.SetDeviceType(deviceType);

            var intent = new Intent(this, typeof(NewDeviceScanActivity));
            intent.AddFlags(ActivityFlags.NoAnimation);
            StartActivity(intent);
        }

        private void NewDeviceSetupActivity_BackPressed(object sender, EventArgs e)
        {
            Finish();
        }

        protected void BindVMEvents()
        {
        }

        private async void OnManageButtonClick(object sender, EventArgs e)
        {
            //ClearMessages();

        }

        private async void OnSetupNewButtonClick(object sender, EventArgs e)
        {
            //ClearMessages();
            
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}

