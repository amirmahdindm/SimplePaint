using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Paint
{
    public partial class Add_string : Form
    {
        public Add_string(Font font,string text)
        {
            InitializeComponent();
            textBox1.Font= fontDialog1.Font = font;
            textBox1.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
                textBox1.Font = fontDialog1.Font;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main f = this.Tag as Main;
            f.font = fontDialog1.Font;
            f.text = textBox1.Text;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Main f = new Main();
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) button3_Click(button3, e);
        }
    }
}