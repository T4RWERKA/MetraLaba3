using ConsoleApp1;
using System.ComponentModel.Design;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Laba3Console
{
    public class Program
    {
        public static readonly HashSet<string> reservedNames = new HashSet<string>{ "function", "number", "string", 
            "boolean", "any", "Array", "void", "let", "return", "const", "for", "while", "if", "else", "switch", "case", 
            "default", "break", "on" };
        
        // регулярное выражение для идентификатора переменной (исключает функции и атрибуты объектов)
        public static readonly string identifierTemplate = @"(?<!['"".])\b(?<name>[a-zA-Z_][a-zA-Z0-9_]*)(?![\w(])\b";
        // public static readonly string identifierTemplate = @"(?<name>[a-zA-Z_][a-zA-Z0-9_]*)(?![\w(])";
        public enum Group
        {
            // приоритеты групп
            P = 2,
            M = 1,
            T = 3,
            C = 4
        }
        public class Variable
        {
            public Group group;
            public bool used;
            public bool isIO;
            public Variable(Group group, bool used = false, bool isIO = false)
            {
                this.group = group;
                this.used = used;
                this.isIO = isIO;
            }
        }
        public static Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        public static Dictionary<string, Variable> getVars() { return variables; }
        public static HashSet<string> GetIdentifiers(string code)
        {
            Regex regex = new Regex(identifierTemplate);
            MatchCollection matches = regex.Matches(code);
            var result = new HashSet<string>();

            foreach (Match match in matches)
            {
                if (!reservedNames.Contains(match.Value))
                {
                    result.Add(match.Value);
                }
            }
            return result;
        }

        public static void Cycle(string code)
        {
            Console.WriteLine(code);
            Console.WriteLine("-----------------");

            for (int i = 0; i < code.Length; i++)
            {
                if (i < code.Length - "on".Length && code.Substring(i, "on".Length) == "on" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "on".Length]) ||
                    i < code.Length - "question".Length && code.Substring(i, "question".Length) == "question" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "question".Length]))
                {
                    while (code[i] != '(') i++;
                    bool noBreak;
                    int closeBracket = Jilb.CloseBracketInd(code, i, "(", out noBreak);
                    HashSet<string> var_C = GetIdentifiers(code.Substring(i + 1, closeBracket - i - 1));
                    foreach (var identifier in var_C)
                        variables[identifier] = new Variable(Group.P, true, true);
                    i = closeBracket;
                }
                // Управляющие (группа С) переменные
                else if (i < code.Length - "if".Length && code.Substring(i, "if".Length) == "if" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "if".Length]) ||
                    i < code.Length - "while".Length && code.Substring(i, "while".Length) == "while" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "while".Length]) ||
                    i < code.Length - "for".Length && code.Substring(i, "for".Length) == "for" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "for".Length]) ||
                    i < code.Length - "switch".Length && code.Substring(i, "switch".Length) == "switch" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "switch".Length]))
                {
                    while (code[i] != '(') i++;
                    bool noBreak;
                    int closeBracket = Jilb.CloseBracketInd(code, i, "(", out noBreak);

                    HashSet<string> var_C = GetIdentifiers(code.Substring(i + 1, closeBracket - i - 1));
                    foreach (var identifier in var_C)
                        variables[identifier] = new Variable(Group.C, true);
                    i = closeBracket;
                }  
                else if (i < code.Length - "case".Length && code.Substring(i, "case".Length) == "case" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "case".Length]))
                {
                    int j = i;
                    while (code[j] != ':') j++;

                    HashSet<string> var_C = GetIdentifiers(code.Substring(i, j - i));
                    foreach (var variable in var_C)
                        variables[variable] = new Variable(Group.C, true);
                    i = j;
                }
                // Модифицируемые переменные (группа M)
                // ищу операции присваивания. То, что слева от них - группа M
                else
                {
                    HashSet<string> assignment_opers = new HashSet<string>() { "+=", "-=", "*=", "/=", "^=", "|=", "&=" };
                        if (i < code.Length - "+=".Length && assignment_opers.Contains(code.Substring(i, "+=".Length)) ||
                        code[i] == '=' && !new HashSet<char>(){ '<', '>', '=' }.Contains(code[i - 1]) && code[i + 1] != '=')
                    {
                        // переменная, которой присваивается значение
                        int j = i;

                        while (j > 0 && code[j] != ';' && code[j] != ':' && code[j] != '{' && code[j] != '}' && code[j] != ',')
                        {
                            j--;

                            // если при просмотре назад встретилось ':', это либо case, либо указание типа данных
                            if (code[j] == ':')
                            {
                                int k = j;
                                while (!(code.Substring(k, "let ".Length) == "let " || code.Substring(k, "case ".Length) == "case ")) k--;

                                if (code.Substring(k, "let ".Length) == "let ")
                                    j = k + "let ".Length;
                            }
                        }

                        HashSet<string> var_M = GetIdentifiers(code.Substring(j, i - j - 1));
                        Console.WriteLine("левая: " + code.Substring(j, i - j - 1));
                        foreach (var identifier in var_M)
                            if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                variables[identifier] = new Variable(Group.M, false);

                        // присваиваемые переменные (перестают быть паразитными)
                        j = i;
                        uint funcCall = 0; // для проверки аргументов функций
                        uint brackCount = 0;
                        while (code[j] != ';' && ((code[j] != ',' || code[j] == ',' && funcCall > 0)))
                        {
                            if (code[j + 1] == '(')
                            {
                                if (char.IsLetter(code[j]))
                                    funcCall++;
                                else
                                    brackCount++;
                            }
                            else if (code[j] == ')')
                            {
                                if (brackCount > 0)
                                    brackCount--;
                                else
                                    funcCall--;
                            }
                            j++;
                        }
                            HashSet<string> var_notT = GetIdentifiers(code.Substring(i + 1, j - i - 1));
                            Console.WriteLine("правая: " + code.Substring(i + 1, j - i - 1));
                            foreach (var identifier in var_notT)
                                if (variables.ContainsKey(identifier))
                                {
                                    variables[identifier].used = true;
                                }
                                else
                                {
                                    Console.WriteLine("используется необъявленная переменная: " + identifier);
                                    variables[identifier] = new Variable(Group.M, true);
                                }
                        i = j;
                    } 
                    // инкремент / декремент
                    else if (i < code.Length - "++".Length && code.Substring(i, "++".Length) == "++" ||
                        i < code.Length - "--".Length && code.Substring(i, "--".Length) == "--")
                    {

                        int j = i;
                        // постфиксный
                        if (char.IsLetter(code[i - 1]))
                        {
                            while (j > 0 && code[j] != ';' && code[j] != ':' && code[j] != '{' && code[j] != '}' && code[j] != ',')
                            {
                                j--;

                                // если при просмотре назад встретилось ':', это либо case, либо указание типа данных
                                if (code[j] == ':')
                                {
                                    int k = j;
                                    while (!(code.Substring(k, "let ".Length) == "let " || code.Substring(k, "case ".Length) == "case ")) k--;

                                    if (code.Substring(k, "let ".Length) == "let ")
                                        j = k + "let ".Length;
                                }
                            }

                            HashSet<string> var_M = GetIdentifiers(code.Substring(j, i - j));
                            foreach (var identifier in var_M)
                                if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                    variables[identifier] = new Variable(Group.M, false);
                        }
                        // префиксный
                        else
                        {
                            uint funcCall = 0; // для проверки аргументов функций
                            uint brackCount = 0;
                            while (code[j] != ';' && ((code[j] != ',' || code[j] == ',' && funcCall > 0)))
                            {
                                if (code[j + 1] == '(')
                                {
                                    if (char.IsLetter(code[j]))
                                        funcCall++;
                                    else
                                        brackCount++;
                                }
                                else if (code[j] == ')')
                                {
                                    if (brackCount > 0)
                                        brackCount--;
                                    else
                                        funcCall--;
                                }
                                j++;
                            }

                            HashSet<string> var_M = GetIdentifiers(code.Substring(i + 1, j - i));
                            foreach (var identifier in var_M)
                                if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                    variables[identifier] = new Variable(Group.M, false);
                            i = j;
                        }
                    }
                    // параметры функции
                    else if (i < code.Length - "function".Length && code.Substring(i, "function".Length) == "function")
                    {
                        int j = i;
                        while (code[j] != '(') j++;
                        bool noBreak;
                        int closeBracket = Jilb.CloseBracketInd(code, j, "(", out noBreak);

                        HashSet<string> var_M = GetIdentifiers(code.Substring(j + 1, closeBracket - j - 1));
                        foreach (var identifier in var_M)
                            if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                variables[identifier] = new Variable(Group.M, false);
                        i = closeBracket;
                    }
                    // вызов процедуры делает аргументы непаразитными
                    else if (i > 0 && code[i] == '(' && char.IsLetter(code[i - 1]))
                    {
                        bool noBreak;
                        int closeBracket = Jilb.CloseBracketInd(code, i, "(", out noBreak);
                        HashSet<string> var_M = GetIdentifiers(code.Substring(i + 1, closeBracket - i - 1));
                        foreach (var identifier in var_M)
                            if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                variables[identifier] = new Variable(Group.M, true);

                        // помечаю IO те, которые в аргументах condole.log
                        int j = i - 1;
                        while (j > 0 && (char.IsLetter(code[j]) || code[j] == '.')) j--;
                        if (code.Substring(j, i - j).Contains("console.log"))
                        {
                            j = i + 1;
                            int funcCall = 1, prevFuncCall = 1, brackCount = 0;
                            int start = j, stop;
                            while (funcCall >= 0)
                            {
                                if (funcCall > 1 && prevFuncCall == 1 || funcCall == 0)
                                {
                                    prevFuncCall = funcCall;
                                    stop = j;
                                    HashSet<string> var_IO = GetIdentifiers(code.Substring(start, stop - start + 1));
                                    foreach (var identifier in var_IO)
                                        variables[identifier].isIO = true;
                                    if (funcCall == 0)
                                        funcCall--;
                                }
                                if (funcCall > 0)
                                {
                                    if (funcCall == 1 && prevFuncCall > 1)
                                    {
                                        prevFuncCall = funcCall;
                                        start = j;
                                    }
                                    if (code[j + 1] == '(')
                                    {
                                        if (char.IsLetter(code[j]))
                                            funcCall++;
                                        else
                                            brackCount++;
                                    }
                                    else if (code[j] == ')')
                                    {
                                        if (brackCount > 0)
                                            brackCount--;
                                        else
                                            funcCall--;
                                    }
                                    j++;
                                }
                            }
                        }
                        i = closeBracket;
                    }
                    else if (i < code.Length - "return".Length && code.Substring(i, "return".Length) == "return" && (i > 0 ? !char.IsLetter(code[i - 1]) : true) && !char.IsLetter(code[i + "return".Length]))
                    {
                        int j = i;
                        while (code[j] != ';') j++;

                        HashSet<string> var_M = GetIdentifiers(code.Substring(i, j - i));
                        foreach (var identifier in var_M)
                            if (variables.ContainsKey(identifier))
                            {
                                variables[identifier].used = true;
                            }
                            else
                            {
                                Console.WriteLine("используется необъявленная переменная: " + identifier);
                                variables[identifier] = new Variable(Group.M, true);
                            }
                        i = j;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            foreach (var variable in variables)
                if (!variable.Value.used)
                    variable.Value.group = Group.T;

            foreach (var variable in variables)
                Console.WriteLine($"{variable.Key}: {variable.Value.group}, IO: {variable.Value.isIO}");
        }
    }
}