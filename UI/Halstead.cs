using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public class Halsted
    {
        private static readonly string[] ServieceWords = {"label", "break","continue","debugger","const","let","var","from","where",
"select",
"function",
"qroup",
"orderby",
"ascending",
"descending",
"join",
"on",
"equals",
"by",
"switch"
,"case","default",
"await","do", "for", "if","else", "return","while"
 };
        private static readonly string[] NotUnarOp = { "++", "+++", "==", "===", "!=", "!==", "-=", "--", "**", "**=", "/=", "%=", "<<", "<<=", ">>", ">>>", ">>=", "??", "??=", "<=", ">=", "&&", "&=", "&&=", "*=", "||", "||=", "^=", "*", "+", "-", "=", "(", ";", "<", ">", "[", ",", ":", "/", "+=","$" };
        public static Dictionary<string, int> OperatorDictionary;
        public static Dictionary<string, int> OperandDictionary;
        public static void FillDictionaries(List<string> code)
        {
            OperatorDictionary = new Dictionary<string, int>();
            OperandDictionary = new Dictionary<string, int>();
            foreach (var line in code)
            {
                Cycle(line, line.Length);
            }
            OperatorDictionary.Remove(",");
            OperatorDictionary.Remove("case");
            OperatorDictionary.Remove("else");
            OperatorDictionary.Remove("default");
        }
        public static void Cycle(string line, int len)
        {
            string StringTmp;
            bool fl;
            for (int i = 0; i < line.Length; i++)
            {

                if (line[i] == ';')
                    if (OperatorDictionary.ContainsKey(";"))
                        OperatorDictionary[";"]++;
                    else
                        OperatorDictionary.Add((";"), 1);
                if (line[i] == ':')
                {
                    if (OperatorDictionary.ContainsKey(":"))
                        OperatorDictionary[":"]++;
                    else
                        OperatorDictionary.Add((":"), 1);
                    continue;
                }
                fl = true;
                StringTmp = "";
                if (line[i] != ' ' && line[i] != ']' && line[i] != ')' && line[i] != '}' && line[i] != '\'' && line[i] != ';' && line[i] != '\t')
                {
                    if (((line[i] < '0') || (line[i] > '9')) && ((line[i] < 'A') || (line[i] > 'Z')) && ((line[i] < 'a') || (line[i] > 'z')) && line[i] != '.' && line[i] != '_')
                    {
                        do
                        {
                            if (line[i] == '"')
                            {
                                do
                                    StringTmp += line[i++];
                                while (line[i] != '"');
                                StringTmp += line[i];
                                if (OperandDictionary.ContainsKey(StringTmp))
                                    OperandDictionary[StringTmp]++;
                                else
                                {
                                    OperandDictionary.Add((StringTmp), 1);
                                }
                                fl = false;
                                i++;
                                continue;
                            }
                            StringTmp += line[i];
                            if (StringTmp == "{")
                            {
                                if (OperatorDictionary.ContainsKey(StringTmp))
                                    OperatorDictionary[StringTmp]++;
                                else
                                {
                                    OperatorDictionary.Add((StringTmp), 1);
                                }
                                fl = false;
                            }
                            else
                            if (!NotUnarOp.Contains(StringTmp + line[i + 1]))
                                if (NotUnarOp.Contains(StringTmp))
                                {
                                    fl = false;
                                    if (OperatorDictionary.ContainsKey(StringTmp))
                                        OperatorDictionary[StringTmp]++;
                                    else
                                    {
                                        OperatorDictionary.Add((StringTmp), 1);
                                    }
                                }
                                else;
                            else
                            {
                                i++;
                                continue;
                            }
                        } while (fl);
                    }
                    else
                    {
                        do
                        {
                            if (line[i] == '[')
                            {
                                int j = i + 1;
                                string tmp = "";
                                while (line[j] != ']')
                                {
                                    tmp += line[j++];
                                }
                                Cycle(tmp, tmp.Length);
                                StringTmp += "[" + tmp;
                                i += 1 + tmp.Length;
                            }
                            StringTmp += line[i++];

                        }
                        while ((i < len) && (((line[i] >= '0') && (line[i] <= '9')) || ((line[i] >= 'A') && (line[i] <= 'Z')) || ((line[i] >= 'a') && (line[i] <= 'z')) || line[i] == '.' || line[i] == '_' || line[i] == '[' || line[i] == ']' || line[i] == '\\'));

                        if (line[i] == '(')
                            if (OperatorDictionary.ContainsKey(StringTmp))
                                OperatorDictionary[StringTmp]++;
                            else
                            {
                                OperatorDictionary.Add((StringTmp), 1);
                            }
                        else
                        if (!ServieceWords.Contains(StringTmp))
                            if (OperandDictionary.ContainsKey(StringTmp))
                                OperandDictionary[StringTmp]++;
                            else
                            {
                                OperandDictionary.Add((StringTmp), 1);
                            }
                        else
                            if (OperatorDictionary.ContainsKey(StringTmp))
                            OperatorDictionary[StringTmp]++;
                        else
                        {
                            OperatorDictionary.Add((StringTmp), 1);
                        }
                        i--;
                    }
                }
            }
        }

        public static void GetMetrics(out int dict, out int length, out int volume)
        {

            dict = OperatorDictionary.Count + OperandDictionary.Count;
            length = 0;
            foreach (var c in OperatorDictionary)
            {
                length += c.Value;
            }
            foreach (var c in OperandDictionary)
            {
                length += c.Value;
            }
            volume = (int)(length * Math.Log(dict) / Math.Log(2));
        }
    }
}
