#region

using System;
using System.Collections.Generic;
using Xunit;

#endregion


namespace Scotec.T4.UnitTest.FeatureBlock
{
   public class FeatureBlockTest : TestCase
    {
        [Fact]
        public void TestFeatureBlock()
        {
            Run(@"FeatureBlock\FeatureBlockTest.t4", "11111\r\n22222\r\n\r\n\r\n", new Dictionary<string, object>());
        }
    }
}