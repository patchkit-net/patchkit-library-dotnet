using System;

namespace PatchKit.Patcher
{
    /// <summary>
    /// Settings for patcher.
    /// </summary>
    public struct PatcherSettings
    {
        /// <summary>
        /// Url to patching service.
        /// </summary>
        public Uri ServiceUrl;

        /// <summary>
        /// Secret key for application.
        /// </summary>
        public string SecretKey;
    }
}
