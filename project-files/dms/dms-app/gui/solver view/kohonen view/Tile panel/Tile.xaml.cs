using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexagonalGrid
{
    /// <summary>
    /// Логика взаимодействия для Tile.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        private double size;
        private Polygon p;
        private SolidColorBrush color;

        private void calcGraphics()
        {
            this.Width = Math.Sqrt(3) * size + 5;
            this.Height = 2 * size + 5;

            Point center = new Point(this.Width / 2.0, this.Height / 2.0);
            Point A = new Point(center.X, center.Y - size);
            Point B = new Point(center.X + Math.Sqrt(3) / 2.0 * size, center.Y - size / 2.0);
            Point C = new Point(B.X, center.Y + size / 2.0);
            Point D = new Point(center.X, center.Y + size);
            Point E = new Point(center.X - Math.Sqrt(3) / 2.0 * size, C.Y);
            Point F = new Point(E.X, B.Y);

            p.Points = new PointCollection { A, B, C, D, E, F };
            p.Stroke = new SolidColorBrush(Colors.Black);
            p.Fill = color;
            this.Content = p;
        }

        public Tile()
        {
            InitializeComponent();
            p = new Polygon();
            color = new SolidColorBrush(Color.FromRgb(135, 135, 135));
            calcGraphics();
        }

        public double Size { get { return size; } set { size = value; calcGraphics();} }
        public Color Background { get { return color.Color; } set { color = new SolidColorBrush(value); calcGraphics(); } }
    }
}
