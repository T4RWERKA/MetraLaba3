namespace UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb1 = new RichTextBox();
            bt1 = new Button();
            bt2 = new Button();
            openFileDialog1 = new OpenFileDialog();
            tb2 = new TextBox();
            tb3 = new RichTextBox();
            label1 = new Label();
            label2 = new Label();
            tb4 = new TextBox();
            label3 = new Label();
            tb5 = new TextBox();
            SuspendLayout();
            // 
            // tb1
            // 
            tb1.Location = new Point(657, 12);
            tb1.Name = "tb1";
            tb1.Size = new Size(306, 490);
            tb1.TabIndex = 0;
            tb1.Text = "";
            // 
            // bt1
            // 
            bt1.Location = new Point(657, 508);
            bt1.Name = "bt1";
            bt1.Size = new Size(126, 43);
            bt1.TabIndex = 1;
            bt1.Text = "Файл";
            bt1.UseVisualStyleBackColor = true;
            bt1.Click += bt1_Click;
            // 
            // bt2
            // 
            bt2.Location = new Point(805, 507);
            bt2.Name = "bt2";
            bt2.Size = new Size(118, 44);
            bt2.TabIndex = 2;
            bt2.Text = "Расчет";
            bt2.UseVisualStyleBackColor = true;
            bt2.Click += bt2_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // tb2
            // 
            tb2.Location = new Point(142, 356);
            tb2.Name = "tb2";
            tb2.Size = new Size(60, 23);
            tb2.TabIndex = 3;
            // 
            // tb3
            // 
            tb3.Location = new Point(21, 12);
            tb3.Name = "tb3";
            tb3.Size = new Size(301, 313);
            tb3.TabIndex = 4;
            tb3.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(43, 359);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 5;
            label1.Text = "Полная Чепина";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 399);
            label2.Name = "label2";
            label2.Size = new Size(127, 15);
            label2.TabIndex = 6;
            label2.Text = "Чепина ввода/вывода";
            // 
            // tb4
            // 
            tb4.Location = new Point(142, 396);
            tb4.Name = "tb4";
            tb4.Size = new Size(60, 23);
            tb4.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(328, 15);
            label3.Name = "label3";
            label3.Size = new Size(104, 15);
            label3.TabIndex = 8;
            label3.Text = "Cуммарный спен";
            // 
            // tb5
            // 
            tb5.Location = new Point(438, 12);
            tb5.Name = "tb5";
            tb5.Size = new Size(100, 23);
            tb5.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(975, 563);
            Controls.Add(tb5);
            Controls.Add(label3);
            Controls.Add(tb4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tb3);
            Controls.Add(tb2);
            Controls.Add(bt2);
            Controls.Add(bt1);
            Controls.Add(tb1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox tb1;
        private Button bt1;
        private Button bt2;
        private OpenFileDialog openFileDialog1;
        private TextBox tb2;
        private RichTextBox tb3;
        private Label label1;
        private Label label2;
        private TextBox tb4;
        private Label label3;
        private TextBox tb5;
    }
}