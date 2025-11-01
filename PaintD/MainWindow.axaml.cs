using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;

namespace PaintD
{
    public partial class MainWindow : Window
    {
        private Color _selectedColor = Colors.Red;
        private bool _isDrawing = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Red_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _selectedColor = Colors.Red;
        }
        private void Black_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _selectedColor = Colors.Black;
        }

        private void Blue_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _selectedColor = Colors.Blue;
        }

        private void Yellow_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _selectedColor = Colors.Yellow;
        }

        private void DrawingCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _isDrawing = true;
            DrawAtPoint(e.GetPosition(DrawingCanvas));
        }

        private void DrawingCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDrawing && e.GetCurrentPoint(DrawingCanvas).Properties.IsLeftButtonPressed)
            {
                DrawAtPoint(e.GetPosition(DrawingCanvas));
            }
        }

        private void DrawingCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDrawing = false;
        }

        private void DrawAtPoint(Point point)
        {
            var ellipse = new Ellipse
        {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(_selectedColor),
                [Canvas.LeftProperty] = point.X - 5,
                [Canvas.TopProperty] = point.Y - 5
            };
            DrawingCanvas.Children.Add(ellipse);
        }
    }
}