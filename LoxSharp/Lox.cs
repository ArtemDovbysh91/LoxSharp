using LoxSharp.AST;
using LoxSharp.Scanning;

namespace LoxSharp;

public class Lox : IErrorHandler
{
    public bool HadError = false;
    
    public void RunFile(string path)
    {
        // Load the text
        string voxScript = File.ReadAllText(path);
        // Run it.
        Execute(voxScript);
    }
    
    public void RunPrompt()
    {
        do
        {
            // Write the input mark
            Console.Write(">");

            if (!Console.KeyAvailable)
            {
                // Read the input
                string? voxCode = Console.ReadLine();
                // Run it
                Execute(voxCode);

                HadError = false;
            }

        } while (!ShouldEscape());
    }
    
    public void Error(int line, string message)
    {
        Report(line, string.Empty, message);
    }
    
    public void Error(Token token, string message)
    {
        HadError = true;
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }
    
    private bool ShouldEscape()
    {
        ConsoleKeyInfo key = Console.ReadKey(true);
        return key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control;
    }
    
    private void Execute(string loxSource)
    {
        var scanner = new Scanner(loxSource, this);
        List<Token> tokens = scanner.ScanTokens();

        var parser = new Parser.Parser(tokens, this);
        // Start the parse process.
        var expression = parser.Parse();

        if(!HadError)
        {
            Debug.Log(new AstPrinter().Print(expression));
        }
    }

    private void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        HadError = true;
    }
}