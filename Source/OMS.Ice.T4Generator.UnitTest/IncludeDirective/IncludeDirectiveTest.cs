#region

using Xunit;

#endregion


namespace OMS.Ice.T4Generator.UnitTest.IncludeDirective
{
    public class IncludeDirectiveTest : TestCase
    {
        [Fact]
        public void TestIncludeDirective1()
        {
            Run(@"IncludeDirective\IncludeDirectiveTest_1.t4", "BEFORE INCLUDE\r\nINCLUDED TEXT\r\nAFTER INCLUDE", new object[0]);
        }

        [Fact]
        public void TestIncludeDirective2()
        {
            Run(@"IncludeDirective\IncludeDirectiveTest_2.t4", "\r\nAFTER INCLUDE", new object[0]);
        }

        [Fact]
        public void TestIncludeDirective3()
        {
            Run(@"IncludeDirective\IncludeDirectiveTest_3.t4", "INCLUDED 3 NUMBER\r\nAFTER INCLUDE", new object[0]);
        }
    }
}