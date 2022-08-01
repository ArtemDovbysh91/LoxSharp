var loxRunner = new LoxSharp.Lox();
        
if (args.Length > 1)
{
    Console.WriteLine("Invalid arguments count. Usage: jlox [script]");
    return -1;
}
else if (args.Length == 1)
{
    loxRunner.RunFile(args[0]);
}
else
{
    loxRunner.RunPrompt();
}

return loxRunner.HadError ? 60 : 0;