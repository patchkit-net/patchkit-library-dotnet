using System;
using JetBrains.Annotations;

namespace PatchKit.API
{
    /// <summary>
    /// <see cref="PatchKitAPI"/> settings.
    /// </summary>
    public class PatchKitAPISettings
    {
        private string _url;

        private long _delayBetweenMirrorRequests;

        public PatchKitAPISettings(string url = "http://api.patchkit.net", string[] mirrorUrls = null, long delayBetweenMirrorRequests = 500)
        {
            Url = url;
            MirrorUrls = mirrorUrls;
            DelayBetweenMirrorRequests = delayBetweenMirrorRequests;
        }

        /// <summary>
        /// The url for main API server.
        /// </summary>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                _url = value;
            }
        }

        /// <summary>
        /// The urls for mirror API servers.
        /// </summary>
        [CanBeNull]
        public string[] MirrorUrls { get; set; }

        /// <summary>
        /// Delay between which mirror requests are sent.
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