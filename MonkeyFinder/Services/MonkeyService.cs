using System.Net.Http.Json;

namespace MonkeyFinder.Services;

public class MonkeyService
{
    private readonly HttpClient httpClient;

    private List<Monkey> monkeys = [];

    public MonkeyService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<Monkey>> GetMonkeysAsync()
    {
        if (monkeys?.Count > 0)
            return monkeys;

        var url = "https://montemagno.com/monkeys.json";
        var response = await httpClient.GetAsync(url);

        if(response.IsSuccessStatusCode)
        {
            monkeys = await response.Content.ReadFromJsonAsync<List<Monkey>>();
        }

        //using var stream = await FileSystem.OpenAppPackageFileAsync("monkeys.json");
        //using var reader = new StreamReader(stream);
        //var contents = await reader.ReadToEndAsync();
        //monkeys = JsonSerializer.Deserialize<List<Monkey>>(contents);

        return monkeys;
    }
}
