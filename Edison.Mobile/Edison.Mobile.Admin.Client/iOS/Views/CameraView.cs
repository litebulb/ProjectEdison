using System;
using AVFoundation;
using Edison.Mobile.Admin.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class CameraView : UIView
    {
        AVCaptureSession captureSession;
        AVCaptureVideoPreviewLayer previewLayer;

        public bool IsRunning => captureSession?.Running ?? false;

        public CameraView()
        {
            BackgroundColor = Constants.Color.DarkGray;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (previewLayer != null) 
            {
                previewLayer.Frame = Bounds;
            }
        }

        public void Start()
        {
            captureSession = new AVCaptureSession();
            previewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
            };

            try
            {
                var captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);
                var input = AVCaptureDeviceInput.FromDevice(captureDevice);

                captureSession.AddInput(input);

                Layer.AddSublayer(previewLayer);

                captureSession.StartRunning();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
