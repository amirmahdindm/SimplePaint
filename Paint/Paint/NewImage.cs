using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Paint
{
    public partial class NewImage : Form
    {
        Point cLocation = new Point(-1, -1);
        List<Layer> Layers = new List<Layer>();
        Graphics gTemp;
        public int cIndex = 0;
        int width, height, ew, eh;
        Layer cLayer;

        void mergImages()
        {
            cLayer.g.Clear(Color.White);
            for (int i = Layers.Count - 1; i >= 0; i--)
                cLayer.g.DrawImage(Layers[i].bmp, Layers[i].location);
            pictureBox1.Image = cLayer.bmp;
            ((Paint)MdiParent).refreshLayerBox(Layers);
        }

        public NewImage(Form fp, string file)
        {
            if (file == "")
            {
                AddNewImage ani = new AddNewImage();
                ani.ShowDialog();
                if (!ani.ok)
                {
                    Close();
                    return;
                }
                InitializeComponent();
                MdiParent = fp;
                Text = ani.textBox1.Text;
                width = (int)ani.numericUpDown1.Value;
                height = (int)ani.numericUpDown2.Value;
                Width = width + 20;
                Height = height + 50;
            }
            else
            {
                InitializeComponent();
                MdiParent = fp;
                Bitmap img = new Bitmap(file);
                Text = file.Substring(file.LastIndexOf('\\') + 1);
                width = img.Width;
                height = img.Height;
                Width = width + 20;
                Height = height + 50;
                Layers.Add(new Layer(Text, width, height));
                Layers[0].bmp = img;
            }
        }

        private void NewImage_Load(object sender, EventArgs e)
        {
            if (Layers.Count == 0)
                Layers.Add(new Layer("BackGround", width, height));
            cLayer = new Layer("", width, height);
            gTemp = pictureBox1.CreateGraphics();
            NewImage_Resize(null, null);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Layers.Count == 0) return;
            switch (((Paint)MdiParent).cTool)
            {
                //////////////////////////////////////////////////////
                case paintTools.Move:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (cLocation.X == -1)
                        {
                            cLocation.X = e.X;
                            cLocation.Y = e.Y;
                        }
                        Layers[cIndex].location.X += (e.Location.X - cLocation.X);
                        Layers[cIndex].location.Y += (e.Location.Y - cLocation.Y);
                        pictureBox1.Refresh();
                        gTemp.DrawImage(Layers[cIndex].bmp, Layers[cIndex].location.X + ew, Layers[cIndex].location.Y + eh);
                        cLocation = e.Location;
                    }
                    else
                        cLocation.X = cLocation.Y = -1;
                    break;
                //////////////////////////////////////////////////////
                case paintTools.Pen:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (cLocation.X == -1)
                        {
                            cLocation.X = e.X;
                            cLocation.Y = e.Y;
                        }
                        Layers[cIndex].g.DrawLine(new Pen(((Paint)MdiParent).fColor), cLocation.X - ew, cLocation.Y - eh, e.X - ew, e.Y - eh);
                        cLayer.g.DrawLine(new Pen(((Paint)MdiParent).fColor), cLocation.X - ew, cLocation.Y - eh, e.X - ew, e.Y - eh);
                        pictureBox1.Image = cLayer.bmp;
                        cLocation = e.Location;
                    }
                    else
                        cLocation.X = cLocation.Y = -1;
                    break;
                //////////////////////////////////////////////////////
                case paintTools.Brush:
                case paintTools.Eraser:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (cLocation.X == -1)
                        {
                            cLocation.X = e.X;
                            cLocation.Y = e.Y;
                        }
                        Layers[cIndex].g.DrawLine(((Paint)MdiParent).cPen, cLocation.X - ew, cLocation.Y - eh, e.X - ew, e.Y - eh);
                        cLayer.g.DrawLine(((Paint)MdiParent).cPen, cLocation.X - ew, cLocation.Y - eh, e.X - ew, e.Y - eh);
                        pictureBox1.Image = cLayer.bmp;
                        cLocation = e.Location;
                    }
                    else
                        cLocation.X = cLocation.Y = -1;
                    break;
                //////////////////////////////////////////////////////
                case paintTools.Line:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (cLocation.X == -1)
                        {
                            cLocation.X = e.X;
                            cLocation.Y = e.Y;
                        }
                        pictureBox1.Refresh();
                        gTemp.DrawLine(((Paint)MdiParent).cPen, cLocation.X, cLocation.Y, e.X, e.Y);
                    }
                    else
                        cLocation.X = cLocation.Y = -1;
                    break;
                //////////////////////////////////////////////////////
                case paintTools.Rectangle:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (cLocation.X == -1)
                        {
                            cLocation.X = e.X;
                            cLocation.Y = e.Y;
                        }
                        pictureBox1.Refresh();
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y > 0)
                            gTemp.DrawRectangle(((Paint)MdiParent).cPen, cLocation.X, cLocation.Y, e.X - cLocation.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y < 0)
                            gTemp.DrawRectangle(((Paint)MdiParent).cPen, cLocation.X, e.Y, e.X - cLocation.X, cLocation.Y - e.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y > 0)
                            gTemp.DrawRectangle(((Paint)MdiParent).cPen, e.X, cLocation.Y, cLocation.X - e.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y < 0)
                            gTemp.DrawRectangle(((Paint)MdiParent).cPen, e.X, e.Y, cLocation.X - e.X, cLocation.Y - e.Y);
                    }
                    else
                        cLocation.X = cLocation.Y = -1;
                    break;
                //////////////////////////////////////////////////////
                case paintTools.Circle:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (cLocation.X == -1)
                        {
                            cLocation.X = e.X;
                            cLocation.Y = e.Y;
                        }
                        pictureBox1.Refresh();
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y > 0)
                            gTemp.DrawEllipse(((Paint)MdiParent).cPen, cLocation.X, cLocation.Y, e.X - cLocation.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y < 0)
                            gTemp.DrawEllipse(((Paint)MdiParent).cPen, cLocation.X, e.Y, e.X - cLocation.X, cLocation.Y - e.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y > 0)
                            gTemp.DrawEllipse(((Paint)MdiParent).cPen, e.X, cLocation.Y, cLocation.X - e.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y < 0)
                            gTemp.DrawEllipse(((Paint)MdiParent).cPen, e.X, e.Y, cLocation.X - e.X, cLocation.Y - e.Y);
                    }
                    else
                        cLocation.X = cLocation.Y = -1;
                    break;
                //////////////////////////////////////////////////////
                case paintTools.Shape:
                    if (cLocation.X != -1)
                    {
                        pictureBox1.Refresh();
                        gTemp.DrawLine(((Paint)MdiParent).cPen, cLocation.X, cLocation.Y, e.X, e.Y);
                    }
                    break;
                //////////////////////////////////////////////////////
            }
        }

        private void NewImage_Resize(object sender, EventArgs e)
        {
            ew = (pictureBox1.Width - width) / 2;
            eh = (pictureBox1.Height - height) / 2;
        }

        private void NewImage_Activated(object sender, EventArgs e)
        {
            ((Paint)MdiParent).cChild = this;
            mergImages();
            pictureBox1.Cursor = ((Paint)MdiParent).cCursor;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                switch (((Paint)MdiParent).cTool)
                {
                    //////////////////////////////////////////////////////
                    case paintTools.Move:
                        Layer tmp = new Layer(Layers[cIndex].name, width, height);
                        tmp.g.DrawImage(Layers[cIndex].bmp, Layers[cIndex].location.X, Layers[cIndex].location.Y);
                        Layers[cIndex] = tmp;
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Pen:
                        Layers[cIndex].bmp.SetPixel(e.X - ew, e.Y - eh, ((Paint)MdiParent).fColor);
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Brush:
                    case paintTools.Eraser:
                        Layers[cIndex].g.DrawLine(((Paint)MdiParent).cPen, e.X - ew - 1, e.Y - eh - 1, e.X - ew, e.Y - eh);
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Fill:
                        fBmp = Layers[cIndex].bmp;
                        bc = Layers[cIndex].bmp.GetPixel(e.X - ew, e.Y - eh);
                        fc = ((Paint)MdiParent).fColor;
                        FillShape(e.X - ew, e.Y - eh);
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Line:
                        Layers[cIndex].g.DrawLine(((Paint)MdiParent).cPen, cLocation.X - ew, cLocation.Y - eh, e.X - ew, e.Y - eh);
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Rectangle:
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y > 0)
                            Layers[cIndex].g.DrawRectangle(((Paint)MdiParent).cPen, cLocation.X - ew, cLocation.Y - eh, e.X - cLocation.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y < 0)
                            Layers[cIndex].g.DrawRectangle(((Paint)MdiParent).cPen, cLocation.X - ew, e.Y - eh, e.X - cLocation.X, cLocation.Y - e.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y > 0)
                            Layers[cIndex].g.DrawRectangle(((Paint)MdiParent).cPen, e.X - ew, cLocation.Y - eh, cLocation.X - e.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y < 0)
                            Layers[cIndex].g.DrawRectangle(((Paint)MdiParent).cPen, e.X - ew, e.Y - eh, cLocation.X - e.X, cLocation.Y - e.Y);
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Circle:
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y > 0)
                            Layers[cIndex].g.DrawEllipse(((Paint)MdiParent).cPen, cLocation.X - ew, cLocation.Y - eh, e.X - cLocation.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X > 0 && e.Y - cLocation.Y < 0)
                            Layers[cIndex].g.DrawEllipse(((Paint)MdiParent).cPen, cLocation.X - ew, e.Y - eh, e.X - cLocation.X, cLocation.Y - e.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y > 0)
                            Layers[cIndex].g.DrawEllipse(((Paint)MdiParent).cPen, e.X - ew, cLocation.Y - eh, cLocation.X - e.X, e.Y - cLocation.Y);
                        if (e.X - cLocation.X < 0 && e.Y - cLocation.Y < 0)
                            Layers[cIndex].g.DrawEllipse(((Paint)MdiParent).cPen, e.X - ew, e.Y - eh, cLocation.X - e.X, cLocation.Y - e.Y);
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Shape:
                        if (cLocation.X > 0)
                            Layers[cIndex].g.DrawLine(((Paint)MdiParent).cPen, cLocation.X - ew, cLocation.Y - eh, e.X - ew, e.Y - eh);
                        if (cLocation.X == -2)
                            cLocation.X = cLocation.Y = -1;
                        else
                            cLocation = e.Location;
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Text:
                        textBox1.Text = "";
                        textBox1.Location = e.Location;
                        textBox1.Width = width - e.Location.X;
                        textBox1.Font = ((Paint)MdiParent).cFont;
                        textBox1.ForeColor = ((Paint)MdiParent).fColor;
                        textBox1.Visible = true;
                        textBox1.Focus();
                        break;
                    //////////////////////////////////////////////////////
                    case paintTools.Picker:
                        ((Paint)MdiParent).fColor = ((Paint)MdiParent).toolStripButton15.BackColor = cLayer.bmp.GetPixel(e.X - ew, e.Y - eh);
                        break;
                    //////////////////////////////////////////////////////
                }
                mergImages();
            }
            catch { }
        }

        Bitmap fBmp;
        Color bc, fc;
        private void FillShape(int x, int y)
        {
            Stack<int> stack = new Stack<int>();
            stack.Push(x);
            stack.Push(y);
            while (stack.Count > 0)
            {
                y = stack.Pop();
                x = stack.Pop();
                fBmp.SetPixel(x, y, fc);
                if (fBmp.GetPixel(x + 1, y) == bc)
                {
                    stack.Push(x + 1);
                    stack.Push(y);
                }
                if (fBmp.GetPixel(x - 1, y) == bc)
                {
                    stack.Push(x - 1);
                    stack.Push(y);
                }
                if (fBmp.GetPixel(x, y + 1) == bc)
                {
                    stack.Push(x);
                    stack.Push(y + 1);
                }
                if (fBmp.GetPixel(x, y - 1) == bc)
                {
                    stack.Push(x);
                    stack.Push(y - 1);
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (((Paint)MdiParent).cTool == paintTools.Shape)
                cLocation.X = cLocation.Y = -2;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Layers[cIndex].g.DrawString(textBox1.Text, textBox1.Font, ((Paint)MdiParent).cPen.Brush, textBox1.Location.X - ew - 2, textBox1.Location.Y - eh);
                textBox1.Visible = false;
                mergImages();
            }
        }

        public void InsertLayer(int index)
        {
            Layers.Insert(index, new Layer("NewLayer", width, height));
            mergImages();
        }

        public void DeleteLayer(int index)
        {
            Layers.RemoveAt(index);
            mergImages();
        }

        private void NewImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void NewImage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Layers.Clear();
            ((Paint)MdiParent).cChild = null;
            ((Paint)MdiParent).refreshLayerBox(Layers);
        }
    }

    public class Layer
    {
        public Bitmap bmp;
        public string name;
        public Point location;
        public Graphics g;

        public Layer(string n, int w, int h)
        {
            bmp = new Bitmap(w, h);
            name = n;
            location.X = location.Y = 0;
            g = Graphics.FromImage(bmp);
        }
    }
}