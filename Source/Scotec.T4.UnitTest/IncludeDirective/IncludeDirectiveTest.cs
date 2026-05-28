#region

using System;
using System.Collections.Generic;
using Xunit;

#endregion


namespace Scotec.T4.UnitTest.IncludeDirective
{
    public class IncludeDirectiveTest : TestCase
    {
        [Fact]
        public void TestIncludeDirective1()
        {
            Run(@"IncludeDirective\IncludeDirectiveTest_1.t4", "BEFORE INCLUDE\r\nINCLUDED TEXT\r\nAFTER INCLUDE", new Dictionary<string, object>());
        }

        [Fact]
        public void TestIncludeDirective2()
        {
            Run(@"IncludeDirective\IncludeDirectiveTest_2.t4", "\r\nAFTER INCLUDE", new Dictionary<string, object>());
        }

        [Fact]
        public void TestIncludeDirective3()
        {
            Run(@"IncludeDirective\IncludeDirectiveTest_3.t4", "INCLUDED 3 NUMBER\r\nAFTER INCLUDE", new Dictionary<string, object>());
        }
    }
}