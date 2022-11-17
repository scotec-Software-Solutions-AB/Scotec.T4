#region

using System.Diagnostics;
using System.IO;

#endregion


namespace Scotec.T4Generator
{
    /// <summary>
    ///     Base class for the generated classes. This class should be used by the generator only.
    /// </summary>
    public abstract class T4Generator
    {
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="endOfLine">The line ending used for the generated text.</param>
        protected T4Generator( string endOfLine )
        {
            EndOfLine = endOfLine;
        }

        /// <summary>
        /// The line ending used for the generated text.
        /// </summary>
        public string EndOfLine { get; }

        /// <summary>
        ///     The TextWriter can be used in the template code to write textual data directly into the output.
        /// </summary>
        protected TextWriter Output { get; private set; }

        /// <summary>
        ///     Generates the textual output. This method will be called by the template generator.
        /// </summary>
        /// <param name="output"> The target stream. </param>
        public void Generate( TextWriter output )
        {
            Output = output;

            Generate();
        }

        /// <summary>
        ///     Used by the T4 text template generator.
        /// </summary>
        protected abstract void Generate();

        /// <summary>
        ///     Used by the T4 text template generator. Can be also called in the code part of the template.
        /// </summary>
        [DebuggerStepThrough]
        protected void Write( string text )
        {
            if( text != null )
                Output.Write( text );
        }

        /// <summary>
        ///     Used by the T4 text template generator. Can be also called in the code part of the template.
        /// </summary>
        [DebuggerStepThrough]
        protected void Write( object value )
        {
            if( value != null )
                Write( value.ToString() );
        }
    }
}