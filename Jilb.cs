using System;
using System.IO;

namespace ConsoleApp1
{
    public static class Jilb
    {
        public static int CloseBracketInd(string str, int begin, string open, out bool noBreak)
        {
            string close;
            noBreak = false;
            switch (open)
            {
                case "(":
                    close = ")";
                    break;
                case "{":
                    close = "}";
                    break;
                case "case":
                    close = "break";
                    break;
                default:
                    return -1;
            }
            int nestLvl = 1;
            int i = begin;
            while (nestLvl != 0)
            {
                i++;
                if (str.Length - i >= "switch".Length && str.Substring(i, "switch".Length).Equals("switch"))
                {
                    while (str[i] != '{') i++;
                    i = CloseBracketInd(str, i, "{", out noBreak);
                }
                else if (str.Length - i >= open.Length && str.Substring(i, open.Length).Equals(open))
                {
                    if (open.Equals("case"))
                    {
                        nestLvl--;
                        noBreak = true;
                    }
                    else
                    {
                        nestLvl++;
                    }
                }
                else if (str.Length - i >= close.Length && str.Substring(i, close.Length).Equals(close))
                    nestLvl--;
                else if (str.Length <= i)
                {
                    noBreak = true;
                    return i - 1;
                }
            }
            return i;
        }
        public static int MaxNestLvl(string code)
        {
            int maxNestLvl = -1, nestLvl = -1;
            bool noBreak;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '\"')
                {
                    i++;
                    while (!(code[i] == '\"' && code[i - 1] != '\\')) i++;
                }
                if ((code.Length - i >= "if".Length && code.Substring(i, "if".Length).Equals("if") && (code[i + "if".Length] == ' ' || code[i + "if".Length] == '(')) ||
                    (code.Length - i >= "else".Length && code.Substring(i, "else".Length).Equals("else") && (code[i + "else".Length] == ' ' || code[i + "else".Length] == '{')) ||
                    (code.Length - i >= "while".Length && code.Substring(i, "while".Length).Equals("while") && (code[i + "while".Length] == ' ' || code[i + "while".Length] == '{')) ||
                    (code.Length - i >= "for".Length && code.Substring(i, "for".Length).Equals("for") && (code[i + "for".Length] == ' ' || code[i + "for".Length] == '{')))
                {
                    if (code.Substring(i, "if".Length) == "if" ||
                        code.Substring(i, "while".Length) == "while" ||
                        code.Substring(i, "for".Length) == "for")
                    {
                        while (code[i] != '(') i++;
                        i = CloseBracketInd(code, i, "(", out noBreak);
                    }
                    while (code[i] != '{') i++;
                    nestLvl = 1 + MaxNestLvl(code.Substring(i + 1, CloseBracketInd(code, i, "{", out noBreak) - (i + 1)));
                    i = CloseBracketInd(code, i, "{", out noBreak) + 1;
                    if (nestLvl > maxNestLvl)
                        maxNestLvl = nestLvl;
                    nestLvl = -1;
                }
                else if (code.Length - i >= "switch".Length && code.Substring(i, "switch".Length).Equals("switch") && (code[i + "switch".Length] == ' ' || code[i + "switch".Length] == '('))
                {
                    while (code[i] != '{') i++;
                    int loop_end = CloseBracketInd(code, i, "{", out noBreak);
                    while (i <= loop_end)
                    {
                        if (code.Length - i >= "case".Length && code.Substring(i, "case".Length).Equals("case") && (code[i + "case".Length] == ' ' || code[i + "case".Length] == '('))
                        {
                            int j;
                            nestLvl++;
                            i += "case".Length;
                            j = CloseBracketInd(code, i, "case", out noBreak);
                            int nestLvlTmp = nestLvl + MaxNestLvl(code.Substring(i + 1, j - i)) + 1;
                            if (noBreak)
                                nestLvl = nestLvlTmp;
                            else
                                i = j + "break".Length;
                            if (nestLvlTmp > maxNestLvl)
                                maxNestLvl = nestLvlTmp;
                        }
                        i++;
                    }
                    nestLvl = -1;
                }
            }
            return maxNestLvl;
        }
    }
    /*internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new StreamReader("D:\\Study\\Metra\\Задание№2_МетрикиПотокаУправления\\code_switch.mts");
            string code;
            try
            {
                code = reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
            }
            Console.WriteLine(Jilb.MaxNestLvl(code));
        }
    }*/
}