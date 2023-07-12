using System.Net.Http.Json;
using System.Text.Json;
using Gamp.Dto;

namespace Gamp;

// More information about API - see https://developers.google.com/analytics/devguides/collection/protocol/ga4/reference
internal class GoogleAnalyticsEventSender
{
    private readonly string _clientId;
    private Uri Endpoint { get; }
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GoogleAnalyticsEventSender(string apiSecret, string measurementId, string clientId)
    {
        _clientId = clientId;

        Endpoint = new Uri($"https://www.google-analytics.com/mp/collect?api_secret={apiSecret}&measurement_id={measurementId}");
    }

    public async Task<HttpResponseMessage> TrackEvents(IEnumerable<GoogleAnalyticsEvent> events)
    {
        var postData = new Dictionary<string, object>
        {
            { "client_id", _clientId },
            { "events", events }
        };

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync(Endpoint, postData, JsonSerializerOptions);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Sent event. Response status code: {response.StatusCode}. Content: {responseString}");
        return response;
    }
}