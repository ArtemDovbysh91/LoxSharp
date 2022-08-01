using LoxSharp.AST;
using LoxSharp.Exceptions;
using LoxSharp.Scanning;

namespace LoxSharp.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private readonly IErrorHandler _errorHandler;
    private int _current = 0;

    public Parser(List<Token> tokens, IErrorHandler errorHandler)
    {
        _tokens = tokens;
        _errorHandler = errorHandler;
    }
    
    public Expression Parse()
    {
        try
        {
            return Expression();
        }
        catch
        {
            return null!;
        }
    }
    
    private Expression Expression()
    {
        return Equality();
    }

    private Expression Equality()
    {
        Expression expression = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            Token @operator = Previous();
            Expression right = Comparison();
            expression = new BinaryExpression(expression, @operator, right);
        }

        return expression;
    }

    private Expression Comparison()
    {
        Expression expression = Term();

        while(Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.LeftBrace, TokenType.LessEqual))
        {
            Token @operator = Previous();
            Expression right = Term();
            expression = new BinaryExpression(expression, @operator, right);
        }

        return expression;
    }

    private Expression Term()
    {
        Expression expression = Factor();

        while(Match(TokenType.Minus, TokenType.Plus))
        {
            Token @operator = Previous();
            Expression right = Factor();
            expression = new BinaryExpression(expression, @operator, right); 
        }

        return expression;
    }
    
    private Expression Factor()
    {
        Expression expression = Unary();

        while (Match(TokenType.Slash, TokenType.Star))
        {
            Token @operator = Previous();
            Expression right = Unary();
            expression = new BinaryExpression(expression, @operator, right);
        }

        return expression;
    }
    
    private Expression Unary()
    {
        if(Match(TokenType.Bang, TokenType.Minus))
        {
            Token @operator = Previous();
            Expression right = Unary();
            return new UnaryExpression(@operator, right);
        }
        return Primary();
    }
    
    private Expression Primary()
    {
        if (Match(TokenType.False)) return new LiteralExpression(false);
        if (Match(TokenType.True)) return new LiteralExpression(true);
        if (Match(TokenType.Nil)) return new LiteralExpression(null);

        if(Match(TokenType.Number, TokenType.String))
        {
            return new LiteralExpression(Previous().Literal);
        }

        if(Match(TokenType.LeftParen))
        {
            Expression expression = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new GroupingExpression(expression); 
        }

        throw Error(Peek(), "Expected expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        throw Error(Peek(), message);
    }
    
    private bool Match(params TokenType[] types)
    {
        if (types.Any(Check))
        {
            Advance();
            return true;
        }

        return false;
    }

    private bool Check(TokenType tokenType)
    {
        if (IsAtEnd())
        {
            return false;
        }
        return Peek().Type == tokenType;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }
        return Previous();
    }
    
    private ParserException Error(Token token, string message)
    {
        _errorHandler.Error(token, message);
        return new ParserException();
    }

    private void Synchronize()
    {
        Advance();

        while(!IsAtEnd())
        {
            if(Previous().Type == TokenType.Semicolon)
            {
                return;
            }

            switch(Peek().Type)
            {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Var:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return:
                    return;
            }

            Advance();
        }
    }
    
    private bool IsAtEnd() => _tokens[_current].Type == TokenType.EOF;

    private Token Peek() => _tokens[_current];

    private Token Previous() => _tokens[_current - 1];
}