

using Xunit;

namespace Scotec.T4.UnitTest.AssemblyDirective
{
    public class AssemblyDirectiveTest : TestCase
    {
        [Fact]
        public void TestAssemblyDirective()
        {
            Run( @"AssemblyDirective\AssemblyDirectiveTest.t4", "", new object[0] );
        }
    }
}