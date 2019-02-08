using System;
using AVFoundation;
using CoreFoundation;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Foundation;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class CameraView : UIView, IAVCaptureMetadataOutputObjectsDelegate
    {
        AVCaptureSession captureSession;
        AVCaptureVideoPreviewLayer previewLayer;

        public event EventHandler<string> OnQRCodeScanned;

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
                var output = new AVCaptureMetadataOutput();
                var queue = new DispatchQueue("qrQueue");

                captureSession.AddInput(input);
                captureSession.AddOutput(output);

                output.SetDelegate(this, queue);
                output.MetadataObjectTypes = AVMetadataObjectType.QRCode;

                Layer.AddSublayer(previewLayer);

                captureSession.StartRunning();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Export("captureOutput:didOutputMetadataObjects:fromConnection:")]
        public void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
        {
            if (metadataObjects.Length > 0 && metadataObjects[0] is AVMetadataMachineReadableCodeObject readableObject)
            {
                if (!string.IsNullOrWhiteSpace(readableObject.StringValue))
                {
                    OnQRCodeScanned?.Invoke(this, readableObject.StringValue);
                }
            }
        }

    }
}
