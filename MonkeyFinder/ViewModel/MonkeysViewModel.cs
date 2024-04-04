using Microsoft.Maui.Networking;
using MonkeyFinder.Services;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
    private readonly MonkeyService monkeyService;
    private readonly IConnectivity connectivity;
    private readonly IGeolocation geolocation;

    public ObservableCollection<Monkey> Monkeys { get; } = [];

    [ObservableProperty]
    bool isRefreshing;

    public MonkeysViewModel(MonkeyService monkeyService, 
        IConnectivity connectivity, 
        IGeolocation geolocation)
    {
        this.monkeyService = monkeyService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;
        Title = "Monkey Finder";
    }

    [RelayCommand]
    public async Task GoToDetailPageAsync(Monkey monkey)
    {
        if (monkey is null)
            return;

        await Shell.Current.GoToAsync(nameof(DetailsPage), true, 
            new Dictionary<string, object>
            {
                { nameof(Monkey), monkey}
            }
        );
    }

    [RelayCommand]
    public async Task GetClosestMonkeyAsync()
    {
        if (IsBusy || Monkeys.Count <= 0)
            return;

        try
        {
            var location = await geolocation.GetLastKnownLocationAsync();

            location ??= await geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });

            if (location is null)
                return;

            var closestMonkey = Monkeys.OrderBy(monkey => 
                    location.CalculateDistance(monkey.Latitude, monkey.Longitude, DistanceUnits.Miles)
                ).FirstOrDefault();

            if (closestMonkey is null)
                return;

            await Shell.Current.DisplayAlert("Closest Monkey", $"{closestMonkey.Name} in {closestMonkey.Location}", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Shell.Current.DisplayAlert("Error!", $"Unable to get closest monkey: {ex.Message}", "Ok!");
        }
    }

    [RelayCommand]
    public async Task GetMonkeysAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet && connectivity.NetworkAccess != NetworkAccess.Local)
            {
                await Shell.Current.DisplayAlert("Internet Issue", "Please check your internet and try gain!", "OK");
                return;
            }

            IsBusy = true;

            var monkeys = await monkeyService.GetMonkeysAsync();

            if (Monkeys.Count != 0)
                Monkeys.Clear();

            foreach (var monkey in monkeys)
            {
                Monkeys.Add(monkey);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Shell.Current.DisplayAlert("Error!", $"Unable to get monkeys: {ex.Message}", "Ok!");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}
