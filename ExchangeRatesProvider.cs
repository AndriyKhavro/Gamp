using System.Net.Http.Json;
using System.Text.Json;
using Gamp.Dto;

namespace Gamp;

internal class ExchangeRatesProvider
{
    private const string RequestUri = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<ExchangeRate[]?> GetExchangeRates()
    {
        using var httpClient = new HttpClient();
        try
        {
            return await httpClient.GetFromJsonAsync<ExchangeRate[]>(RequestUri, JsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}