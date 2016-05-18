using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var result = API.GetAppVersionsList();

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);

            CollectionAssert.AreEquivalent(result.Result, Data.AppVersions);
        }

        [Test]
        public virtual void GetAppLatestVersionTest()
        {
            var result = API.GetAppLatestVersion();

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);
            Assert.AreEqual(result.Result, Data.AppLatestVersion);
        }

        [Test]
        public virtual void GetAppLatestVersionIDTest()
        {
            var result = API.GetAppLatestVersionID();

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);
            Assert.AreEqual(result.Result, Data.AppLatestVersionID);
        }

        [TestCaseSource("GetAppVersionTestData")]
        public virtual void GetAppVersionTest(int version, AppVersion appVersion)
        {
            var result = API.GetAppVersion(version);

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);

            Assert.AreEqual(result.Result, appVersion);
        }

        [TestCaseSource("GetAppContentSummaryTestData")]
        public virtual void GetAppContentSummaryTest(int version, AppContentSummary appContentSummary)
        {
            var result = API.GetAppContentSummary(version);

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);

            CollectionAssert.AreEquivalent(result.Result.Files, appContentSummary.Files);
            Assert.AreEqual(result.Result.CompressionMethod, appContentSummary.CompressionMethod);
            Assert.AreEqual(result.Result.EncryptionMethod, appContentSummary.EncryptionMethod);
            Assert.AreEqual(result.Result.Size, appContentSummary.Size);
        }

        [TestCaseSource("GetAppContentTorrentUrlTestData")]
        public virtual void GetAppContentTorrentUrlTest(int version, AppContentTorrentUrl appContentTorrentUrl)
        {
            var result = API.GetAppContentTorrentUrl(version);

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);

            Assert.AreEqual(result.Result, appContentTorrentUrl);
        }

        [TestCaseSource("GetAppContentUrlsTestData")]
        public virtual void GetAppContentUrlsTest(int version, AppContentUrl[] appContentUrls)
        {
            var result = API.GetAppContentUrls(version);

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);

            CollectionAssert.AreEquivalent(result.Result, appContentUrls);
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
            var result = API.GetAppDiffTorrentUrl(version);

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);

            Assert.AreEqual(result.Result, appDiffTorrentUrl);
        }

        [TestCaseSource("GetAppDiffUrlsTestData")]
        public virtual void GetAppDiffUrlsTest(int version, AppDiffUrl[] appDiffUrls)
        {
            var result = API.GetAppDiffUrls(version);

            while (!result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            Assert.IsNull(result.Exception);

            CollectionAssert.AreEquivalent(result.Result, appDiffUrls);
        }
    }
}
