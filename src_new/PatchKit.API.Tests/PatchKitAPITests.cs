using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PatchKit.API.Data;
using PatchKit.API.Web;

namespace PatchKit.API.Tests
{
    public abstract class PatchKitAPITests
    {
        protected PatchKitAPI API { get; private set; }

        protected IWWW WWW { get; private set; }

        protected static readonly PatchKitAPITestsData Data = PatchKitAPITestsData.Default;

        private readonly IWWWProvider _wwwProvider;

        protected PatchKitAPITests(IWWWProvider wwwProvider)
        {
            _wwwProvider = wwwProvider;
        }

        public static IEnumerable<object> GetAppVersionTestData
        {
            get
            {
                return Data.AppVersions.Select((t, i) => new object[] {i + 1, t});
            }
        }

        public static IEnumerable<object> GetAppContentSummaryTestData
        {
            get
            {
                return Data.AppContentSummaries.Select((t, i) => new object[] { i + 1, t });
            }
        }

        public static IEnumerable<object> GetAppContentTorrentUrlTestData
        {
            get
            {
                return Data.AppContentTorrentUrls.Select((t, i) => new object[] { i + 1, t });
            }
        }

        public static IEnumerable<object> GetAppContentUrlsTestData
        {
            get
            {
                return Data.AppContentUrls.Select((t, i) => new object[] { i + 1, t });
            }
        }

        public static IEnumerable<object> GetAppDiffSummaryTestData
        {
            get
            {
                return Data.AppDiffSummaries.Select((t, i) => new object[] { i + 2, t });
            }
        }

        public static IEnumerable<object> GetAppDiffTorrentUrlTestData
        {
            get
            {
                return Data.AppDiffTorrentUrls.Select((t, i) => new object[] { i + 2, t });
            }
        }

        public static IEnumerable<object> GetAppDiffUrlsTestData
        {
            get
            {
                return Data.AppDiffUrls.Select((t, i) => new object[] { i + 2, t });
            }
        }

        [SetUp]
        protected virtual void Init()
        {
            WWW = _wwwProvider.GetWWW();
            API = new PatchKitAPI(Data.Settings, WWW);
        }

        [Test]
        public virtual void GetAppVersionsListTest()
        {
            var asyncResult = API.BeginGetAppVersionsList();

            var result = API.EndGetAppVersionsList(asyncResult);

            CollectionAssert.AreEquivalent(result, Data.AppVersions);
        }

        [Test]
        public virtual void GetAppLatestVersionTest()
        {
            var asyncResult = API.BeginGetAppLatestVersion();

            var result = API.EndGetAppLatestVersion(asyncResult);

            Assert.AreEqual(result, Data.AppLatestVersion);
        }

        [Test]
        public virtual void GetAppLatestVersionIDTest()
        {
            var asyncResult = API.BeginGetAppLatestVersionID();

            var result = API.EndGetAppLatestVersionID(asyncResult);

            Assert.AreEqual(result, Data.AppLatestVersionID);
        }

        [TestCaseSource("GetAppVersionTestData")]
        public virtual void GetAppVersionTest(int version, AppVersion appVersion)
        {
            var asyncResult = API.BeginGetAppVersion(version);

            var result = API.EndGetAppVersion(asyncResult);

            Assert.AreEqual(result, appVersion);
        }

        [TestCaseSource("GetAppContentSummaryTestData")]
        public virtual void GetAppContentSummaryTest(int version, AppContentSummary appContentSummary)
        {
            var asyncResult = API.BeginGetAppContentSummary(version);

            var result = API.EndGetAppContentSummary(asyncResult);

            CollectionAssert.AreEquivalent(result.Files, appContentSummary.Files);
            Assert.AreEqual(result.CompressionMethod, appContentSummary.CompressionMethod);
            Assert.AreEqual(result.EncryptionMethod, appContentSummary.EncryptionMethod);
            Assert.AreEqual(result.Size, appContentSummary.Size);
        }

        [TestCaseSource("GetAppContentTorrentUrlTestData")]
        public virtual void GetAppContentTorrentUrlTest(int version, AppContentTorrentUrl appContentTorrentUrl)
        {
            var asyncResult = API.BeginGetAppContentTorrentUrl(version);

            var result = API.EndGetAppContentTorrentUrl(asyncResult);

            Assert.AreEqual(result, appContentTorrentUrl);
        }

        [TestCaseSource("GetAppContentUrlsTestData")]
        public virtual void GetAppContentUrlsTest(int version, AppContentUrl[] appContentUrls)
        {
            var asyncResult = API.BeginGetAppContentUrls(version);

            var result = API.EndGetAppContentUrls(asyncResult);

            CollectionAssert.AreEquivalent(result, appContentUrls);
        }

        //TODO: Diff summary test
        /*[TestCaseSource("GetAppDiffSummaryTestData")]
        public virtual void GetAppDiffSummaryTest(int version, AppDiffSummary appDiffSummary)
        {
            var result = API.GetAppDiffSummary(version);

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);

            CollectionAssert.AreEquivalent(result.Result.AddedFiles, appDiffSummary.AddedFiles);
            CollectionAssert.AreEquivalent(result.Result.ModifiedFiles, appDiffSummary.ModifiedFiles);
            CollectionAssert.AreEquivalent(result.Result.RemovedFiles, appDiffSummary.RemovedFiles);
            Assert.AreEqual(result.Result.CompressionMethod, appDiffSummary.CompressionMethod);
            Assert.AreEqual(result.Result.EncryptionMethod, appDiffSummary.EncryptionMethod);
            Assert.AreEqual(result.Result.Size, appDiffSummary.Size);
        }*/

        [TestCaseSource("GetAppDiffTorrentUrlTestData")]
        public virtual void GetAppDiffTorrentUrlTest(int version, AppDiffTorrentUrl appDiffTorrentUrl)
        {
            var asyncResult = API.BeginGetAppDiffTorrentUrl(version);

            var result = API.EndGetAppDiffTorrentUrl(asyncResult);

            Assert.AreEqual(result, appDiffTorrentUrl);
        }

        [TestCaseSource("GetAppDiffUrlsTestData")]
        public virtual void GetAppDiffUrlsTest(int version, AppDiffUrl[] appDiffUrls)
        {
            var asyncResult = API.BeginGetAppDiffUrls(version);

            var result = API.EndGetAppDiffUrls(asyncResult);

            CollectionAssert.AreEquivalent(result, appDiffUrls);
        }
    }
}
