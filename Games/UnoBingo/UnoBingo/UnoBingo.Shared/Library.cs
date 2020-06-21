using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
public class Library
{
    private const string title = "Uno Bingo";
    private const int size = 25;
    private const int balls = 90;
    private const int marks = 25;
    private const int maximum = 90;
    private readonly Random _random = new Random((int)DateTime.UtcNow.Ticks);
    private int _count;
    private int _house;
    private List<int> _balls;
    private List<int> _marks;
    private bool _gameOver = false;

    private void Show(string content, string title) =>
        _ = new MessageDialog(content, title).ShowAsync();

    private List<int> Choose(int total)
    {
        int number;
        var numbers = new List<int>();
        while (numbers.Count < total)
        {
            number = _random.Next(1, maximum + 1);
            if (!numbers.Contains(number) || numbers.Count < 1)
            {
                numbers.Add(number);
            }
        }
        return numbers;
    }

    private Ellipse Ellipse(bool ball, int value)
    {
        var ellipse = new Ellipse()
        {
            Width = size,
            Height = size,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        if (ball)
        {
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = new SolidColorBrush(Color.FromArgb(255, 248, 89, 119));
        }
        else
        {
            ellipse.Opacity = 0;
            ellipse.Name = $"mark{value}";
            ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 103, 229, 173));
        }
        return ellipse;
    }

    private void Add(Grid grid, bool ball, int row, int column, int value)
    {
        var element = new Grid()
        {
            Width = size,
            Height = size,
        };
        if (ball)
        {
            element.Opacity = 0;
            element.Name = $"ball{value}";
        }
        var label = new TextBlock()
        {
            FontSize = 12,
            Text = $"{value}",
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Colors.Black),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        element.Children.Add(label);
        element.Children.Add(Ellipse(ball, value));
        element.SetValue(Grid.RowProperty, row);
        element.SetValue(Grid.ColumnProperty, column);
        grid.Children.Add(element);
    }

    private Grid Layout(bool isBall, int rows, int cols, List<int> list)
    {
        int count = 0;
        Grid grid = new Grid()
        {
            Width = 250,
            Height = 250
        };
        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        for (int row = 0; row < rows; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
        }
        for (int column = 0; column < cols; column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < cols; column++)
            {
                Add(grid, isBall, row, column, list[count]);
                count++;
            }
        }
        return grid;
    }

    private void Set(StackPanel panel, bool isBall, int value, double opacity)
    {
        var name = $"{(isBall ? "ball" : "mark")}{value}";
        var element = (UIElement)panel.FindName(name);
        if (element != null)
            element.Opacity = opacity;
    }

    public void New(StackPanel panel)
    {
        _count = 0;
        _house = 0;
        _gameOver = false;
        _balls = Choose(balls);
        _marks = Choose(marks);
        panel.Children.Clear();
        panel.Children.Add(Layout(true, 9, 10, _balls));
        panel.Children.Add(Layout(false, 5, 5, _marks));
    }
    public void Play(StackPanel panel)
    {
        if (!panel.Children.Any()) New(panel);
        if (_count < balls && !_gameOver)
        {
            var ball = _balls[_count];
            Set(panel, true, ball, 1);
            if (_marks.Contains(ball))
            {
                _house++;
                Set(panel, false, ball, 0.5);
                if (_house == marks)
                {
                    _gameOver = true;
                    Show($"Full House in {_count} Balls!", title);
                }
            }
            _count++;
        }
        else
        {
            Show($"Game Over!", title);
        }
    }
}