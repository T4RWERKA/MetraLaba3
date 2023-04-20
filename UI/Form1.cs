using System.Reflection.Emit;
using Laba3Console;
using static Laba3Console.Program;

namespace UI
{
    public partial class Form1 : Form
    {
        public List<string> code = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }
        private void bt1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var reader = new StreamReader(openFileDialog1.FileName);
                code = new List<string>();
                try
                {
                    while (!reader.EndOfStream)
                        code.Add(reader.ReadLine());
                }
                finally
                {
                    reader.Close();
                }
                tb1.Text = "";
                foreach (var str in code)
                    tb1.Text += str + "\n";
            }
        }

        private void bt2_Click(object sender, EventArgs e)
        {
            tb2.Text = "";
            tb3.Text = "";
            tb6.Text = "";
            var vars = Laba3Console.Program.getVars();
            Halsted.FillDictionaries(code);
            string? txt = "";
            int fullspen = 0;
            double fullchp = 0, partchp = 0;
            foreach (string str in code) txt += str;
            Laba3Console.Program.Cycle(txt);
            foreach (var str in vars)
            {
                fullspen += Halsted.OperandDictionary[str.Key] - 1;
                switch (str.Value.group)
                {
                    case Group.P:
                        fullchp++;
                        tb6.Text += $"{str.Key} Полная Чепина:+1\n";
                        if (str.Value.isIO)
                        {
                            partchp++;
                            tb6.Text += $"{str.Key} IO:+1\n";
                        }
                        break;
                    case Group.M:
                        fullchp += 2;
                        tb6.Text += $"{str.Key} Полная Чепина:+2\n";
                        if (str.Value.isIO)
                        {
                            partchp+=2;
                            tb6.Text += $"{str.Key} IO:+2\n";
                        }
                        break;
                    case Group.C:
                        fullchp += 3;
                        tb6.Text += $"{str.Key} Полная Чепина:+3\n";
                        if (str.Value.isIO)
                        {
                            partchp+=3;
                            tb6.Text += $"{str.Key} IO:+3\n";
                        }
                        break;
                    case Group.T:
                        fullchp += 0.5;
                        tb6.Text += $"{str.Key} Полная Чепина:+0.5\n";
                        if (str.Value.isIO)
                        {
                            partchp++;
                            tb6.Text += $"{str.Key} IO:+0.5\n";
                        }
                        break;
                }
                tb3.Text += $"{str.Key}: {str.Value.group}, IO: {str.Value.isIO} , spen: {Halsted.OperandDictionary[str.Key] - 1}" + '\n';
            }
            tb2.Text = $"{fullchp}";
            tb4.Text = $"{partchp}";
            tb5.Text = $"{fullspen}";
            Laba3Console.Program.variables.Clear();
            Halsted.OperandDictionary.Clear();
        }
    }
}