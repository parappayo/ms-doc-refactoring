using NUnit.Framework;

namespace CancellationTokenExample
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Failing()
        {
            Assert.IsTrue(false);
        }
    }
}
