#region

using System;
using System.Collections;
using System.Text.RegularExpressions;

#endregion


namespace Scotec.T4Generator
{
    internal class MacroResolver
    {
        private static readonly Regex Regex = new Regex( @"%(([^%])+)%", RegexOptions.Compiled );

        public MacroResolver( IGeneratorSettings settings )
        {
            Settings = settings;
        }

        private IGeneratorSettings Settings { get; }

        public string Resolve( string text )
        {
            var result = text;

            var matches = Regex.Matches( text );
            foreach( Match match in matches )
            {
                var macro = match.Value;
                result = result.Replace( macro, GetValue( macro.Trim( '%' ) ) );
            }

            return result;
        }

        private string GetValue( string macro )
        {
            // First try to get the value from options.
            if( Settings.TemplateParameters != null && Settings.TemplateParameters.TryGetValue( macro, out var result ) )
                return result;

            // Search in environment variables.
            foreach( DictionaryEntry entry in Environment.GetEnvironmentVariables() )
            {
                if( macro.ToLower() == entry.Key.ToString().ToLower() )
                    return entry.Value.ToString();
            }

            return macro;
        }
    }
}