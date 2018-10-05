#region

using System;

#endregion


namespace OMS.Ice.T4Generator
{
    /// <summary>
    ///     The exception that is thrown when the text generation fails.
    /// </summary>
    [Serializable]
    public class T4Exception : Exception
    {
        internal T4Exception( string message )
            : base( message )
        {
        }

        internal T4Exception( string message, Exception innerException )
            : base( message, innerException )
        {
        }
    }
}