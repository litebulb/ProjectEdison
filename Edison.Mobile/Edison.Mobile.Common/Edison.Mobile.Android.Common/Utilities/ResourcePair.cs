

namespace Edison.Mobile.Android.Common
{
    public class ResourcePair
    {

        public int Resource1 { get; set; } = 0;
        public int Resource2 { get; set; } = 0;

        public ResourcePair() { }

        public ResourcePair(int resource1, int resource2)
        {
            Resource1 = resource1;
            Resource2 = resource2;
        }

    }
}