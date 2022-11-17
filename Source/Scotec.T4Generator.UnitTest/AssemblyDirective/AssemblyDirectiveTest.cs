

using Xunit;

namespace Scotec.T4Generator.UnitTest.AssemblyDirective
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