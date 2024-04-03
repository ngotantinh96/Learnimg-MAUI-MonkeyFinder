namespace MonkeyFinder.ViewModel;

[QueryProperty(nameof(Monkey), nameof(Monkey))]
public partial class MonkeyDetailsViewModel : BaseViewModel
{
    [ObservableProperty]
    Monkey monkey;
}
