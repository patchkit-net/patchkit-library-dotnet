using NUnit.Framework;
using PatchKit.API.Web;

namespace PatchKit.API.Tests
{
    [TestFixture]
    public class PatchKitAPIIntegrationTests : PatchKitAPITests
    {
        private class WWWProvider : IWWWProvider
        {
            public IStringDownloader GetWWW()
            {
                return new StringDownloader();
            }
        }

        public PatchKitAPIIntegrationTests() : base(new WWWProvider())
        {
        }
    }
}
