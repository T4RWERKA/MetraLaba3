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
            var vars = Laba3Console.Program.getVars();
            Halsted.FillDictionaries(code);
            string? txt = "";
            int fullspen = 0;
            double fullchp = 0;
            double partchp = 0;
            foreach (string str in code) txt += str;
                Laba3Console.Program.Cycle(txt);
            foreach (var str in vars)
            {
                tb3.Text += $"{str.Key}: {str.Value.group}, IO: {str.Value.isIO} , spen: {Halsted.OperandDictionary[str.Key] - 1}" + '\n';
                fullspen += Halsted.OperandDictionary[str.Key] - 1;
                switch (str.Value.group)
                {
                    case Group.P:
                        fullchp++;
                        if (str.Value.isIO)
                            partchp++;
                        break;
                    case Group.M:
                        fullchp += 2;
                        if (str.Value.isIO)
                            partchp += 2;
                        break;
                    case Group.C:
                        fullchp += 3;
                        if (str.Value.isIO)
                            partchp += 3;
                        break;
                    case Group.T:
                        fullchp += 0.5;
                        if (str.Value.isIO)
                            partchp += 0.5;
                        break;
                }
            }
            tb2.Text = $"{fullchp}";
            tb4.Text = $"{partchp}";
            tb5.Text = $"{fullspen}";

        }
    }
}