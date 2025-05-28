using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Paint
{
    public enum paintTools
    {
        Move,
        Zoom,
        Pen,
        Brush,
        Eraser,
        Fill,
        Gradiant,
        Line,
        Rectangle,
        Circle,
        Shape,
        Text,
        Picker
    }

    public partial class Paint : Form
    {
        public Cursor cCursor = new Cursor("Pen.cur");
        public paintTools cTool = paintTools.Pen;
        public Color fColor = Color.Black;
        public Color bColor = Color.White;
        public Pen cPen = new Pen(Color.Black);
        public List<Layer> cLayers;
        public NewImage cChild;
        public Font cFont;

        void ClearTool(ToolStripButton tsb)
        {
            for (int i = 0; i < toolStrip2.Items.Count; i++)
                try
                {
                    ((ToolStripButton)toolStrip2.Items[i]).Checked = false;
                }
                catch { }
            tsb.Checked = true;
        }

        void changeCursor(Cursor c)
        {
            if (cChild != null)
                cChild.pictureBox1.Cursor = cCursor = c;
        }

        public void refreshLayerBox(List<Layer> l)
        {
            cLayers = l;
            dataGridView1.RowCount = l.Count;
            for (int i = 0; i < l.Count; i++)
            {
                dataGridView1.Rows[i].Cells["cImage"].Value = l[i].bmp.GetThumbnailImage(32, 32, null,IntPtr.Zero);
                dataGridView1.Rows[i].Cells["cName"].Value = l[i].name;
            }
        }

        public Paint()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0;
            cPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            cFont = this.Font;
            toolStripLabel3.Text = cFont.Name;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Move;
            changeCursor(Cursors.SizeAll);
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                toolStripButton15.BackColor = cPen.Color = fColor = colorDialog1.Color;
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                toolStripButton14.BackColor = bColor = colorDialog1.Color;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
                cLayers[e.RowIndex].name = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (cChild == null) return;
            if (dataGridView1.CurrentRow == null)
                cChild.InsertLayer(0);
            else
                cChild.InsertLayer(dataGridView1.CurrentRow.Index);
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            cChild.cIndex = e.RowIndex;
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            if (cChild == null || dataGridView1.CurrentRow == null) return;
            cChild.DeleteLayer(dataGridView1.CurrentRow.Index);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Pen;
            changeCursor(new Cursor("Pen.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewImage ni = new NewImage(this, "");
            if (!ni.IsDisposed)
                ni.Show();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.FileName = cChild.Text;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            /* Serialization must be here
                             * 
                             * FileStream stream = File.Create(saveFileDialog1.FileName);
                            BinaryWriter writer = new BinaryWriter(stream);
                            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter =
                                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            formatter.Serialize(stream, cLayers);*/
                            break;
                        case 2:
                            cChild.pictureBox1.Image.Save(saveFileDialog1.FileName);
                            break;
                    }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                switch (openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3))
                {
                    case "ppf":
                        break;
                    case "jpg":
                        NewImage ni = new NewImage(this, openFileDialog1.FileName);
                        ni.Show();
                        break;
                }
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            /*for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                if (dataGridView1.Rows[i].Selected && dataGridView1.Rows[i + 1].Selected)
                    cLayers[i + 1].g.DrawImage(cLayers[i].bmp, 0, 0);

            for (int i = 0; i < dataGridView1.Rows.Count - 1; )
                if (dataGridView1.Rows[i].Selected && dataGridView1.Rows[i + 1].Selected)
                {
                    cLayers.RemoveAt(i);
                    dataGridView1.SelectedRows[dataGridView1.SelectedRows.Count - 1].Selected = false;
                }
                else
                    i++;*/
            try
            {
                if (dataGridView1.RowCount == 0 || dataGridView1.SelectedRows.Count == 0) return;
                int count = dataGridView1.SelectedRows.Count, start = dataGridView1.SelectedRows[count - 1].Index;
                for (int i = start; i < start + count - 1; i++)
                    cLayers[i + 1].g.DrawImage(cLayers[i].bmp, 0, 0);
                for (int i = 0; i < count - 1; i++)
                    cLayers.RemoveAt(start);
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    dataGridView1.Rows[i].Selected = false;
                dataGridView1.Rows[start].Selected = true;
                cChild.cIndex = dataGridView1.SelectedRows[0].Index;
                refreshLayerBox(cLayers);
            }
            catch { }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cPen.Width = int.Parse(toolStripComboBox1.Text) * 2;
            if (cTool == paintTools.Brush || cTool == paintTools.Eraser
                || cTool == paintTools.Line || cTool == paintTools.Rectangle
                || cTool == paintTools.Circle || cTool == paintTools.Shape)
                changeCursor(new Cursor(toolStripComboBox1.Text + "x.cur"));
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Brush;
            cPen.Color = fColor;
            changeCursor(new Cursor(toolStripComboBox1.Text + "x.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Eraser;
            cPen.Color = bColor;
            changeCursor(new Cursor(toolStripComboBox1.Text + "x.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Line;
            cPen.Color = fColor;
            changeCursor(new Cursor(toolStripComboBox1.Text + "x.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Rectangle;
            cPen.Color = fColor;
            changeCursor(new Cursor(toolStripComboBox1.Text + "x.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Circle;
            cPen.Color = fColor;
            changeCursor(new Cursor(toolStripComboBox1.Text + "x.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Shape;
            cPen.Color = fColor;
            changeCursor(new Cursor(toolStripComboBox1.Text + "x.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Fill;
            changeCursor(new Cursor("Fill.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Text;
            changeCursor(Cursors.IBeam);
            ClearTool((ToolStripButton)sender);
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                cFont = fontDialog1.Font;
                toolStripLabel3.Text = cFont.Name;
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            cTool = paintTools.Picker;
            changeCursor(new Cursor("CP.cur"));
            ClearTool((ToolStripButton)sender);
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(":::::::::: Product BY Bahman Akbarzadeh ::::::::::", "About", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
        }
    }
}