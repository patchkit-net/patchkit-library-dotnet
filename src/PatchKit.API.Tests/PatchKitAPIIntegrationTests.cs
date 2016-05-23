using NUnit.Framework;
using PatchKit.API.Web;

namespace PatchKit.API.Tests
{
    [TestFixture]
    public class PatchKitAPIIntegrationTests : PatchKitAPITests
    {
        private class WWWProvider : IWWWProvider
        {
            public IWWW GetWWW()
            {
                return new DefaultWWW();
            }
        }

        public PatchKitAPIIntegrationTests() : base(new WWWProvider())
        {
        }
    }
}
