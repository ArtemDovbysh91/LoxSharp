using LoxSharp.Exceptions;
using LoxSharp.Scanning;

namespace LoxSharp;

public interface IErrorHandler
{
    void Error(int line, string message);
    void Error(Token token, string message);
    void RuntimeError(RuntimeError error);
}