using System;
using NUnit.Framework;

namespace PatchKit.API.Tests
{
    [TestFixture]
    public class PatchKitAPISettingsUnitTests
    {
        [Test]
        public void ConstructorTests()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new PatchKitAPISettings(string.Empty);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new PatchKitAPISettings("http://api.patchkit.net", null, -1);
            });
        }

        [Test]
        public void FieldTests()
        {
            var settings = new PatchKitAPISettings();

            Assert.Throws<ArgumentException>(() =>
            {
                settings.Url = string.Empty;
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                settings.DelayBetweenMirrorRequests = -1;
            });
        }
    }
}
