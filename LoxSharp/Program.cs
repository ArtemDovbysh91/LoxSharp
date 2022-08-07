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

if(loxRunner.HadError)
{
    return 60;
}

if(loxRunner.HadRuntimeError)
{
    return 70;
}
return 0;