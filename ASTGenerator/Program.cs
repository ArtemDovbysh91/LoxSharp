using GenerateAst;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: GenerateAst <output directory>)");
    Environment.Exit(1);
}

var outputDirectory = args[0];
var baseName = "Expression";

Generator.DefineAst(baseName, new string[]
{
    "Binary     : Expression left, Token operator, Expression right",
    "Grouping   : Expression expression"  ,
    "Literal    : object value",
    "Unary      : Token operator, Expression right"
});

Generator.GenerateFile(outputDirectory, baseName);