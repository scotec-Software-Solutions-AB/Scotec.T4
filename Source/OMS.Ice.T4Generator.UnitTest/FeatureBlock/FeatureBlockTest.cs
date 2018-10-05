#region

using Xunit;

#endregion


namespace OMS.Ice.T4Generator.UnitTest.FeatureBlock
{
   public class FeatureBlockTest : TestCase
    {
        [Fact]
        public void TestFeatureBlock()
        {
            Run(@"FeatureBlock\FeatureBlockTest.t4", "11111\r\n22222\r\n\r\n\r\n", new object[0]);
        }
    }
}