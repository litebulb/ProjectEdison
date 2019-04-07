using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using static Android.Gms.Vision.Detector;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Droid.Toolbars;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_edison_launcher",ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class NewDeviceScanActivity : BaseActivity<RegisterDeviceViewModel>, ISurfaceHolderCallback, IProcessor    
    {
        SurfaceView surfaceView;
        BarcodeDetector barcodeDetector;
        CameraSource cameraSource;
        private AppCompatTextView statusLabel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.new_device_scan);

            BindResources();


            BindVMEvents();
        }

        private void BindResources()
        {
            var toolbar = FindViewById<CenteredToolbar>(Resource.Id.toolbar);                        
            toolbar.SetTitle(Resource.String.new_device_scan_title);

            var layout = FindViewById<LinearLayout>(Resource.Id.instruction);

            var instructionNumber = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_number);
            var instructionText = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_text);

            statusLabel = FindViewById<AppCompatTextView>(Resource.Id.status_label);

            instructionNumber.Text = "3";
            instructionText.SetText(Resource.String.scan_qr_message_label);            

            SetSupportActionBar(toolbar);
            this.BackPressed += NewDeviceSetupActivity_BackPressed;               
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            surfaceView = FindViewById<SurfaceView>(Resource.Id.cameraView);
            
            barcodeDetector = new BarcodeDetector.Builder(this)
                .SetBarcodeFormats(BarcodeFormat.QrCode)                
                .Build();
                        
            cameraSource = new CameraSource
                .Builder(this, barcodeDetector)
                .SetRequestedPreviewSize(640, 480)
                .Build();

            surfaceView.Holder.AddCallback(this);
            barcodeDetector.SetProcessor(this);

        }

        private void NewDeviceSetupActivity_BackPressed(object sender, EventArgs e)
        {
            Finish();
        }

        protected void BindVMEvents()
        {
            this.ViewModel.OnPairingStatusTextChanged += ViewModel_OnPairingStatusTextChanged;
            this.ViewModel.OnFinishDevicePairing += ViewModel_OnFinishDevicePairing;
        }

        private void ViewModel_OnFinishDevicePairing(object sender, RegisterDeviceViewModel.OnFinishDevicePairingEventArgs e)
        {
            if (!e.IsSuccess)
            {
                RunOnUiThread(() =>
                {
                    statusLabel.Text = $"{statusLabel.Text} - Error occurred";
                });

                Console.Out.WriteLine(e);
            }
            else
            {
                var intent = new Intent(this, typeof(SelectWifiOnDeviceActivity));
                intent.AddFlags(ActivityFlags.NoAnimation);
                StartActivity(intent);
            }
        }

        private void ViewModel_OnPairingStatusTextChanged(object sender, string e)
        {
            RunOnUiThread(() =>
            {
                statusLabel.Text = e;
            });            
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
        
        public void ReceiveDetections(Detections detections)
        {
            if (this.ViewModel.State == RegisterDeviceViewModel.RegistrationState.New)
            {
                SparseArray qrcodes = detections.DetectedItems;
                if (qrcodes.Size() != 0)
                {
                    try
                    {
                        Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
                        vibrator.Vibrate(VibrationEffect.CreateOneShot(1000, 1));
                    }
                    catch { }
                    var value = ((Barcode)qrcodes.ValueAt(0)).RawValue;

                    string networkSSID = value;

                    if (!string.IsNullOrEmpty(networkSSID))
                    {                        
                        Task.Run(async () => await this.ViewModel.ProvisionDevice(new Common.WiFi.WifiNetwork() { SSID = networkSSID }));
                    }
                }
            }
        }

        public void Release()
        {
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                cameraSource.Start(surfaceView.Holder);
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }
    }
}

