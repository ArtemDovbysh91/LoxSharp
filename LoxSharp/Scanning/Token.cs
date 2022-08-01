namespace LoxSharp.Scanning;

public class Token
{
    public TokenType Type;
    public string Lexeme;
    public object Literal;
    public int Line;

    public Token(TokenType type, string lexeme, object literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    /// <summary>
    /// A handy string name for this class.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}