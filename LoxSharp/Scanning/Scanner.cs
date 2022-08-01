namespace LoxSharp.Scanning;

public class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens;
    private int _start;
    private int _current;
    private int _line;
    private IErrorHandler _errorHandler;
    
    public Scanner(string source, IErrorHandler errorHandler)
    {
        _source = source;
        _tokens = new List<Token>();
        _start = 0;
        _line = 0;
        _current = 0;
        _errorHandler = errorHandler;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd)
        {
            _start = _current;
            ScanToken();
        }
        
        _tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));

        return _tokens;
    }

    /// <summary>
    /// Returns true if we are at the end of a file.
    /// </summary>
    private bool IsAtEnd => _current >= _source.Length;

    /// <summary>
    /// Advances to the next Char
    /// </summary>
    /// <returns></returns>
    private char ReadNext()
    {
        return _source[_current++];
    }
    
    /// <summary>
    /// Adds a new null token to our list. 
    /// </summary>
    private void AddToken(TokenType tokenType)
    {
        AddToken(tokenType, null);
    }
    
    /// <summary>
    /// Adds a new token to our list with values.
    /// </summary>
    private void AddToken(TokenType tokenType, object literal)
    {
        var text = Substring(_start, _current);
        _tokens.Add(new Token(tokenType, text, literal, _line));
    }

    /// <summary>
    /// Checks to see if our next char is the expected one if it is the char is consumed
    /// if not it's left alone.
    /// </summary>
    private bool Match(char expected)
    {
        // We can't scan past the end
        if (IsAtEnd)
        {
            return false;
        }
        // Is our expected char there?
        if (_source[_current] != expected)
        {
            return false;
        }
        _current++;
        return true;
    }
    
    /// <summary>
    /// Returns the current char without consuming it.
    /// </summary>
    private char Peek()
    {
        if (IsAtEnd)
        {
            return '\0';
        }
        return _source[_current];
    }
    
    
    /// <summary>
    /// Creates a substring of our source from a start
    /// and ending position.
    /// </summary>
    private string Substring(int start, int end)
    {
        var length = end - start;
        return _source.Substring(start, length);
    }
    
    /// <summary>
    /// Starts parsing out a token from a string
    /// </summary>
    private void ParseStringToken()
    {
        while (Peek() != '"' && !IsAtEnd)
        {
            // We allow multi line comments
            if (Peek() == '\n')
            {
                _line++;
            }
            ReadNext();
        }

        // Unterminated string.
        if (IsAtEnd)
        {
            _errorHandler.Error(_line, "Unterminated string");
            return;
        }

        // Skip to next line
        ReadNext();

        // Trim the surrounding quotes
        string value = Substring(_start + 1, _current - 1);
        AddToken(TokenType.String, value);
    }
    
    /// <summary>
    /// Starts parsing out a number token from a string
    /// </summary>
    private void ParseNumberToken()
    {
        // First part of number
        while (IsDigit(Peek()))
        {
            ReadNext();
        }

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            ReadNext();

            // Last part of number.
            while (IsDigit(Peek()))
            {
                ReadNext();
            }
        }

        // Parse our string
        string asString = Substring(_start, _current);
        double asDouble = double.Parse(asString);
        // Add our token
        AddToken(TokenType.Number, asDouble);
    }
    
    /// <summary>
    /// Returns the next char without consuming it.
    /// </summary>
    private char PeekNext()
    {
        if (_current + 1 >= _source.Length)
        {
            return '\0';
        }
        return _source[_current + 1];
    }

    private bool IsDigit(char c) => c is >= '0' and <= '9';

    /// <summary>
    /// Returns if this char is a letter or an underscore.
    /// </summary>
    private bool IsAlpha(char c)
    {
        return c is >= 'a' and <= 'z' ||
               c is >= 'A' and <= 'Z' ||
               c == '_';
    }
    
    /// <summary>
    /// Starts parsing out a identifier token from a string
    /// </summary>
    private void ParseIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            ReadNext();
        }

        // Grab our text
        string text = Substring(_start, _current);

        // Create our token
        TokenType type = Keywords.Get(text);
        // Check if it's not null
        if (type == TokenType.Undefined)
        {
            type = TokenType.Identifier;
        }
        AddToken(type);
    }

    /// <summary>
    /// Returns true if the character is a digit or a letter.
    /// </summary>
    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || char.IsDigit(c);
    }
    
    /// <summary>
    /// Reads  text in a in our source until we reach the end
    /// of a multi line comment
    /// </summary>
    private void ParseMultiLineComment()
    {
        int depth = 1;
        int startingLine = _line;
        while(true)
        {
            if (IsAtEnd)
            {
                _errorHandler.Error(startingLine, "Unterminated multi line comment");
                break;
            }

            char current = ReadNext();

            if(current == '/' && Match('*'))
            {
                // We hit a nested comment
                depth++;
            }
            else if (current == '*' && Match('/'))
            {
                depth--;

                if (depth == 0)
                {
                    break;
                }
            }

            if(current == '\n')
            {
                _line++;
            }
        }
    }
    
    private void ScanToken()
    {
        char c = ReadNext();
        switch (c)
        {
            // Braces
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            // Syntax
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            // Math
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            
            case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
            case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
            case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
            case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
            
            case '"': ParseStringToken(); break;
            
            // Whitespace
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;
            
            case '/':
                if (Match('/'))
                {
                    // A comment goes to the end of a line
                    while (Peek() != '\n' && !IsAtEnd)
                    {
                        // Move to the next char
                        ReadNext();
                    }
                }
                else if(Match('*'))
                {
                    ParseMultiLineComment();
                }
                else
                {
                    AddToken(TokenType.Slash);
                }
                break;
            
            default:
                // Handle all numbers
                if (IsDigit(c))
                {
                    ParseNumberToken();
                }
                else if (IsAlpha(c))
                {
                    ParseIdentifier();
                }
                else
                {
                    _errorHandler.Error(_line, "Unexpected character");
                }
                break;
        }
    }
}