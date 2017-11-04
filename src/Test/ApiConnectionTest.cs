using System;
using System.IO;
using System.Net;
using System.Text;
using NSubstitute;
using NUnit.Framework;

namespace PatchKit.Api
{
    [TestFixture]
    public class ApiConnectionTest
    {
        private ApiConnectionSettings _apiConnectionSettings;

        [SetUp]
        public void SetUp()
        {
            _apiConnectionSettings = new ApiConnectionSettings();
            _apiConnectionSettings.MainServer = new ApiConnectionServer()
            {
                Host = "main_server",
                Timeout = 1
            };
            _apiConnectionSettings.CacheServers = new[]
            {
                new ApiConnectionServer()
                {
                    Host = "cache_server_1",
                    Timeout = 1
                },
                new ApiConnectionServer()
                {
                    Host = "cache_server_2",
                    Timeout = 1
                }
            };
        }

        [Test]
        public void TestSingle()
        {
            var apiConnection = new ApiConnection(_apiConnectionSettings);

            var webRequest = Substitute.For<IHttpWebRequest>();
            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;
            factory.Create("http://main_server/path?query").Returns(webRequest);

            var webResponse = CreateSimpleWebResponse("test");
            webRequest.GetResponse().Returns(webResponse);

            var apiResponse = apiConnection.GetResponse("/path", "query");
            Assert.AreEqual("test", apiResponse.Body);
        }

        [Test]
        public void TestHttps()
        {
            _apiConnectionSettings.MainServer.UseHttps = true;
            var apiConnection = new ApiConnection(_apiConnectionSettings);

            var webRequest = Substitute.For<IHttpWebRequest>();
            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;
            factory.Create("https://main_server/path?query").Returns(webRequest);

            var webResponse = CreateSimpleWebResponse("test");
            webRequest.GetResponse().Returns(webResponse);

            var apiResponse = apiConnection.GetResponse("/path", "query");
            Assert.AreEqual("test", apiResponse.Body);
        }
        
        [Test]
        public void TestCustomPort()
        {
            _apiConnectionSettings.MainServer.Port = 81;
            var apiConnection = new ApiConnection(_apiConnectionSettings);

            var webRequest = Substitute.For<IHttpWebRequest>();
            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;
            factory.Create("http://main_server:81/path?query").Returns(webRequest);

            var webResponse = CreateSimpleWebResponse("test");
            webRequest.GetResponse().Returns(webResponse);

            var apiResponse = apiConnection.GetResponse("/path", "query");
            Assert.AreEqual("test", apiResponse.Body);
        }

        [Test]
        public void TestApiCache500()
        {
            // test how connection will behave on 500 error

            var apiConnection = new ApiConnection(_apiConnectionSettings);

            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;

            // main
            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/path?query").Returns(mainWebRequest);

            var mainResponse = Substitute.For<IHttpWebResponse>();
            mainResponse.StatusCode.Returns(HttpStatusCode.InternalServerError);
            mainWebRequest.GetResponse().Returns(mainResponse);

            // cache
            var cacheWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://cache_server_1/path?query").Returns(cacheWebRequest);

            var cacheWebResponse = CreateSimpleWebResponse("test");
            cacheWebRequest.GetResponse().Returns(cacheWebResponse);

            var apiResponse = apiConnection.GetResponse("/path", "query");
            Assert.AreEqual("test", apiResponse.Body);
        }

        [Test]
        public void TestApiCache502()
        {
            // test how connection will behave on 502 error

            var apiConnection = new ApiConnection(_apiConnectionSettings);

            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;

            // main
            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/path?query").Returns(mainWebRequest);

            var mainResponse = Substitute.For<IHttpWebResponse>();
            mainResponse.StatusCode.Returns(HttpStatusCode.BadGateway);
            mainWebRequest.GetResponse().Returns(mainResponse);

            // cache
            var cacheWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://cache_server_1/path?query").Returns(cacheWebRequest);

            var cacheWebResponse = CreateSimpleWebResponse("test");
            cacheWebRequest.GetResponse().Returns(cacheWebResponse);

            var apiResponse = apiConnection.GetResponse("/path", "query");
            Assert.AreEqual("test", apiResponse.Body);
        }

        [Test]
        public void Test4XXException()
        {
            // any 4XX error from the main server should throw an exception
            var apiConnection = new ApiConnection(_apiConnectionSettings);

            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;

            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/path?query").Returns(mainWebRequest);

            var mainResponse = Substitute.For<IHttpWebResponse>();
            mainResponse.StatusCode.Returns(HttpStatusCode.NotFound);
            mainWebRequest.GetResponse().Returns(mainResponse);
            mainWebRequest.Address.Returns(new Uri("http://main_server"));

            Assert.Throws(
                Is.TypeOf<ApiResponseException>()
                    .And.Message.EqualTo("API server returned status code 404"),
                () => apiConnection.GetResponse("/path", "query")
            );
        }

        [Test]
        public void TestErrorOnCache()
        {
            // Any code different than 200 on API cache should skip it
            var apiConnection = new ApiConnection(_apiConnectionSettings);

            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;

            // main
            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/path?query").Returns(mainWebRequest);

            var mainResponse = CreateErrorResponse(HttpStatusCode.BadGateway);
            mainWebRequest.GetResponse().Returns(mainResponse);

            // cache1
            var cache1WebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://cache_server_1/path?query").Returns(cache1WebRequest);
            cache1WebRequest.Address.Returns(new Uri("http://cache_server_1"));

            var cache1WebResponse = CreateErrorResponse(HttpStatusCode.NotFound);
            cache1WebRequest.GetResponse().Returns(cache1WebResponse);

            // cache2
            var cache2WebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://cache_server_2/path?query").Returns(cache2WebRequest);
            var cache2WebResponse = CreateSimpleWebResponse("test");
            cache2WebRequest.GetResponse().Returns(cache2WebResponse);
            cache2WebRequest.Address.Returns(new Uri("http://cache_server_2"));

            var apiResponse = apiConnection.GetResponse("/path", "query");
            Assert.AreEqual("test", apiResponse.Body);

        }

        [Test]
        public void TestGetContentUrls()
        {
            var apiConnection = new MainApiConnection(_apiConnectionSettings);
            
            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;
            
            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/1/apps/secret/versions/13/content_urls").Returns(mainWebRequest);
            
            var webResponse = CreateSimpleWebResponse(
                "[{\"url\": \"http://first\", \"meta_url\": \"http://efg\", \"country\": \"PL\"}, " +
                "{\"url\": \"http://second\", \"meta_url\": \"http://efg\"}]");
            mainWebRequest.GetResponse().Returns(webResponse);

            var contentUrls = apiConnection.GetAppVersionContentUrls("secret", 13);
            Assert.AreEqual(2, contentUrls.Length);
            Assert.AreEqual("http://first", contentUrls[0].Url);
            Assert.AreEqual("PL", contentUrls[0].Country);
            Assert.AreEqual(null, contentUrls[1].Country);
        }
        
        [Test]
        public void TestGetContentUrlsWithCountry()
        {
            var apiConnection = new MainApiConnection(_apiConnectionSettings);
            
            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;
            
            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/1/apps/secret/versions/13/content_urls?country=PL").Returns(mainWebRequest);
            
            var webResponse = CreateSimpleWebResponse(
                "[{\"url\": \"http://first\", \"meta_url\": \"http://efg\", \"country\": \"PL\"}, " +
                "{\"url\": \"http://second\", \"meta_url\": \"http://efg\"}]");
            mainWebRequest.GetResponse().Returns(webResponse);

            var contentUrls = apiConnection.GetAppVersionContentUrls("secret", 13, "PL");
            
            Assert.AreEqual(2, contentUrls.Length);
            Assert.AreEqual("http://first", contentUrls[0].Url);
            Assert.AreEqual("PL", contentUrls[0].Country);
            Assert.AreEqual(null, contentUrls[1].Country);
        }
        
        [Test]
        public void TestGetDiffUrls()
        {
            var apiConnection = new MainApiConnection(_apiConnectionSettings);
            
            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;
            
            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/1/apps/secret/versions/13/diff_urls").Returns(mainWebRequest);
            
            var webResponse = CreateSimpleWebResponse(
                "[{\"url\": \"http://first\", \"meta_url\": \"http://efg\", \"country\": \"PL\"}, " +
                "{\"url\": \"http://second\", \"meta_url\": \"http://efg\"}]");
            mainWebRequest.GetResponse().Returns(webResponse);

            var contentUrls = apiConnection.GetAppVersionDiffUrls("secret", 13);
            Assert.AreEqual(2, contentUrls.Length);
            Assert.AreEqual("http://first", contentUrls[0].Url);
            Assert.AreEqual("PL", contentUrls[0].Country);
            Assert.AreEqual(null, contentUrls[1].Country);
        }
        
        [Test]
        public void TestGetDiffUrlsWithCountry()
        {
            var apiConnection = new MainApiConnection(_apiConnectionSettings);
            
            var factory = Substitute.For<IHttpWebRequestFactory>();
            apiConnection.HttpWebRequestFactory = factory;
            
            var mainWebRequest = Substitute.For<IHttpWebRequest>();
            factory.Create("http://main_server/1/apps/secret/versions/13/diff_urls?country=PL").Returns(mainWebRequest);
            
            var webResponse = CreateSimpleWebResponse(
                "[{\"url\": \"http://first\", \"meta_url\": \"http://efg\", \"country\": \"PL\"}, " +
                "{\"url\": \"http://second\", \"meta_url\": \"http://efg\"}]");
            mainWebRequest.GetResponse().Returns(webResponse);

            var contentUrls = apiConnection.GetAppVersionDiffUrls("secret", 13, "PL");
            
            Assert.AreEqual(2, contentUrls.Length);
            Assert.AreEqual("http://first", contentUrls[0].Url);
            Assert.AreEqual("PL", contentUrls[0].Country);
            Assert.AreEqual(null, contentUrls[1].Country);
        }

        private static IHttpWebResponse CreateErrorResponse(HttpStatusCode statusCode)
        {
            var mainResponse = Substitute.For<IHttpWebResponse>();
            mainResponse.StatusCode.Returns(statusCode);
            return mainResponse;
        }

        private static IHttpWebResponse CreateSimpleWebResponse(string str)
        {
            var webResponse = Substitute.For<IHttpWebResponse>();
            webResponse.GetResponseStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes(str)));
            webResponse.CharacterSet.Returns("UTF-8");
            webResponse.StatusCode.Returns(HttpStatusCode.OK);
            return webResponse;
        }
    }
}