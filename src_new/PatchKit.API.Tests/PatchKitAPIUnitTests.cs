using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using PatchKit.API.Async;
using PatchKit.API.Data;
using PatchKit.API.Web;

namespace PatchKit.API.Tests
{
    [TestFixture]
    public class PatchKitAPIUnitTests : PatchKitAPITests
    {
        private class WWWProvider : IWWWProvider
        {
            public IWWW GetWWW()
            {
                var www = Substitute.For<IWWW>();

                return www;
            }
        }

        public PatchKitAPIUnitTests() : base(new WWWProvider())
        {
        }

        private static string GetUrl(string apiUrl, string methodUrl)
        {
            return apiUrl.EndsWith("/") ? apiUrl + methodUrl : apiUrl + "/" + methodUrl;
        }

        private void DownloadStringSubstitute(string apiUrl, string methodUrl, string content, int statusCode)
        {
            var ar = Substitute.For<ICancellableAsyncResult>();

            ar.IsCancelled.Returns(false);
            ar.IsCompleted.Returns(true);
            ar.AsyncWaitHandle.Returns(delegate
            {
                return new ManualResetEvent(true);
            });
            ar.AsyncState.Returns(null);
            ar.CompletedSynchronously.Returns(false);
            ar.Cancel().Returns(false);

            WWW.BeginDownloadString(GetUrl(apiUrl, methodUrl), Arg.Any<CancellableAsyncCallback>(), Arg.Any<object>()).Returns(
                info =>
                {
                    var callback = info.Arg<CancellableAsyncCallback>();
                    if (callback != null)
                    {
                        callback(ar);
                    }
                    return ar;
                });
            WWW.EndDownloadString(ar).Returns(info => new WWWResponse<string>(content, statusCode));
        }

        private void DownloadStringSubstitute(string methodUrl, string content)
        {
            if (Data.Settings.MirrorAPIUrls != null)
                foreach (var mirrorAPIUrl in Data.Settings.MirrorAPIUrls)
                {
                    DownloadStringSubstitute(mirrorAPIUrl, methodUrl, content, 200);
                }

            DownloadStringSubstitute(Data.Settings.APIUrl, methodUrl, content, 200);
        }

        private void CheckDownloadStringSubstitute(string apiUrl, string methodUrl)
        {
            WWW.Received(1).BeginDownloadString(GetUrl(apiUrl, methodUrl), Arg.Any<CancellableAsyncCallback>(), Arg.Any<object>());
        }

        private void CheckDownloadStringSubstitute(string methodUrl) 
        {
            CheckDownloadStringSubstitute(Data.Settings.APIUrl, methodUrl);
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                // ReSharper disable once AssignNullToNotNullAttribute
                new PatchKitAPI(null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                // ReSharper disable once AssignNullToNotNullAttribute
                new PatchKitAPI(new PatchKitAPISettings(), null);
            });
        }

        [Test]
        public void AsyncResultToTask()
        {
            var url = string.Format("1/apps/{0}/versions", Data.SecretKey);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(Data.AppVersions));

            var task = Task.Factory.FromAsync((callback, o) => API.BeginGetAppVersionsList(Data.SecretKey, (ar) => callback(ar), o),
                result => API.EndGetAppVersionsList(result), null);

            task.Wait();

            Assert.AreEqual(task.Result, Data.AppVersions);

            CheckDownloadStringSubstitute(url);
        }

        [Test]
        public override void GetAppVersionsListTest()
        {
            var url = string.Format("1/apps/{0}/versions", Data.SecretKey);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(Data.AppVersions));

            base.GetAppVersionsListTest();

            CheckDownloadStringSubstitute(url);
        }

        [Test]
        public override void GetAppLatestVersionTest()
        {
            var url = string.Format("1/apps/{0}/versions/latest", Data.SecretKey);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(Data.AppLatestVersion));

            base.GetAppLatestVersionTest();

            CheckDownloadStringSubstitute(url);
        }

        [Test]
        public override void GetAppLatestVersionIdTest()
        {
            var url = string.Format("1/apps/{0}/versions/latest/id", Data.SecretKey);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(Data.AppLatestVersionId));

            base.GetAppLatestVersionIdTest();

            CheckDownloadStringSubstitute(url);
        }

        [TestCaseSource("GetAppVersionTestData")]
        public override void GetAppVersionTest(int version, AppVersion appVersion)
        {
            string url = string.Format("1/apps/{0}/versions/{1}", Data.SecretKey, version);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(appVersion));

            base.GetAppVersionTest(version, appVersion);

            CheckDownloadStringSubstitute(url);
        }

        [TestCaseSource("GetAppContentSummaryTestData")]
        public override void GetAppContentSummaryTest(int version, AppContentSummary appContentSummary)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/content_summary", Data.SecretKey, version);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(appContentSummary));

            base.GetAppContentSummaryTest(version, appContentSummary);

            CheckDownloadStringSubstitute(url);
        }

        [TestCaseSource("GetAppContentTorrentUrlTestData")]
        public override void GetAppContentTorrentUrlTest(int version, AppContentTorrentUrl appContentTorrentUrl)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/content_torrent_url", Data.SecretKey, version);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(appContentTorrentUrl));

            base.GetAppContentTorrentUrlTest(version, appContentTorrentUrl);

            CheckDownloadStringSubstitute(url);
        }

        [TestCaseSource("GetAppContentUrlsTestData")]
        public override void GetAppContentUrlsTest(int version, AppContentUrl[] appContentUrls)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/content_urls", Data.SecretKey, version);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(appContentUrls));

            base.GetAppContentUrlsTest(version, appContentUrls);

            CheckDownloadStringSubstitute(url);
        }

        // TODO: Diff summary unit test
        /*[TestCaseSource("GetAppDiffSummaryTestData")]
        public override void GetAppDiffSummaryTest(int version, AppDiffSummary appDiffSummary)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/diff_summary", Data.SecretKey, version);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(appDiffSummary));

            base.GetAppDiffSummaryTest(version, appDiffSummary);

            CheckDownloadStringSubstitute(url);
        }*/

        [TestCaseSource("GetAppDiffTorrentUrlTestData")]
        public override void GetAppDiffTorrentUrlTest(int version, AppDiffTorrentUrl appDiffTorrentUrl)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/diff_torrent_url", Data.SecretKey, version);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(appDiffTorrentUrl));

            base.GetAppDiffTorrentUrlTest(version, appDiffTorrentUrl);

            CheckDownloadStringSubstitute(url);
        }

        [TestCaseSource("GetAppDiffUrlsTestData")]
        public override void GetAppDiffUrlsTest(int version, AppDiffUrl[] appDiffUrls)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/diff_urls", Data.SecretKey, version);

            DownloadStringSubstitute(url, JsonConvert.SerializeObject(appDiffUrls));

            base.GetAppDiffUrlsTest(version, appDiffUrls);

            CheckDownloadStringSubstitute(url);
        }
    }
}
