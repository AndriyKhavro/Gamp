using System.Net.Http.Json;
using System.Text.Json;
using Gamp.Dto;

namespace Gamp;

// More information about API - see https://developers.google.com/analytics/devguides/collection/protocol/ga4/reference
internal class GoogleAnalyticsEventSender
{
    private readonly string _clientId;
    private Uri Endpoint { get; }
    private Uri DebugEndpoint { get; }
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GoogleAnalyticsEventSender(string apiSecret, string measurementId, string clientId)
    {
        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new ArgumentException("API_SECRET must not be null, empty or whitespace", nameof(apiSecret));
        }

        if (string.IsNullOrWhiteSpace(measurementId))
        {
            throw new ArgumentException("MEASUREMENT_ID must not be null, empty or whitespace", nameof(measurementId));
        }

        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new ArgumentException("CLIENT_ID must not be null, empty or whitespace", nameof(clientId));
        }

        _clientId = clientId;
        const string baseUrl = "https://www.google-analytics.com";
        string queryParams = $"?api_secret={apiSecret}&measurement_id={measurementId}";
        Endpoint = new Uri($"{baseUrl}/mp/collect{queryParams}");
        DebugEndpoint = new Uri($"{baseUrl}/debug/mp/collect{queryParams}");
    }

    public async Task<HttpResponseMessage> TrackEvents(IEnumerable<GoogleAnalyticsEvent> events)
    {
        var postData = new Dictionary<string, object>
        {
            { "client_id", _clientId },
            { "events", events }
        };

        using var httpClient = new HttpClient();

        await EnsureRequestValid(httpClient);

        var response = await httpClient.PostAsJsonAsync(Endpoint, postData, JsonSerializerOptions);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Sent event. Response status code: {response.StatusCode}. Content: {responseString}");
        return response;

        async Task EnsureRequestValid(HttpClient http)
        {
            var debugResponse = await http.PostAsJsonAsync(DebugEndpoint, postData, JsonSerializerOptions);
            debugResponse.EnsureSuccessStatusCode();
            var response = await debugResponse.Content.ReadFromJsonAsync<DebugResponse>();
            if (response!.ValidationMessages.Any())
            {
                throw new InvalidOperationException(
                    $"Found {response.ValidationMessages.Length} validation messages: {string.Join(',', response.ValidationMessages)}");
            }
        }
    }

    private class DebugResponse
    {
        public required object[] ValidationMessages { get; init; }
    }
}