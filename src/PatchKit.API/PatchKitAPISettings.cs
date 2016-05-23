using System;
using JetBrains.Annotations;

namespace PatchKit.API
{
    /// <summary>
    /// API settings.
    /// </summary>
    public class PatchKitAPISettings
    {
        private string _apiUrl;

        private long _delayBetweenMirrorRequests;

        public PatchKitAPISettings(string apiUrl = "http://api.patchkit.net", string[] mirrorAPIUrls = null, long delayBetweenMirrorRequests = 500)
        {
            APIUrl = apiUrl;
            MirrorAPIUrls = mirrorAPIUrls;
            DelayBetweenMirrorRequests = delayBetweenMirrorRequests;
        }

        /// <summary>
        /// The url for main api server.
        /// </summary>
        public string APIUrl
        {
            get
            {
                return _apiUrl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                _apiUrl = value;
            }
        }

        /// <summary>
        /// The urls for mirror api servers.
        /// </summary>
        [CanBeNull]
        public string[] MirrorAPIUrls { get; set; }

        /// <summary>
        /// Delay between mirror requests are sent.
        /// </summary>
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