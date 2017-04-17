﻿using System;
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
            _apiConnectionSettings.MainServer = "main_server";
            _apiConnectionSettings.CacheServers = new[] {"cache_server_1", "cache_server_2"};
            _apiConnectionSettings.Timeout = 1;
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
                Is.TypeOf<ApiConnectionException>()
                    .And.Message.EqualTo("Server 'main_server' returned code 404"),
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