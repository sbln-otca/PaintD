using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using System.Collections.Generic;
using System.Linq;

namespace PaintD
{
    public enum ToolType
    {
        Select,
        Circle,
        Rectangle
    }

    public partial class MainWindow : Window
    {
        private Color _selectedColor = Colors.Red;
        private ToolType _currentTool = ToolType.Select;
        private List<VectorShape> _shapes = new List<VectorShape>();
        private VectorShape? _selectedShape = null;
        private CircleShape? _currentCircle = null;
        private RectangleShape? _currentRectangle = null;
        private Point _startPoint;
        private Point _dragOffset;
        private bool _isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            UpdateToolButtons();
        }

        // Инструменты
        private void SelectTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _currentTool = ToolType.Select;
            UpdateToolButtons();
            DeselectAll();
        }

        private void CircleTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _currentTool = ToolType.Circle;
            UpdateToolButtons();
            DeselectAll();
        }

        private void RectangleTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _currentTool = ToolType.Rectangle;
            UpdateToolButtons();
            DeselectAll();
        }

        private void UpdateToolButtons()
        {
            SelectToolButton.IsEnabled = _currentTool != ToolType.Select;
            CircleToolButton.IsEnabled = _currentTool != ToolType.Circle;
            RectangleToolButton.IsEnabled = _currentTool != ToolType.Rectangle;
        }

        //Цвета
        private void Red_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _selectedColor = Colors.Red;
        }
        private void Green_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _selectedColor = Colors.Green;
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
            var point = e.GetPosition(DrawingCanvas);
            if (!IsPointInCanvas(point)) return;

            _startPoint = point;

            if (_currentTool == ToolType.Select)
            {
                _selectedShape = _shapes.LastOrDefault(s => s.ContainsPoint(point));
                
                if (_selectedShape != null)
                {
                    DeselectAll();
                    _selectedShape.IsSelected = true;
                    _selectedShape.UpdateVisual();
                    
                    if (_selectedShape is CircleShape circle)
                    {
                        _dragOffset = new Point(point.X - circle.Center.X, point.Y - circle.Center.Y);
                    }
                    _isDragging = true;
                }
                else
                {
                    DeselectAll();
                }
            }
            else if (_currentTool == ToolType.Circle)
            {
                _currentCircle = new CircleShape(point, 0, _selectedColor);
                _shapes.Add(_currentCircle);
                var shape = _currentCircle.GetShape();
                DrawingCanvas.Children.Add(shape);
                UpdateShapePosition(_currentCircle);
            }
            else if (_currentTool == ToolType.Rectangle)
            {
                _currentRectangle = new RectangleShape(point, point, _selectedColor);
                _shapes.Add(_currentRectangle);
                var shape = _currentRectangle.GetShape();
                DrawingCanvas.Children.Add(shape);
                UpdateShapePosition(_currentRectangle);
            }
        }

        private void DrawingCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetPosition(DrawingCanvas);
            
            if (_currentTool == ToolType.Select && _isDragging && _selectedShape != null)
            {
                if (_selectedShape is CircleShape circle)
                {
                    circle.Center = new Point(point.X - _dragOffset.X, point.Y - _dragOffset.Y);
                    circle.UpdateVisual();
                    UpdateShapePosition(circle);
                }
            }
            else if (_currentTool == ToolType.Circle && _currentCircle != null && 
                     e.GetCurrentPoint(DrawingCanvas).Properties.IsLeftButtonPressed)
            {
                double dx = point.X - _startPoint.X;
                double dy = point.Y - _startPoint.Y;
                double radius = System.Math.Sqrt(dx * dx + dy * dy);
                
                _currentCircle.Radius = radius;
                _currentCircle.UpdateVisual();
                UpdateShapePosition(_currentCircle);
            }
            else if (_currentTool == ToolType.Rectangle && _currentRectangle != null &&
                     e.GetCurrentPoint(DrawingCanvas).Properties.IsLeftButtonPressed)
            {
                _currentRectangle.End = point;
                _currentRectangle.UpdateVisual();
                UpdateShapePosition(_currentRectangle);
            }
        }

        private void DrawingCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_currentTool == ToolType.Circle && _currentCircle != null)
            {
                if (_currentCircle.Radius < 5)
                {

                    DrawingCanvas.Children.Remove(_currentCircle.GetShape());
                    _shapes.Remove(_currentCircle);
                }
                _currentCircle = null;
            }
            else if (_currentTool == ToolType.Rectangle && _currentRectangle != null)
            {
                if (_currentRectangle.Width < 5 || _currentRectangle.Height < 5)
                {
                    DrawingCanvas.Children.Remove(_currentRectangle.GetShape());
                    _shapes.Remove(_currentRectangle);
                }
                _currentRectangle = null;
            }
            
            _isDragging = false;
            }

        private void UpdateShapePosition(CircleShape circle)
        {
            var shape = circle.GetShape();
            Canvas.SetLeft(shape, circle.Center.X - circle.Radius);
            Canvas.SetTop(shape, circle.Center.Y - circle.Radius);
        }

        private void UpdateShapePosition(RectangleShape rect)
        {
            var shape = rect.GetShape();
            Canvas.SetLeft(shape, rect.Left);
            Canvas.SetTop(shape, rect.Top);
        }

        private void DeselectAll()
        {
            foreach (var shape in _shapes)
        {
                shape.IsSelected = false;
                shape.UpdateVisual();
            }
            _selectedShape = null;
        }

        private bool IsPointInCanvas(Point point)
        {
            return point.X >= 0 && point.Y >= 0 && 
                   point.X <= DrawingCanvas.Bounds.Width && 
                   point.Y <= DrawingCanvas.Bounds.Height;
        }
    }
}