using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;
using ExCSS;
using Newtonsoft.Json;
using Svg;

namespace Final_Project
{
    public partial class Form1 : Form
    {
        public List<Shape> Shapes = new List<Shape>();
        public bool Candraw = false;
        public bool SelectMode = false;
        public float xstart;
        public float ystart;

        public Form1()
        {
            InitializeComponent();
            strokeColorDialog.Color = System.Drawing.Color.Blue;
            fillColorDialog.Color = System.Drawing.Color.Orange;
            UpdatePanels();
            comboBox1.SelectedIndex = 0;
        }

        private void UpdatePanels()
        {
            panelStrokeColor.BackColor = strokeColorDialog.Color;
            panelFillColor.BackColor = fillColorDialog.Color;
        }

        private void panelFillColor_Click(object sender, EventArgs e)
        {
            fillColorDialog.ShowDialog();
            checkBox1.Checked = true;
            UpdatePanels();
        }

        private void panelStrokeColor_Click(object sender, EventArgs e)
        {
            strokeColorDialog.ShowDialog();
            UpdatePanels();
        }

        private Shape CreateShape(float x, float y)
        {
            return new Shape()
            {
                X = x,
                Y = y,
                Width = 10,
                Height = 10,
                StrokeColor = strokeColorDialog.Color,
                FillColor = fillColorDialog.Color,
                Filled = checkBox1.Checked,
                StrokeThickness = (float)numericUpDown1.Value,
                Type = comboBox1.SelectedIndex == 1 ? ShapeType.Ellipse : ShapeType.Rect,
            };
        }

        private void SaveSVGFile(string fileName)
        {
            string json = JsonConvert.SerializeObject(Shapes, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        private void LoadSVGFile(string fileName)
        {
            string json = File.ReadAllText(fileName);
            Shapes = JsonConvert.DeserializeObject<List<Shape>>(json)!;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var shape in Shapes)
            {
                using var pen = new Pen(shape.StrokeColor, shape.StrokeThickness);
                using var brush = new SolidBrush(shape.FillColor);
                if (shape.Type == ShapeType.Ellipse)
                {
                    if (shape.Filled)
                    {
                        e.Graphics.FillEllipse(brush, shape.X, shape.Y, shape.Width, shape.Height);
                    }

                    e.Graphics.DrawEllipse(pen, shape.X, shape.Y, shape.Width, shape.Height);
                }

                if (shape.Type == ShapeType.Rect)
                {
                    if (shape.Filled)
                    {
                        e.Graphics.FillRectangle(brush, shape.X, shape.Y, shape.Width, shape.Height);
                    }

                    e.Graphics.DrawRectangle(pen, shape.X, shape.Y, shape.Width, shape.Height);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "SVG files (*.msp)|*.msp";
            saveFileDialog1.Title = "Save file";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("File not saved");
                return;
            }
            SaveSVGFile(saveFileDialog1.FileName);
        }

        public static SvgColourServer ToSvgColor(System.Drawing.Color color)
        {
            return new SvgColourServer(ColorTranslator.FromHtml(ColorTranslator.ToHtml(color)));
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SvgDocument svgDoc = new SvgDocument();

            saveFileDialog1.Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            foreach (var shape in Shapes)
            {
                SvgElement svgElement = null;

                if (shape.Type == ShapeType.Ellipse)
                {
                    svgElement = new SvgEllipse()
                    {
                        CenterX = new SvgUnit(shape.X + shape.Width / 2),
                        CenterY = new SvgUnit(shape.Y + shape.Height / 2),
                        RadiusX = shape.Width / 2,
                        RadiusY = shape.Height / 2,
                        Fill = shape.Filled ? ToSvgColor(shape.FillColor) : null,
                        StrokeWidth = shape.StrokeThickness,
                        Stroke = ToSvgColor(shape.StrokeColor)
                    };
                }

                if (shape.Type == ShapeType.Rect)
                {
                    svgElement = new SvgRectangle()
                    {
                        X = new SvgUnit(shape.X),
                        Y = new SvgUnit(shape.Y),
                        Width = shape.Width,
                        Height = shape.Height,
                        Fill = shape.Filled ? ToSvgColor(shape.FillColor) : null,
                        StrokeWidth = shape.StrokeThickness,
                        Stroke = ToSvgColor(shape.StrokeColor)
                    };
                }

                if (svgElement != null)
                {
                    svgDoc.Children.Add(svgElement);
                }

                using (var stream = new MemoryStream())
                {
                    svgDoc.Write(stream);
                    File.WriteAllText(saveFileDialog1.FileName, System.Text.Encoding.UTF8.GetString(stream.ToArray()));
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "MSP files (*.msp)|*.msp|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            LoadSVGFile(openFileDialog1.FileName);
            pictureBox1.Invalidate();
        }

        public void Undo()
        {
            // Delete the most recent shape
            if (Shapes.Count > 0)
                Shapes.RemoveAt(Shapes.Count - 1);
            pictureBox1.Invalidate();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shapes.Clear();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        var point = pictureBox1.PointToScreen(new System.Drawing.Point(0, 0));
                        xstart = MousePosition.X - point.X;
                        ystart = MousePosition.Y - point.Y;
                        if (!SelectMode)
                        {
                            Candraw = true;

                            var x = MousePosition.X - point.X;
                            var y = MousePosition.Y - point.Y;
                            Shapes.Add(CreateShape(x, y));
                        }
                        else
                        {
                            Shape ToDelete = null;
                            foreach (var shape in Shapes)
                            {
                                var x2 = shape.X + shape.Width;
                                var y2 = shape.Y + shape.Height;
                                if (xstart < x2 & xstart > shape.X & ystart < y2 & ystart > shape.Y)
                                {
                                    ToDelete = shape;
                                }
                            }
                            if (ToDelete != null)
                            {
                                Shapes.Remove(ToDelete);
                            }
                            pictureBox1.Image = null;
                            pictureBox1.Invalidate();
                        }
                    }
                    break;
                case MouseButtons.Right:
                    {
                        Undo();
                    }
                    break;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var point = pictureBox1.PointToScreen(new System.Drawing.Point(0, 0));
            var xcurrent = MousePosition.X - point.X;
            var ycurrent = MousePosition.Y - point.Y;
            if (Candraw && !SelectMode)
            {
                var x = xcurrent > xstart ? xstart : xcurrent;
                var y = ycurrent > ystart ? ystart : ycurrent;
                Shapes[Shapes.Count - 1].X = x;
                Shapes[Shapes.Count - 1].Y = y;
                Shapes[Shapes.Count - 1].Width = Math.Abs(xcurrent - xstart);
                Shapes[Shapes.Count - 1].Height = Math.Abs(ycurrent - ystart);
                pictureBox1.Invalidate();
            }
            else if (SelectMode)
            {
                pictureBox1.Cursor = Cursors.Default;
                bool find = false;
                SvgDocument svg = new SvgDocument();
                Shape ToDraw = null;
                foreach (var shape in Shapes)
                {
                    var x2 = shape.X + shape.Width;
                    var y2 = shape.Y + shape.Height;
                    if (xcurrent < x2 & xcurrent > shape.X & ycurrent < y2 & ycurrent > shape.Y)
                    {
                        find = true;
                        pictureBox1.Cursor = Cursors.Hand;
                        ToDraw = shape;
                    }
                    else
                    {
                        if (!find)
                        {
                            pictureBox1.Cursor = Cursors.Default;
                            pictureBox1.Image = null;
                        }
                    }
                }
                if (ToDraw != null)
                {
                    SvgRectangle rect = new SvgRectangle()
                    {
                        X = ToDraw.X - ToDraw.StrokeThickness - 3,
                        Y = ToDraw.Y - ToDraw.StrokeThickness - 3,
                        Width = ToDraw.Width + 2 * ToDraw.StrokeThickness + 6,
                        Height = ToDraw.Height + 2 * ToDraw.StrokeThickness + 6,
                        Fill = null,
                        Stroke = new SvgColourServer(System.Drawing.Color.Black),
                        StrokeWidth = new SvgUnit(1)
                    };
                    svg.Children.Add(rect);
                    pictureBox1.Image = svg.Draw();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.Invalidate();
            Candraw = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!SelectMode)
            {
                pictureBox1.Cursor = Cursors.Default;
                SelectMode = true;
                button1.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                pictureBox1.Cursor = Cursors.Cross;
                SelectMode = false;
                button1.ResetBackColor();
            }
        }
    }
}