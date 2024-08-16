#region

using System;
using System.Collections.Generic;
using Xunit;

#endregion


namespace Scotec.T4.UnitTest.TemplateDirective
{
    public class TemplateDirectiveTest : TestCase
    {
        [Fact]
        public void TestTemplateDirective()
        {
            //Generator.Settings.UseAssemblyResolver = true;
            Run(@"TemplateDirective\TemplateDirectiveTest.t4", "TEST", new Dictionary<string, object>());
        }

        [Fact]
        public void TestTemplateDirectiveFromString()
        {
            //Generator.Settings.UseAssemblyResolver = true;
            Run("<#@ template language=\"C#\" #>\r\nTEST", "TemplateDirectiveTest", "TEST", new Dictionary<string, object>());
        }

        [Fact]
        public void TestTemplateDirectiveWithClassname()
        {
            Run( @"TemplateDirective\TemplateDirectiveWithClassnameTest.t4", "", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestTemplateDirectiveWithCodefile()
        {
            Run( @"TemplateDirective\TemplateDirectiveWithCodefileTest.t4", "", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestTemplateDirectiveWithShortClassname()
        {
            Run( @"TemplateDirective\TemplateDirectiveWithShortClassnameTest.t4", "", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestTemplateDirectiveContainingMacros()
        {
            Generator.Options.TemplateParameters.Add( "Macro1", "Scotec" );
            Generator.Options.TemplateParameters.Add( "Macro2", "Testx" );
            Generator.Options.TemplateParameters.Add("Macro3", "TemplateDirectiveWithCodefileTest");
                                                      
            Run(@"TemplateDirective\TemplateDirectiveContainingMacrosTest.t4", "", new Dictionary<string, object>());
        }
    }
}