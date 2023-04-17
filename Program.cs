using ConsoleApp1;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Laba3Console
{
    internal class Program
    {
        public static readonly HashSet<string> reservedNames = new HashSet<string>{ "function", "number", "let", "return",
            "const", "for", "while", "if", "else", "switch", "case", "default", "break" };
        public static readonly string identifierTemplate = @"(?<name>[a-zA-Z_][a-zA-Z0-9_]*)(?![\w(])";
        enum Group
        {
            P = 1,
            M = 2,
            T = 3,
            C = 3
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
                fstream = new FileStream(@"D:\Study\Metra\Задание№3_МетрикиПотокаДанных\Laba3Console\Laba3Console\code.txt", FileMode.Open);
                byte[] buffer = new byte[fstream.Length];
                fstream.ReadAsync(buffer, 0, buffer.Length);
                code = Encoding.Default.GetString(buffer);
            }
            finally
            {
                fstream?.Close();
            }

            Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

            Console.WriteLine(code.Length);

            var var_C = new HashSet<string>();
            for (int i = 0; i < code.Length; i++)
            {
                if (i < code.Length - " if".Length && code.Substring(i, " if".Length) == " if" ||
                    i < code.Length - " while".Length && code.Substring(i, " while".Length) == " while" ||
                    i < code.Length - " for".Length && code.Substring(i, " for".Length) == " for" ||
                    i < code.Length - " switch".Length && code.Substring(i, " switch".Length) == " switch")
                {
                    while (code[i] != '(') i++;
                    bool noBreak;
                    int closeBracket = Jilb.CloseBracketInd(code, i, "(", out noBreak);
                    var_C.UnionWith(GetIdentifiers(code.Substring(i + 1, closeBracket - i - 1)));
                } 
                else if (i < code.Length - " case".Length && code.Substring(i, " case".Length) == " case")
                {
                    int j = i;
                    while (code[j] != ':') j++; 
                    var_C.UnionWith(GetIdentifiers(code.Substring(i + 1, j - i - 1)));
                }
            }

            foreach (var var in var_C) 
            {
                Console.WriteLine(var);
            }

            /*Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Regex regex = new Regex(identifierTemplate);
            MatchCollection matches = regex.Matches("i == 1");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                    Console.WriteLine(match.Groups["name"].Value);
            }
            else
            {
                Console.WriteLine("Совпадений не найдено");
                Console.WriteLine(Regex.IsMatch("", identifierTemplate));
            }*/
        }
    }
}