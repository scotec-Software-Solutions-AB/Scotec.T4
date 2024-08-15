#region

using System.Collections.Generic;
using Xunit;

#endregion

namespace Scotec.T4.UnitTest.Parameters;

public class ParametersTest : TestCase
{
    [Fact]
    public void TestStringCs()
    {
        const string text1 = "TEXT1";
        const string text2 = "TEXT2";

        Generator.Settings.TemplateParameters.Add("Language", "C#");
        var parameters = new Dictionary<string, object>
        {
            { "text1", text1 },
            { "text2", text2 }
        };

        Run(@"Parameters\StringParameterTest.t4", $"{text1} {text2}", parameters);
    }

    [Fact]
    public void TestStringVb()
    {
        const string text1 = "TEXT1";
        const string text2 = "TEXT2";

        Generator.Settings.TemplateParameters.Add("Language", "VB");
        var parameters = new Dictionary<string, object>
        {
            { "text1", text1 },
            { "text2", text2 }
        };

        Run(@"Parameters\StringParameterTest.t4", $"{text1} {text2}", parameters);
    }
}
