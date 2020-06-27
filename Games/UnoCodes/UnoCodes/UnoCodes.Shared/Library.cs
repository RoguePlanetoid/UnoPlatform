using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class CommandHandler : ICommand
{
    public event EventHandler CanExecuteChanged = null;
    private readonly Action _action;

    public CommandHandler(Action action) => _action = action;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) => _action();
}

public class Code : INotifyPropertyChanged
{
    private int _value;
    private SolidColorBrush _foreground;
    private SolidColorBrush _background;

    public virtual void OnPropertyChanged(string propertyName) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public int Index { get; set; }
    public int Value { get => _value; set { _value = value; OnPropertyChanged("Value"); } }
    public SolidColorBrush Foreground { get => _foreground; set { _foreground = value; OnPropertyChanged("Foreground"); } }
    public SolidColorBrush Background { get => _background; set { _background = value; OnPropertyChanged("Background"); } }
    public Func<int, int> Action { get; set; }
    public ICommand Command => new CommandHandler(() => Action(Index));
    public event PropertyChangedEventHandler PropertyChanged;
}

public class Library
{
    private const string app_title = "Uno Codes";
    private const int size = 4;
    private readonly Color light = Color.FromArgb(255, 103, 229, 173);
    private readonly Color dark = Color.FromArgb(255, 122, 103, 248);
    private readonly Color fore = Color.FromArgb(255, 248, 89, 119);
    private readonly Color back = Color.FromArgb(255, 21, 155, 255);

    private readonly ObservableCollection<Code> _codes = new ObservableCollection<Code>();
    private readonly Random _random = new Random((int)DateTime.Now.Ticks);
    private List<int> _numbers = new List<int>();
    private int _turns = 0;

    private List<int> Shuffle(int start, int total) => 
        Enumerable.Range(start, total).OrderBy(r => _random.Next(start, total)).ToList();

    public void Show(string content, string title) => 
        _ = new MessageDialog(content, title).ShowAsync();

    private Code GetCode(int value, int index) => 
        new Code()
        {
            Action = (int i) =>
            {
                Code code = _codes[i];
                code.Value = (code.Value == size) ? 1 : code.Value + 1;
                code.Foreground = new SolidColorBrush(dark);
                code.Background = new SolidColorBrush(light);
                return code.Value;
            },
            Index = index,
            Value = value,
            Foreground = new SolidColorBrush(dark),
            Background = new SolidColorBrush(light)
        };

    private bool Check(int number, int index)
    {
        var code = _codes[index];
        if (number == code.Value)
        {
            code.Foreground = new SolidColorBrush(dark);
            code.Background = new SolidColorBrush(light);
            return true;
        }
        else
        {
            code.Foreground = new SolidColorBrush(fore);
            code.Background = new SolidColorBrush(back);
            return false;
        }
    }

    public void New(ItemsControl items)
    {
        _turns = 0;
        _codes.Clear();
        for (int i = 0; i < size; i++)
        {
            _codes.Add(GetCode(i + 1, i));
        }
        items.ItemsSource = _codes;
        _numbers = Shuffle(1, size);
    }

    public void Accept(ItemsControl items)
    {
        int index = 0;
        int correct = 0;
        foreach (int number in _numbers)
        {
            if (Check(number, index))
                correct++;
            index++;
        }
        _turns++;
        if (correct == size)
        {
            Show($"You got all the numbers correct in {_turns} turns!", app_title);
            New(items);
        }
    }
}