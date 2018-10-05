#region

using Xunit;

#endregion


namespace OMS.Ice.T4Generator.UnitTest.TemplateDirective
{
    public class TemplateDirectiveTest : TestCase
    {
        [Fact]
        public void TestTemplateDirective()
        {
            //Generator.Settings.UseAssemblyResolver = true;
            Run(@"TemplateDirective\TemplateDirectiveTest.t4", "", new object[0]);
        }

        [Fact]
        public void TestTemplateDirectiveWithClassname()
        {
            Run( @"TemplateDirective\TemplateDirectiveWithClassnameTest.t4", "", new object[0] );
        }

        [Fact]
        public void TestTemplateDirectiveWithCodefile()
        {
            Run( @"TemplateDirective\TemplateDirectiveWithCodefileTest.t4", "", new object[0] );
        }

        [Fact]
        public void TestTemplateDirectiveWithShortClassname()
        {
            Run( @"TemplateDirective\TemplateDirectiveWithShortClassnameTest.t4", "", new object[0] );
        }

        [Fact]
        public void TestTemplateDirectiveContainingMacros()
        {
            Generator.Settings.TemplateParameters.Add( "Macro1", "OMS.Ice" );
            Generator.Settings.TemplateParameters.Add( "Macro2", "Testx" );
            Generator.Settings.TemplateParameters.Add("Macro3", "TemplateDirectiveWithCodefileTest");
                                                      
            Run(@"TemplateDirective\TemplateDirectiveContainingMacrosTest.t4", "", new object[0]);
        }
    }
}