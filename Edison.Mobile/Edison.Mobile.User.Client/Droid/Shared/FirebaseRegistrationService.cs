using System.Collections.Generic;
using Android.App;
using Android.Content;
using Firebase.Iid;
using SharedConstants = Edison.Mobile.User.Client.Core.Shared.Constants;
using WindowsAzure.Messaging;

namespace Edison.Mobile.User.Client.Droid.Shared
{

    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseRegistrationService : FirebaseInstanceIdService
    {
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            SendRegistrationToServer(refreshedToken);
        }
        void SendRegistrationToServer(string token)
        {
            var hub = new NotificationHub(SharedConstants.NotificationHubName, SharedConstants.ListenConnectionString, this);
            var tags = new List<string>();
            var regId = hub.Register(token, tags.ToArray()).RegistrationId;
        }
    }

}