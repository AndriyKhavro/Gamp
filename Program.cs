using Gamp;
using Gamp.Dto;

string apiSecret = GetEnvironmentVariable("API_SECRET");
string measurementId = GetEnvironmentVariable("MEASUREMENT_ID");
string clientId = GetEnvironmentVariable("CLIENT_ID");
var supportedExchangeRateCodes = new[] { "EUR", "GBP", "PLN", "RSD" };

var eventSender = new GoogleAnalyticsEventSender(apiSecret, measurementId, clientId);
var exchangeRatesProvider = new ExchangeRatesProvider();

var timer = new PeriodicTimer(TimeSpan.FromHours(1));

do
{
    var rates = await exchangeRatesProvider.GetExchangeRates();
    if (rates is null)
    {
        Console.WriteLine("Failed to fetch exchange rates");
        continue;
    }

    var events = rates
        .Where(rate => supportedExchangeRateCodes.Contains(rate.Cc))
        .Select(rate => new GoogleAnalyticsEvent("currency_rates", rate));

    await eventSender.TrackEvents(events);
}
while (await timer.WaitForNextTickAsync());

string GetEnvironmentVariable(string name)
{
    return Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException($"Environment variable {name} is not set.");
}