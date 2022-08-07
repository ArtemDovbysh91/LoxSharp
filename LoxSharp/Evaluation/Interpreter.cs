using System.ComponentModel;
using LoxSharp.AST;
using LoxSharp.Exceptions;
using LoxSharp.Scanning;

namespace LoxSharp.Evaluation;

public class Interpreter : IVisitor<object>
{
    private readonly IErrorHandler _errorHandler;

    public Interpreter(IErrorHandler errorHandler)
    {
        _errorHandler = errorHandler;
    }
    
    public void Interpret(Expression expresssion)
    {
        try
        {
            object value = Evaluate(expresssion);
            Console.WriteLine(Stringify(value));
        }
        catch( RuntimeError error)
        {
            _errorHandler.RuntimeError(error);
        }
    }
    
    public object VisitBinaryExpression(BinaryExpression binary)
    {
        object right = Evaluate(binary.Right);
        object left = Evaluate(binary.Left);
        
        switch (binary.Operator.Type)
        {
            case TokenType.Greater:
                CheckNumberOperand(binary.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual:
                CheckNumberOperand(binary.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                CheckNumberOperand(binary.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual:
                CheckNumberOperand(binary.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.Minus:
                CheckNumberOperand(binary.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.Plus:
                if (left is double leftDouble && right is double rightDouble)
                {
                    return leftDouble + rightDouble;
                }
                else if (left is string leftStr && right is string rightStr)
                {
                    return leftStr + rightStr;
                }
                
                _errorHandler.RuntimeError(new RuntimeError(binary.Operator, "Operands must be two numbers or two strings."));
                break;
            case TokenType.Slash:
                CheckNumberOperand(binary.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.Star:
                CheckNumberOperand(binary.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.BangEqual:
                return !IsEqual(left, right);
            case TokenType.EqualEqual:
                return IsEqual(left, right);

        }

        // Unreachable. 
        return null;
    }

    public object VisitGroupingExpression(GroupingExpression grouping)
    {
        return Evaluate(grouping.Expression);
    }

    public object VisitLiteralExpression(LiteralExpression literal)
    {
        return literal.Value;
    }

    public object VisitUnaryExpression(UnaryExpression unary)
    {
        object right = Evaluate(unary.Right);

        switch (unary.Operator.Type) {
            case TokenType.Minus:
                CheckNumberOperand(unary.Operator, right);
                return -(double)right;
            case TokenType.Bang:
                return !IsTruthy(right);
            // default:
            //     throw new ArgumentOutOfRangeException();
        }

        // Unreachable.
        return null;
    }
    
    private object Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }
    
    private static bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool boolValue => boolValue,
            _ => true
        };
    }
    
    private static bool IsEqual(object? a, object? b) {
        if (a == null && b == null) 
            return true;
        
        if (a == null) return false;

        return Equals(a, b);
    }
    
    private static void CheckNumberOperand(Token @operator, object operand) {
        if (operand is double) return;
        throw new RuntimeError(@operator, "Operand must be a number");
    }
    
    private void CheckNumberOperand(Token @operator, object right, object left)
    {
        if (right is double && left is double) return;
        throw new RuntimeError(@operator, "Operands must be a number");
    }
    
    private string Stringify(object? value) => value?.ToString() ?? "nil";
}