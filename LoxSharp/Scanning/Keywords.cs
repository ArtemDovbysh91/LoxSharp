﻿namespace LoxSharp.Scanning;

public static class Keywords
{
    public static readonly Dictionary<string, TokenType> MAP;

    public const string AND = "and";
    public const string CLASS = "class";
    public const string ELSE = "else";
    public const string FALSE = "false";
    public const string FOR = "for";
    public const string FUN = "fun";
    public const string IF = "if";
    public const string NIL = "nil";
    public const string OR = "or";
    public const string PRINT = "print";
    public const string RETURN = "return";
    public const string SUPER = "super";
    public const string THIS = "this";
    public const string TRUE = "true";
    public const string VAR = "var";
    public const string WHILE = "while";
    
    static Keywords()
    {
        MAP = new Dictionary<string, TokenType>();
        MAP[AND] = TokenType.And;
        MAP[CLASS] = TokenType.Class;
        MAP[ELSE] = TokenType.Else;
        MAP[FALSE] = TokenType.False;
        MAP[FOR] = TokenType.For;
        MAP[FUN] = TokenType.Fun;
        MAP[IF] = TokenType.If;
        MAP[NIL] = TokenType.Nil;
        MAP[OR] = TokenType.Or;
        MAP[PRINT] = TokenType.Print;
        MAP[RETURN] = TokenType.Return;
        MAP[SUPER] = TokenType.Super;
        MAP[THIS] = TokenType.This;
        MAP[TRUE] = TokenType.True;
        MAP[VAR] = TokenType.Var;
        MAP[WHILE] = TokenType.While;
    }
    
    public static TokenType Get(string key)
    {
        if(MAP.ContainsKey(key))
        {
            return MAP[key];
        }
        return TokenType.Undefined;
    }
}