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
using System.Globalization;
using System.ComponentModel;
using dms.solvers.neural_nets.kohonen;

namespace HexagonalGrid
{
    /// <summary>
    /// Логика взаимодействия для TilePane.xaml
    /// </summary>
    public partial class TilePane : UserControl
    {
        public TilePane()
        {
            InitializeComponent();
            InitializeTileSet();
        }

        public void SetMapWeights()
        {
            if (Weights.Count > 0)
                minWeight = maxWeight = Weights[0].Item2;

            foreach (var item in Weights)
            {
                minWeight = Math.Min(minWeight, item.Item2);
                maxWeight = Math.Max(maxWeight, item.Item2);
            }

            legendValues[0].Content = minWeight.ToString("0.00");
            legendValues[1].Content = (minWeight + 0.25 * (maxWeight - minWeight)).ToString("0.00");
            legendValues[2].Content = (minWeight + 0.5 * (maxWeight - minWeight)).ToString("0.00");
            legendValues[3].Content = (minWeight + 0.75 * (maxWeight - minWeight)).ToString("0.00");
            legendValues[4].Content = maxWeight.ToString("0.00");

            foreach (var tile in tiles)
                tile.Background = new Color() { A = 255, R = 135, G = 135, B = 135 };

            foreach (var item in Weights)
            {
                int2d pos = item.Item1;
                double value = (item.Item2 - minWeight) / (maxWeight + 0.1 - minWeight);
                int colorIndex = Convert.ToInt32(Math.Floor(value / 0.25));
                value -= 0.25 * colorIndex;

                Color color = new Color();
                color.A = 255;
                if (colorIndex == 0)
                {
                    color.R = 0;
                    color.B = 135;
                    color.G = Convert.ToByte(135 * value / 0.25);
                }
                else if (colorIndex == 1)
                {
                    color.R = 0;
                    color.B = Convert.ToByte(135 * (1 - value / 0.25));
                    color.G = 135;
                }
                else if (colorIndex == 2)
                {
                    color.R = Convert.ToByte(135 * value / 0.25);
                    color.B = 0;
                    color.G = 135;
                }
                else if (colorIndex == 3)
                {
                    color.R = 135;
                    color.B = 0;
                    color.G = Convert.ToByte(135 * (1 - value / 0.25));
                }

                tiles[pos.y * ColumnsCount + pos.x].Background = color;
            }
            Refresh();
        }

        public void Refresh()
        {
            double rowMult = 2.0 + 1.5 * (RowsCount - 1);
            double colMult = (ColumnsCount + 0.5) * Math.Sqrt(3);

            tilePane.Width = colMult * TileSize + 5;
            tilePane.Height = rowMult * TileSize + 50;
            this.Width = tilePane.Width + 2;
            this.Height = tilePane.Height + 2;

            double tileWidth = Math.Sqrt(3) * TileSize;
            double tileHeight = 3.0 * TileSize / 2.0;

            for (int row = 0; row < RowsCount; row++)
            {
                double rowPadding = (row % 2 == 0) ? tileWidth / 2.0 : 0.0;
                for (int col = 0; col < ColumnsCount; col++)
                {
                    Tile current = tiles[row * ColumnsCount + col];
                    current.Size = TileSize;
                    current.RenderTransform = new TranslateTransform(
                            tileWidth * col + rowPadding,
                            tileHeight * row);
                }
            }

            legend.Width = tilePane.Width - 10;
            legend.Height = 20;
            legend.RenderTransform = new TranslateTransform(5, tilePane.Height - 40);

            double legendXStep = legend.Width / 4.0;
            Size s;
            legendValues[0].RenderTransform = new TranslateTransform(5, tilePane.Height - 25);

            s = MeasureString(legendValues[1]);
            legendValues[1].RenderTransform = new TranslateTransform(legendXStep - s.Width / 2, tilePane.Height - 25);

            s = MeasureString(legendValues[2]);
            legendValues[2].RenderTransform = new TranslateTransform(2 * legendXStep - s.Width / 2, tilePane.Height - 25);

            s = MeasureString(legendValues[3]);
            legendValues[3].RenderTransform = new TranslateTransform(3 * legendXStep - s.Width / 2, tilePane.Height - 25);

            s = MeasureString(legendValues[4]);
            legendValues[4].RenderTransform = new TranslateTransform(tilePane.Width - 10 - s.Width, tilePane.Height - 25);
        }

        #region public double TileSize
        public static readonly DependencyProperty TileSizeProperty =
            DependencyProperty.Register("TileSize", typeof(double), typeof(TilePane),
                new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                    TileSizeChanged)
                {
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    PropertyChangedCallback = TileSizePropertyChanged
                });

        private static void TileSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TilePane)d).TileSize = (double)e.NewValue;
        }

        private static void TileSizeChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            ((TilePane)d).TileSize = (double)e.NewValue;
        }

        public double TileSize
        {
            get
            {
                return (double)GetValue(TileSizeProperty);
            }
            set
            {
                SetValue(TileSizeProperty, value);
                Refresh();
            }
        }
        #endregion

        #region public int RowsCount
        public static readonly DependencyProperty RowsCountProperty =
            DependencyProperty.Register("RowsCount", typeof(int), typeof(TilePane),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure, RowsCountChanged));
        private static void RowsCountChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((TilePane)d).RowsCount = (int)e.NewValue;
        }
        public int RowsCount
        {
            get { return (int)GetValue(RowsCountProperty); }
            set { SetValue(RowsCountProperty, value); InitializeTileSet(); }
        }
        #endregion

        #region public int ColumnsCount
        public static readonly DependencyProperty ColumnsCountProperty =
           DependencyProperty.Register("ColumnsCount", typeof(int), typeof(TilePane),
               new FrameworkPropertyMetadata(0,
                   FrameworkPropertyMetadataOptions.AffectsMeasure, ColumnsCountChanged));
        private static void ColumnsCountChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((TilePane)d).ColumnsCount = (int)e.NewValue;
        }
        public int ColumnsCount
        {
            get { return (int)GetValue(ColumnsCountProperty); }
            set { SetValue(ColumnsCountProperty, value); InitializeTileSet(); }
        }
        #endregion

        #region public List<Tuple<int2d, double>> Weights
        public static readonly DependencyProperty WeightsProperty =
    DependencyProperty.Register("Weights", typeof(List<Tuple<int2d, double>>), typeof(TilePane),
        new FrameworkPropertyMetadata(null
            , FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, WeightsPropertyChanged)
        {
            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            PropertyChangedCallback = WeightsPropertyChanged
        });

        private static void WeightsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TilePane)d).Weights = e.NewValue as List<Tuple<int2d, double>>;
        }
        public List<Tuple<int2d, double>> Weights
        {
            get
            {
                return (List<Tuple<int2d, double>>)GetValue(WeightsProperty);
            }
            set
            {
                SetValue(WeightsProperty, value);
                SetMapWeights();
            }
        }
        #endregion

        private Label[] legendValues;
        private Tile[] tiles;
        private Rectangle legend;

        private double minWeight, maxWeight;

        private Size MeasureString(Label target)
        {
            if (target.Content == null)
                return new Size(0, 0);

            var formattedText = new FormattedText(
                target.Content.ToString(),
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(target.FontFamily, target.FontStyle, target.FontWeight, target.FontStretch),
                target.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void InitializeTileSet()
        {
            tilePane.Children.Clear();

            tiles = new Tile[RowsCount * ColumnsCount];
            for (int row = 0; row < RowsCount; row++)
            {
                for (int col = 0; col < ColumnsCount; col++)
                {
                    tiles[row * ColumnsCount + col] = new Tile();
                    tilePane.Children.Add(tiles[row * ColumnsCount + col]);
                }
            }
            legend = new Rectangle();
            LinearGradientBrush br = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop(new Color() { A = 255, R = 0, B = 135, G = 0 }, 0),
                new GradientStop(new Color() { A = 255, R = 0, B = 135, G = 135 }, 0.25),
                new GradientStop(new Color() { A = 255, R = 0, B = 0, G = 135 }, 0.5),
                new GradientStop(new Color() { A = 255, R = 135, B = 0, G = 135 }, 0.75),
                new GradientStop(new Color() { A = 255, R = 135, B = 0, G = 0 }, 1)
            });
            legend.Fill = br;
            legend.Stroke = new SolidColorBrush(Colors.Black);
            legend.RadiusX = 5;
            legend.RadiusY = 5;

            legendValues = new Label[5];
            for (int i = 0; i < 5; i++)
            {
                legendValues[i] = new Label();
                tilePane.Children.Add(legendValues[i]);
            }

            tilePane.Children.Add(legend);
            Refresh();
        }
    }
}
