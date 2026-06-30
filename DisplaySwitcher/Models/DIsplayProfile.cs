using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DisplaySwitcher.Models;

public class DisplayProfile : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _displayDeviceName = string.Empty;
    private int _width;
    private int _height;
    private int _frequency;
    private bool _isSelected;
    private bool _createCustomResolution;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string DisplayDeviceName
    {
        get => _displayDeviceName;
        set => SetProperty(ref _displayDeviceName, value);
    }

    public int Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    public int Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    public int Frequency
    {
        get => _frequency;
        set => SetProperty(ref _frequency, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool CreateCustomResolution
    {
        get => _createCustomResolution;
        set => SetProperty(ref _createCustomResolution, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
            return;

        field = value;
        OnPropertyChanged(propertyName);
    }
}