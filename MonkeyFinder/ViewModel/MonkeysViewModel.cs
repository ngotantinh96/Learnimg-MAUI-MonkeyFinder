using MonkeyFinder.Services;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
    private readonly MonkeyService monkeyService;

    public ObservableCollection<Monkey> Monkeys { get; } = [];

    public MonkeysViewModel(MonkeyService monkeyService)
    {
        this.monkeyService = monkeyService;
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
    public async Task GetMonkeysAsync()
    {
        if (IsBusy)
            return;

        try
        {
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
        }
    }
}
