namespace Hitomi.Uno.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private INavigator _navigator;

    [ObservableProperty]
    private string? name;

    public MainViewModel(INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
    }
    public string? Title { get; }

    [RelayCommand]
    private async Task GoToSecondView()
    {
        await _navigator.NavigateViewModelAsync<DetailViewModel>(this, data: new Entity(Name!));
    }

}
