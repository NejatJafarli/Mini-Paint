using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp15
{
    public partial class Form1 : Form
    {
        public Graphics grafik { get; set; }
        public List<IFigure> Figures { get; set; } = new List<IFigure>();
        public IFactory FigureFactory { get; set; }
        public Color FigureColor { get; set; }
        public interface IFigure
        {
            Point point { get; set; }
            Size size { get; set; }
            Color color { get; set; }
            bool isFill { get; set; }
        }
        class Circle : IFigure
        {
            public Point point { get; set; }
            public Size size { get; set; }
            public Color color { get; set; }
            public bool isFill { get; set; }
        }
        class rectangle : IFigure
        {
            public Point point { get; set; }
            public Size size { get; set; }
            public Color color { get; set; }
            public bool isFill { get; set; }

        }
        class Triangle : IFigure
        {
            public Point point { get; set; }
            public Point[] TrianglePoints { get; set; }
            public Size size { get; set; }
            public Color color { get; set; }
            public bool isFill { get; set; }
        }
        public interface IFactory
        {
            IFigure GetFigure();
        }

        public class CircleFactory : IFactory
        {
            public IFigure GetFigure() => new Circle();
        }
        public class RectangleFactory : IFactory
        {
            public IFigure GetFigure() => new rectangle();
        }
        public class TriangleFactory : IFactory
        {
            public IFigure GetFigure() => new Triangle();
        }
        public Form1()
        {
            InitializeComponent();
            grafik = CreateGraphics();

            cbColor.Items.Clear();
            List<string> Figures = new List<string>();
            Figures.Add("Circle");
            Figures.Add("Rectangle");
            Figures.Add("Triangle");

            cbFigures.Items.AddRange(Figures.ToArray());

            cbFigures.SelectedIndex = 2;

            string[] colores = Enum.GetNames(typeof(System.Drawing.KnownColor));
            cbColor.Items.AddRange(colores);
            cbColor.SelectedIndex = 29;
        }

        private void cbColor_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                string texto = cbColor.Items[e.Index].ToString();
                Brush border = new SolidBrush(e.ForeColor);
                Color color = Color.FromName(texto);
                Brush pincel = new SolidBrush(color);
                Pen boli = new Pen(e.ForeColor);
                e.Graphics.DrawRectangle(boli, new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + 2, 24, e.Bounds.Height - 4));
                e.Graphics.FillRectangle(pincel, new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, 22, e.Bounds.Height - 6));
                e.Graphics.DrawString(texto, e.Font, border, e.Bounds.Left + 30, e.Bounds.Top + 2);

                e.DrawFocusRectangle();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cbFigures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFigures.SelectedItem.ToString() == "Rectangle")
            {
                FigureFactory = new RectangleFactory();
            }
            else if (cbFigures.SelectedItem.ToString() == "Circle")
            {
                FigureFactory = new CircleFactory();
            }
            else if (cbFigures.SelectedItem.ToString() == "Triangle")
            {
                FigureFactory = new TriangleFactory();
            }
        }

        private void cbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            FigureColor = Color.FromName(cbColor.SelectedItem.ToString());
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            using (var g = e.Graphics)
            {
                foreach (var item in Figures)
                {
                    Pen pen = new Pen(item.color, 5);
                    SolidBrush brush = new SolidBrush(item.color);
                    if (item is rectangle rec)
                    {
                        if (rec.isFill)
                            grafik.FillRectangle(brush, rec.point.X, rec.point.Y, rec.size.Width, rec.size.Height);
                        else
                            grafik.DrawRectangle(pen, rec.point.X, rec.point.Y, rec.size.Width, rec.size.Height);
                    }
                    else if (item is Circle circle)
                    {
                        if (circle.isFill)
                            grafik.FillEllipse(brush, circle.point.X, circle.point.Y, circle.size.Width, circle.size.Height);
                        else
                            grafik.DrawEllipse(pen, circle.point.X, circle.point.Y, circle.size.Width, circle.size.Height);
                    }
                    else if (item is Triangle triangle)
                    {
                        if (triangle.isFill)
                            grafik.FillPolygon(brush, triangle.TrianglePoints);
                        else
                            grafik.DrawPolygon(pen, triangle.TrianglePoints);
                    }
                }

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        public Point MouseDownLocation { get; set; }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownLocation = e.Location;
        }
        public Point MouseUpLocation { get; set; }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUpLocation = e.Location;

            IFigure figure = FigureFactory.GetFigure();

            if (figure is Triangle triangle)
            {
                triangle.color = FigureColor;
                if (rbFill.Checked)
                    triangle.isFill = true;
                Point[] points = new Point[3];
                if (MouseDownLocation.Y >= MouseUpLocation.Y)
                {
                    points[0] = new Point((MouseDownLocation.X - MouseUpLocation.X) / 2 + MouseUpLocation.X, MouseUpLocation.Y);
                    points[1] = MouseDownLocation;
                    points[2] = new Point(MouseUpLocation.X, MouseDownLocation.Y);
                }
                else
                {
                    points[0] = new Point((MouseUpLocation.X - MouseDownLocation.X) / 2 + MouseDownLocation.X, MouseDownLocation.Y);
                    points[1] = MouseUpLocation;
                    points[2] = new Point(MouseDownLocation.X, MouseUpLocation.Y);
                }
                triangle.TrianglePoints = points;
            }
            else
                CreateFigure(figure);

            Figures.Add(figure);
            this.Refresh();
        }

        private void CreateFigure(IFigure rect)
        {
            rect.color = FigureColor;
            if (MouseUpLocation.X >= MouseDownLocation.X)
            {
                if (MouseUpLocation.Y <= MouseDownLocation.Y)
                {
                    rect.size = new Size(MouseUpLocation.X - MouseDownLocation.X, MouseDownLocation.Y - MouseUpLocation.Y);
                    rect.point = new Point(MouseDownLocation.X, MouseUpLocation.Y);
                }
                else
                {
                    rect.size = new Size(MouseUpLocation.X - MouseDownLocation.X, MouseUpLocation.Y - MouseDownLocation.Y);
                    rect.point = MouseDownLocation;
                }
            }
            else
            {
                if (MouseUpLocation.Y <= MouseDownLocation.Y)
                {
                    rect.size = new Size(MouseDownLocation.X - MouseUpLocation.X, MouseDownLocation.Y - MouseUpLocation.Y);
                    rect.point = MouseUpLocation;

                }
                else
                {
                    rect.size = new Size(MouseDownLocation.X - MouseUpLocation.X, MouseUpLocation.Y - MouseDownLocation.Y);
                    rect.point = new Point(MouseUpLocation.X, MouseDownLocation.Y);
                }
            }
            if (rbFill.Checked)
                rect.isFill = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Figures = new List<IFigure>();
            this.Refresh();
        }
    }
}
