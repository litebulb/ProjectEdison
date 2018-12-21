using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Common;

namespace Edison.Mobile.Android.Common
{
    public class PermissionsService : IPermissionsService
    {


        public bool HasPermission(Permission permission)
        {
            string[] permissions = MapToAndroidPermissions(permission);
            var res = HasPermissions(permissions);
            return !res.Contains(false);
        }

        public async Task<bool> HasPermissionAsync(Permission permission)
        {
            string[] permissions = MapToAndroidPermissions(permission);
            var res = await HasPermissionsAsync(permissions);
            return !res.Contains(false);
        }

        public bool[] HasPermissions(Permission[] permissions)
        {
            bool[] results = new bool[permissions.Length];
            int i = 0;
            foreach (Permission permission in permissions)
            {
                results[i] = HasPermission(permission);
                i++;
            }
            return results;
        }

        public async Task<bool[]> HasPermissionsAsync(Permission[] permissions)
        {
            bool[] results = new bool[permissions.Length];
            int i = 0;
            foreach (Permission permission in permissions)
            {
                results[i] = await HasPermissionAsync(permission);
                i++;
            }
            return results;
        }

        private bool[] HasPermissions(string[] permissions)
        {
            bool[] result = new bool[permissions.Length];
            int i = 0;
            foreach (string permission in permissions)
            {
                result[i] = ContextCompat.CheckSelfPermission(Application.Context, permission) == global::Android.Content.PM.Permission.Granted;
                i++;
            }
            return result;
        }

        private async Task<bool[]> HasPermissionsAsync(string[] permissions)
        {
            return await Task.Run(() => {
                return HasPermissions(permissions);
            }).ConfigureAwait(false);
        }


        public bool[] RequestPermissions(Permission[] permissions, int requestId, object activity)
        {
            if (activity != null && activity is Activity act && permissions.Length > 0)
            {
                List<string> permissionsList = new List<string>();
                foreach (Permission permission in permissions)
                {
                    permissionsList.AddRange(MapToAndroidPermissions(permission));
                }
                RequestPermissions(permissions, requestId, activity);
            }
            else
                throw new Exception("object activity is not an instance of Activity");
            return null;  // Android permisson requests are handled asynchronously in the calling Activity by overriding OnRequestPermissionsResult
        }



        public async Task<bool[]> RequestPermissionsAsync(Permission[] permissions, int requestId, object activity)
        {
            if (activity != null && activity is Activity act && permissions.Length > 0)
            {
                List<string> permissionsList = new List<string>();
                foreach (Permission permission in permissions)
                {
                    permissionsList.AddRange(MapToAndroidPermissions(permission));
                }
                await RequestPermissionsAsync(permissions, requestId, activity);
            }
            else
                throw new Exception("object activity is not an instance of Activity");
            return null;  // Android permisson requests are handled asynchronously in the calling Activity by overriding OnRequestPermissionsResult
        }



        private void RequestPermissions(string[] permissions, int requestId, object activity)
        {
            if (activity != null && activity is Activity act)
                ActivityCompat.RequestPermissions(act, permissions, requestId);
            else
                throw new Exception("object activity is not an instance of Activity");
        }

        private async Task RequestPermissionsAsync(string[] permissions, int requestId, object activity)
        {
            await Task.Run(() => {
                RequestPermissions(permissions, requestId, activity);
            }).ConfigureAwait(false);
        }



        private string[] MapToAndroidPermissions(Permission permission)
        {
            switch (permission)
            {
                case Permission.Location:
                    return new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation };

                case Permission.Calendar:
                    return new string[] { Manifest.Permission.ReadCalendar, Manifest.Permission.WriteCalendar };

                case Permission.CallLog:
                    return new string[] { Manifest.Permission.ReadCallLog, Manifest.Permission.WriteCallLog, Manifest.Permission.ProcessOutgoingCalls };

                case Permission.Camera:
                    return new string[] { Manifest.Permission.Camera };

                case Permission.Contacts:
                    return new string[] { Manifest.Permission.ReadContacts, Manifest.Permission.WriteContacts, Manifest.Permission.GetAccounts };

                case Permission.Microphone:
                    return new string[] { Manifest.Permission.RecordAudio };

                case Permission.Phone:
                    return new string[] { Manifest.Permission.ReadPhoneState, Manifest.Permission.ReadPhoneNumbers, Manifest.Permission.CallPhone,
                                            Manifest.Permission.AnswerPhoneCalls, Manifest.Permission.AddVoicemail, Manifest.Permission.UseSip };

                case Permission.Sensors:
                    return new string[] { Manifest.Permission.BodySensors };

                case Permission.Sms:
                    return new string[] { Manifest.Permission.SendSms, Manifest.Permission.ReceiveSms, Manifest.Permission.ReadSms, Manifest.Permission.ReceiveWapPush, Manifest.Permission.ReceiveMms };

                case Permission.Storage:
                    return new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };


                // All other permersion are set in the Manifest and do not require runtime authoriozation
                // therefore they are a programming issue and should be assumed to be authorized
                default:
                    return null;
            }
        }

    }

}