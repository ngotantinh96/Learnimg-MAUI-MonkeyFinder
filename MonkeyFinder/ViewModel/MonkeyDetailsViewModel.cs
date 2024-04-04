namespace MonkeyFinder.ViewModel;

[QueryProperty(nameof(Monkey), nameof(Monkey))]
public partial class MonkeyDetailsViewModel : BaseViewModel
{
    private readonly IMap map;
    [ObservableProperty]
    Monkey monkey;

    public MonkeyDetailsViewModel(IMap map)
    {
        this.map = map;
    }

    [RelayCommand]
    public async Task OpenMapAsync()
    {
        if (Monkey is null)
            return;

        try
        {
            await map.OpenAsync(Monkey.Latitude, Monkey.Longitude,
                new MapLaunchOptions
                {
                    Name = Monkey.Name,
                    NavigationMode = NavigationMode.None
                });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Shell.Current.DisplayAlert("Error!", $"Unable to open map: {ex.Message}", "Ok!");
        }
    }
}
