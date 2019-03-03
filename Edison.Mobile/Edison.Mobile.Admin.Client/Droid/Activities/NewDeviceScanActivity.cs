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
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }

        public async void ReceiveDetections(Detections detections)
        {
            SparseArray qrcodes = detections.DetectedItems;
            if (qrcodes.Size() != 0)
            {
                Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
                vibrator.Vibrate(VibrationEffect.CreateOneShot(1000, 1));
                var value = ((Barcode)qrcodes.ValueAt(0)).RawValue;

                string networkSSID = value;

                await this.ViewModel.ProvisionDevice(new Common.WiFi.WifiNetwork() { SSID = networkSSID });
                                
                var intent = new Intent(this, typeof(SelectWifiOnDeviceActivity));
                intent.AddFlags(ActivityFlags.NoAnimation);
                StartActivity(intent);
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

