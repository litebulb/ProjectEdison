using System.Collections.Generic;
using Android.App;
using Android.Content;
using Firebase.Iid;
using SharedConstants = Edison.Mobile.User.Client.Core.Shared.Constants;
using WindowsAzure.Messaging;
using Firebase;

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



        public static bool ValidPushNotificationToken
        {
            get
            {
                try
                {
                    return FirebaseInstanceId.Instance?.Token != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static FirebaseInstanceId GetFirebaseInstanceId()
        {
            return FirebaseInstanceId.Instance;
        }

        public static string GetPushNotificationToken()
        {
            return FirebaseInstanceId.Instance.Token;
        }

        public string TryForceTokenRefresh(string senderId)
        {
            string token = null;
            try
            {
                FirebaseApp firebaseApp = FirebaseApp.Instance;
                if (firebaseApp == null)
                    firebaseApp = FirebaseApp.InitializeApp(Application.Context, FirebaseOptions.FromResource(Application.Context));

                FirebaseInstanceId firebaseInstanceId = FirebaseInstanceId.Instance;
                if (firebaseInstanceId == null && firebaseApp != null)
                    firebaseInstanceId = FirebaseInstanceId.GetInstance(firebaseApp);

                if (firebaseInstanceId != null && firebaseApp != null)
                {
                    /// should force token to be refreshed
                    firebaseInstanceId.DeleteInstanceId();
                    token = FirebaseInstanceId.GetInstance(firebaseApp)?.GetToken(senderId, "FCM");
                }
            }
            catch { }
            return token;
        }



    }

}