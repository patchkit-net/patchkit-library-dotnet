using System;

namespace PatchKit.API
{
    public class PatchKitAPISettings
    {
        private string _secretKey;

        private string _apiUrl;

        private string[] _mirrorAPIUrls;

        private long _delayBetweenMirrorRequests;

        public PatchKitAPISettings(string secretKey, string apiUrl, string[] mirrorAPIUrls, long delayBetweenMirrorRequests)
        {
            SecretKey = secretKey;
            APIUrl = apiUrl;
            MirrorAPIUrls = mirrorAPIUrls;
            _delayBetweenMirrorRequests = delayBetweenMirrorRequests;
        }

        public PatchKitAPISettings(string secretKey, string apiUrl, string[] mirrorAPIUrls) : this(secretKey, apiUrl, mirrorAPIUrls, 5000)
        {
        }

        public PatchKitAPISettings(string secretKey, string apiUrl) : this(secretKey, apiUrl, new string[0])
        {
        }

        public PatchKitAPISettings(string secretKey) : this(secretKey, "http://api.patchkit.net")
        {
        }

        public string SecretKey
        {
            get
            {
                return _secretKey;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _secretKey = value;
            }
        }

        public string APIUrl
        {
            get
            {
                return _apiUrl;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _apiUrl = value;
            }
        }

        public string[] MirrorAPIUrls
        {
            get
            {
                return _mirrorAPIUrls;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _mirrorAPIUrls = value;
            }
        }

        public long DelayBetweenMirrorRequests
        {
            get
            {
                return _delayBetweenMirrorRequests;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _delayBetweenMirrorRequests = value;
            }
        }
    }
}