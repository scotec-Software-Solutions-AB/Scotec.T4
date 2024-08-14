using System.Diagnostics.CodeAnalysis;

namespace Scotec.T4;

/// <summary>
///     Specifies the type of line breaks for the generated text output.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum EndOfLine
{
    /// <summary>
    ///     Microsoft Windows, DEC TOPS-10, RT-11 and most other early non-Unix and non-IBM OSes, CP/M, MP/M, DOS (MS-DOS,
    ///     PC-DOS, etc.), Atari TOS, OS/2, Symbian OS, Palm OSCrLf,
    /// </summary>
    CRLF,

    /// <summary>
    ///     Acorn BBC and RISC OS spooled text output.
    /// </summary>
    LFCR,

    /// <summary>
    ///     Commodore 8-bit machines, Acorn BBC, TRS-80, Apple II family, Mac OS up to version 9 and OS-9
    /// </summary>
    CR,

    /// <summary>
    ///     Multics, Unix and Unix-like systems (GNU/Linux, Mac OS X, FreeBSD, AIX, Xenix, etc.), BeOS, Amiga, RISC OS and
    ///     others.
    /// </summary>
    LF,

    /// <summary>
    ///     Next line (Unicode U+0085)
    /// </summary>
    NEL,

    /// <summary>
    ///     Line separator (Unicode U+2028)
    /// </summary>
    LS,

    /// <summary>
    ///     Paragraph separator (Unicode U+2029)
    /// </summary>
    PS
}
