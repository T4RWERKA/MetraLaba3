using ConsoleApp1;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Laba3Console
{
    internal class Program
    {
        public static readonly HashSet<string> reservedNames = new HashSet<string>{ "function", "number", "string", 
            "boolean", "any", "Array", "void", "let", "return", "const", "for", "while", "if", "else", "switch", "case", 
            "default", "break" };
        
        // регулярное выражение для идентификатора переменной (исключает функции и атрибуты объектов)
        public static readonly string identifierTemplate = @"(?<!\.)\b(?<name>[a-zA-Z_][a-zA-Z0-9_]*)(?![\w(])";
        // public static readonly string identifierTemplate = @"(?<name>[a-zA-Z_][a-zA-Z0-9_]*)(?![\w(])";
        enum Group
        {
            // приоритеты групп
            P = 1,
            M = 2,
            T = 3,
            C = 4
        }
        class Variable
        {
            public Group group;
            public bool used;
            public Variable(Group group, bool used = false)
            {
                this.group = group;
                this.used = used;
            }
        }

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
        static void Main(string[] args)
        {


            FileStream? fstream = null;
            string? code;
            try
            {
                // поменяй путь
                fstream = new FileStream(@"D:\Study\Metra\func.txt", FileMode.Open);
                byte[] buffer = new byte[fstream.Length];
                fstream.ReadAsync(buffer, 0, buffer.Length);
                code = Encoding.Default.GetString(buffer);
            }
            finally
            {
                fstream?.Close();
            }

            Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

            Console.WriteLine(code);
            Console.WriteLine("-----------------");

            for (int i = 0; i < code.Length; i++)
            {
                // Управляющие (группа С) переменные
                if (i < code.Length - " if".Length && code.Substring(i, " if".Length) == " if" ||
                    i < code.Length - " while".Length && code.Substring(i, " while".Length) == " while" ||
                    i < code.Length - " for".Length && code.Substring(i, " for".Length) == " for" ||
                    i < code.Length - " switch".Length && code.Substring(i, " switch".Length) == " switch")
                {
                    while (code[i] != '(') i++;
                    bool noBreak;
                    int closeBracket = Jilb.CloseBracketInd(code, i, "(", out noBreak);

                    HashSet<string> var_C = GetIdentifiers(code.Substring(i + 1, closeBracket - i - 1));
                    foreach (var identifier in var_C)
                        variables[identifier] = new Variable(Group.C, true);
                    i = closeBracket;
                }  
                else if (i < code.Length - " case".Length && code.Substring(i, " case".Length) == " case")
                {
                    int j = i;
                    while (code[j] != ':') j++;

                    HashSet<string> var_C = GetIdentifiers(code.Substring(i + 1, j - i - 1));
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
                        // Console.WriteLine("левая: " + code.Substring(j, i - j - 1));
                        foreach (var identifier in var_M)
                            if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                variables[identifier] = new Variable(Group.M, false);

                        // присваиваемые переменные (перестают быть паразитными)
                        j = i;
                        while (code[j] != ';' && code[j] != ',') j++;

                        HashSet<string> var_notT = GetIdentifiers(code.Substring(i + 1, j - i - 1));
                        // Console.WriteLine("правая: " + code.Substring(i + 1, j - i - 1));
                        foreach (var identifier in var_notT)
                            if (variables.ContainsKey(identifier))
                            {
                                variables[identifier].used = true;
                            }
                            else
                            {
                                Console.WriteLine("используется необъявленная переменная: " + identifier);
                                variables[identifier] = new Variable(Group.M, false);
                            }
                    } 
                    // инкремент / декремент
                    else if (i < code.Length - "++".Length && code.Substring(i, "++".Length) == "++" ||
                        i < code.Length - "--".Length && code.Substring(i, "--".Length) == "--")
                    {

                        int j = i;
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
                        else
                        {
                            while (code[j] != ';' && code[j] != ',') j++;
                            HashSet<string> var_M = GetIdentifiers(code.Substring(i + 1, j - i));
                            foreach (var identifier in var_M)
                                if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                    variables[identifier] = new Variable(Group.M, false);
                        }
                    }
                    // параметры функции
                    else if (i < code.Length - "function".Length && code.Substring(i, "function".Length) == "function")
                    {
                        int j = i;
                        while (code[j] != '(') j++;
                        bool noBreak;
                        int closeBracket = Jilb.CloseBracketInd(code, i, "(", out noBreak);

                        HashSet<string> var_M = GetIdentifiers(code.Substring(j + 1, closeBracket - j - 1));
                        foreach (var identifier in var_M)
                            if (!(variables.ContainsKey(identifier) && variables[identifier].group > Group.M))
                                variables[identifier] = new Variable(Group.M, false);
                        i = closeBracket;
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
                Console.WriteLine($"{variable.Key}: {variable.Value.group}");
        }
    }
}