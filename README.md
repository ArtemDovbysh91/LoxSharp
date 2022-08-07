# LoxSharp
A c# port of the Lox tree-walking interpreter from Bob Nystrom’s Crafting Interpreters book. This repository is made to stay close to the original examples from the book's code so everyone easily can refer to the Crafting Interpreters book for explanations. Nonetheless, to get jlox working in Csharp some differences are unavoidable.

The current implementation includes language scanning, parsing, and execution. It can accept a file or console input read in a circle, line by line. 

The ASTGeneration project is used for Expression.cs file creation. [Chapter 5](http://craftinginterpreters.com/representing-code.html) fully explains why and how it works.
