using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp.Lexer
{
    /// <summary>
    /// Token types used by the parser
    /// </summary>
    enum TokenType
    {
        Integer = 6,
        Float = 7,
        String = 8,
        Index = 4,
        Node = 1,
        Symbol = 5,
        BlockStart = 2,
        BlockEnd = 3
    }
}