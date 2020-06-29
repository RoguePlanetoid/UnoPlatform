using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace DualOrNoDual
{
    public class Library
    {
        private const string app_title = "Dual or no Dual";
        private const string app_culture_info = "en-GB";
        private const string app_value_format = "{0:c}";
        private const string game_over = "Game Over";
        private const string button_deal = "Deal";
        private const string button_not = "Not";
        private const string button_ok = "Ok";
        private const int total_boxes = 22;
        private const int high_values = 11;
        private const int total_rows = 4;
        private const int offer_point = 5;

        private readonly double[] box_values =
        {
            0.01, 0.10, 0.50, 1, 5, 10, 50, 100, 250, 500, 750,
            1000, 3000, 5000, 10000, 15000, 20000, 35000, 50000, 75000, 100000, 250000
        };
        private readonly string[] box_colours =
        {
            "0026ff", "0039ff", "004dff", "0060ff", "0073ff", "0086ff", "0099ff", "0099ff", "0099ff", "00acff", "00bfff",
            "ff5900", "ff4d00", "ff4000", "ff3300", "ff2600", "ff2600", "ff2600", "ff2600", "ff1a00", "ff1c00", "ff0d00"
        };
        private readonly char[] box_ids = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
            'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v'
        };
        private readonly int[] box_rows = { 5, 6, 6, 5 };

        private List<double> _amounts;
        private ContentDialog _dialog;
        private Random _random;
        private double _amount;
        private Grid _boxes;
        private Grid _values;
        private bool _dealt;
        private int _turn;

        private Color ConvertFromHex(string hex) =>
            (Color)XamlBindingHelper.ConvertValue(typeof(Color), $"#ff{hex}");

        private async Task<ContentDialogResult> ShowDialogAsync(
            string primary, string secondary, UIElement content)
        {
            if (_dialog != null)
                _dialog.Hide();
            _dialog = new ContentDialog
            {
                Title = app_title,
                Content = content,
                PrimaryButtonText = primary,
                SecondaryButtonText = secondary
            };
            return await _dialog.ShowAsync();
        }

        private IEnumerable<int> Shuffle(int total) =>
            Enumerable.Range(0, total).OrderBy(r => _random.Next(0, total));

        private string FormatAmount(double value) =>
            string.Format(new CultureInfo(app_culture_info), app_value_format, value);

        private Grid GetAmount(double value, Color background)
        {
            var grid = new Grid()
            {
                Background = new SolidColorBrush(background)
            };
            var text = new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = FormatAmount(value),
                Margin = new Thickness(5),
                FontSize = 24
            };
            grid.Children.Add(text);
            return grid;
        }

        private double GetOffer()
        {
            int count = 0;
            double total = 0.0;
            foreach (double amount in _amounts)
            {
                total += amount;
                count++;
            }
            double average = total / count;
            double offer = average * _turn / 10;
            return Math.Round(offer, 0);
        }

        private Color GetBackground(double amount)
        {
            int position = 0;
            while (amount != box_values[position])
                position++;
            return ConvertFromHex(box_colours[position]);
        }

        private UIElement GetValueElement(double amount) =>
            _values.FindName($"{box_ids[Array.IndexOf(box_values, amount)]}") as UIElement;

        private void Setup()
        {
            _turn = 0;
            _dealt = false;
            _amounts = new List<double>();
            _random = new Random((int)DateTime.Now.Ticks);
            foreach (var position in Shuffle(total_boxes))
                _amounts.Add(box_values[position]);
            _boxes.Children.Clear();
            _values.Children.Clear();
            _boxes.Children.Add(GetBoxes());
            _values.Children.Add(GetValues());
        }

        private async void Choose(Button button)
        {
            if (_turn < box_ids.Length)
            {
                double offer;
                var name = (char)button.Tag;
                _amount = _amounts[Array.IndexOf(box_ids, name)];
                var value = GetValueElement(_amount);
                button.Opacity = 0;
                var response = await ShowDialogAsync(button_ok, string.Empty,
                    GetAmount(_amount, GetBackground(_amount)));
                value.Opacity = 0;
                if (response == ContentDialogResult.Primary)
                {
                    if (!_dealt && _turn % offer_point == 0 && _turn > 1)
                    {
                        offer = GetOffer();
                        var result = await ShowDialogAsync(button_deal, button_not,
                            GetAmount(offer, Colors.Black));
                        if (result == ContentDialogResult.Primary)
                        {
                            _amount = offer;
                            _dealt = true;
                        }
                    }
                    _turn++;
                }
            }
            if (_turn == box_ids.Length || _dealt)
            {
                var result = await ShowDialogAsync(game_over, string.Empty, _dealt ?
                    GetAmount(_amount, Colors.Black) :
                    GetAmount(_amount, GetBackground(_amount)));
                if (result == ContentDialogResult.Primary)
                    Setup();
            }
        }

        private void Add(StackPanel panel, char tag, int value)
        {
            var button = new Button()
            {
                Margin = new Thickness(5),
                Tag = tag
            };
            button.Click += (object sender, RoutedEventArgs e) =>
            {
                Choose((Button)sender);
            };
            var box = new StackPanel()
            {
                Width = 100
            };
            var lid = new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.DarkRed),
                Height = 10
            };
            var front = new Grid()
            {
                Background = new SolidColorBrush(Colors.Red),
                Height = 75
            };
            var label = new Grid()
            {
                Background = new SolidColorBrush(Colors.WhiteSmoke),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 50
            };
            var text = new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.Black),
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Bold,
                Text = $"{value}",
                FontSize = 32
            };
            label.Children.Add(text);
            front.Children.Add(label);
            box.Children.Add(lid);
            box.Children.Add(front);
            button.Content = box;
            panel.Children.Add(button);
        }

        private StackPanel GetBoxes()
        {
            int count = 0;
            var panel = new StackPanel();
            for (var row = 0; row < total_rows; row++)
            {
                var places = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Orientation = Orientation.Horizontal
                };
                for (var column = 0; column < box_rows[row]; column++)
                    Add(places, box_ids[count], (count++) + 1);
                panel.Children.Add(places);
            }
            return panel;
        }

        private StackPanel AddValues(int start, int offset)
        {
            var panel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical
            };
            for (int value = start; value < offset; value++)
            {
                var grid = new Grid()
                {
                    Margin = new Thickness(5),
                    Name = $"{box_ids[value]}"
                };
                grid.Children.Add(GetAmount(box_values[value],
                    ConvertFromHex(box_colours[value])));
                panel.Children.Add(grid);
            }
            return panel;
        }

        private StackPanel GetValues()
        {
            var panel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            panel.Children.Add(AddValues(0, high_values));
            panel.Children.Add(AddValues(high_values, total_boxes));
            return panel;
        }

        public void New(Grid boxes, Grid values)
        {
            _boxes = boxes;
            _values = values;
            Setup();
        }
    }
}