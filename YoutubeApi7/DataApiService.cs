using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeApi7;

public class DataApiService
{
    private YouTubeService _service;
    private string _apiKey;

    public DataApiService(YouTubeService service, string apiKey)
    {
        _service = service;
        _apiKey = apiKey;
    }

    public async Task<ulong> GetSubscriberCount()
    {
        string parts = "snippet, statistics";

        var request = _service.Channels.List(parts);
        request.Key = _apiKey;
        request.Mine = true;

        var response = await request.ExecuteAsync();

        return response.Items[0].Statistics.SubscriberCount ?? 0;
    }
}