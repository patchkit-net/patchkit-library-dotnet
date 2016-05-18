using System;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
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
                return Substitute.For<IWWW>();
            }
        }

        public PatchKitAPIUnitTests() : base(new WWWProvider())
        {
        }

        private void WWWRequestArg(IWWW www, string apiUrl, string url)
        {
            string fullUrl = apiUrl.EndsWith("/") ? apiUrl + url : apiUrl + "/" + url;

            www.DownloadString(Arg.Is<WWWRequest<string>>(request => request.Url == fullUrl));
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new PatchKitAPI(new PatchKitAPISettings(string.Empty), null);
            });
        }

        [Test]
        public override void GetAppVersionsListTest()
        {
            var url = string.Format("1/apps/{0}/versions", Data.Settings.SecretKey);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(Data.AppVersions), 200);
            });

            base.GetAppVersionsListTest();

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        [Test]
        public override void GetAppLatestVersionTest()
        {
            string url = string.Format("1/apps/{0}/versions/latest", Data.Settings.SecretKey);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(Data.AppLatestVersion), 200);
            });

            base.GetAppLatestVersionTest();

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        [Test]
        public override void GetAppLatestVersionIDTest()
        {
            string url = string.Format("1/apps/{0}/versions/latest/id", Data.Settings.SecretKey);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(Data.AppLatestVersionID), 200);
            });

            base.GetAppLatestVersionIDTest();

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        [TestCaseSource("GetAppVersionTestData")]
        public override void GetAppVersionTest(int version, AppVersion appVersion)
        {
            string url = string.Format("1/apps/{0}/versions/{1}", Data.Settings.SecretKey, version);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(appVersion), 200);
            });

            base.GetAppVersionTest(version, appVersion);

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        [TestCaseSource("GetAppContentSummaryTestData")]
        public override void GetAppContentSummaryTest(int version, AppContentSummary appContentSummary)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/content_summary", Data.Settings.SecretKey, version);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(appContentSummary), 200);
            });

            base.GetAppContentSummaryTest(version, appContentSummary);

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        [TestCaseSource("GetAppContentTorrentUrlTestData")]
        public override void GetAppContentTorrentUrlTest(int version, AppContentTorrentUrl appContentTorrentUrl)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/content_torrent_url", Data.Settings.SecretKey, version);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(appContentTorrentUrl), 200);
            });

            base.GetAppContentTorrentUrlTest(version, appContentTorrentUrl);

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        [TestCaseSource("GetAppContentUrlsTestData")]
        public override void GetAppContentUrlsTest(int version, AppContentUrl[] appContentUrls)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/content_urls", Data.Settings.SecretKey, version);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(appContentUrls), 200);
            });

            base.GetAppContentUrlsTest(version, appContentUrls);

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        // TODO: Diff summary unit test
        /*[TestCaseSource("GetAppDiffSummaryTestData")]
        public override void GetAppDiffSummaryTest(int version, AppDiffSummary appDiffSummary)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/diff_summary", Data.Settings.SecretKey, version);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(appDiffSummary), 200);
            });

            base.GetAppDiffSummaryTest(version, appDiffSummary);

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }*/

        [TestCaseSource("GetAppDiffTorrentUrlTestData")]
        public override void GetAppDiffTorrentUrlTest(int version, AppDiffTorrentUrl appDiffTorrentUrl)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/diff_torrent_url", Data.Settings.SecretKey, version);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(appDiffTorrentUrl), 200);
            });

            base.GetAppDiffTorrentUrlTest(version, appDiffTorrentUrl);

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }

        [TestCaseSource("GetAppDiffUrlsTestData")]
        public override void GetAppDiffUrlsTest(int version, AppDiffUrl[] appDiffUrls)
        {
            string url = string.Format("1/apps/{0}/versions/{1}/diff_urls", Data.Settings.SecretKey, version);

            WWW.When(www => WWWRequestArg(www, Data.Settings.APIUrl, url)).Do(info =>
            {
                info.Arg<WWWRequest<string>>().Complete(JsonConvert.SerializeObject(appDiffUrls), 200);
            });

            base.GetAppDiffUrlsTest(version, appDiffUrls);

            WWWRequestArg(WWW.Received(1), Data.Settings.APIUrl, url);
        }
    }
}
