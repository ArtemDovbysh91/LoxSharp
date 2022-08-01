namespace GenerateAst;

public static class Generator
{
    public static void GenerateFile(string outputDirectory, string baseName)
    {
        var folder = Path.Combine(outputDirectory, "AST");

        Directory.CreateDirectory(folder);

        var file = Path.Combine(folder, $"{baseName}.cs");

        using (var writer = new StreamWriter(File.OpenWrite(file)))
        {
            foreach (var line in FormatOutput())
            {
                writer.WriteLine(line);
            }
        }
    }

    private static readonly Queue<string> output = new Queue<string>();

    private static void AppendLine(string? s = null)
    {
        output.Enqueue(s ?? string.Empty);
    }

    private static IEnumerable<string> FormatOutput()
    {
        var tab = "    ";
        var tabLevel = 0;

        while (output.Count > 0)
        {
            var line = output.Dequeue();
            string indent;

            switch (line)
            {
                case "{":
                    indent = tab.Repeat(tabLevel++);
                    break;
                case "}":
                    indent = tab.Repeat(--tabLevel);
                    break;
                case "":
                    indent = string.Empty;
                    break;
                default:
                    indent = tab.Repeat(tabLevel);
                    break;
            }

            yield return indent + line;
        }
    }

    public static void DefineAst(string baseName, IEnumerable<string> types)
    {
        AppendLine("// Generated code, do not modify.");
        AppendLine("using LoxSharp.Scanning;");
        AppendLine();
        AppendLine("namespace LoxSharp.AST");
        AppendLine("{");

        DefineVisitor(baseName, types);

        AppendLine();

        DefineTypes(baseName, types);

        AppendLine("}");
    }

    private static void DefineVisitor(string baseName, IEnumerable<string> types)
    {
        AppendLine("public interface IVisitor<out T>");
        AppendLine("{");

        foreach (var type in types)
        {
            var (typeName, _) = type.Split(':').Select(s => s.Trim());
            AppendLine($"T Visit{typeName}{baseName}({typeName}{baseName} {baseName.ToLower()});");
        }

        AppendLine("}");
    }

    private static void DefineTypes(string baseName, IEnumerable<string> types)
    {
        // base class
        AppendLine($"public abstract class {baseName}");
        AppendLine("{");
        AppendLine($"public abstract T Accept<T>(IVisitor<T> visitor);");
        AppendLine("}");

        // extension classes
        foreach (var type in types)
        {
            AppendLine();

            var (className, fields, _) = type.Split(':').Select(s => s.Trim());

            DefineType(baseName, className, fields);
        }
    }

    private static readonly Dictionary<string, string> keywordMap = new Dictionary<string, string>
    {
        { "operator", "op" }
    };

    private static string FilterKeywords(string str)
    {
        foreach (KeyValuePair<string, string> entry in keywordMap)
        {
            str = str.Replace(entry.Key, entry.Value);
        }

        return str;
    }

    private static void DefineType(string baseName, string className, string fields)
    {
        var fieldParts = fields.Split(',').Select(s => s.Trim());

        AppendLine($"public class {className}{baseName} : {baseName}");
        AppendLine("{");

        // fields
        foreach (var field in fieldParts)
        {
            var (type, name, _) = field.Split(' ');
            AppendLine($"public readonly {type} {name.ToUppercaseFirst()};");
        }

        AppendLine();

        // constructor
        AppendLine($"public {className}{baseName}({FilterKeywords(fields)})");
        AppendLine("{");
        foreach (var field in fieldParts)
        {
            var (_, name, _) = field.Split(' ');
            AppendLine($"{name.ToUppercaseFirst()} = {FilterKeywords(name)};");
        }
        AppendLine("}");

        AppendLine();

        // visitor pattern
        AppendLine($"public override T Accept<T>(IVisitor<T> visitor)");
        AppendLine("{");
        AppendLine($"return visitor.Visit{className}{baseName}(this);");
        AppendLine("}");

        AppendLine("}");
    }
}