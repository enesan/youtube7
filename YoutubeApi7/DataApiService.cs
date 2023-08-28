using System.Text;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeApi7;

public class DataApiService
{
    private YouTubeService _service;
    private string _apiKey;
    public List<string> DefaultLanguages { get; private set; }
    private string _accessToken;
    

    public DataApiService(YouTubeService service, string apiKey, string accessToken)
    {
        _service = service;
        _apiKey = apiKey;
        _accessToken = accessToken;
    }
    
    public async Task<ulong> GetSubscriberCountAsync()
    {
        string parts = "snippet, statistics";

        var request = _service.Channels.List(parts);
        request.Key = _apiKey;
        request.Mine = true;
        request.AccessToken = _accessToken;

        var response = await request.ExecuteAsync();

        return response.Items[0].Statistics.SubscriberCount ?? 0;
    }

    
    // до лучших времен
    private async void GetVideos(Func<string, Task> videoProcessor)
    {
        int monthsShift = 3;
        
        string parts = "snippet";
        var request = _service.Search.List(parts);
        request.Key = _apiKey;
        request.ForMine = true;
        request.MaxResults = 50; // maximum
        request.Order = SearchResource.ListRequest.OrderEnum.Date;
        request.PublishedAfter = DateTime.Now.AddMonths(-monthsShift);

        SearchListResponse response;
        StringBuilder sb = new StringBuilder();

        do
        {
            response = await request.ExecuteAsync();

            foreach (var item in response.Items)
            {
                sb.Append(item.Id.VideoId + ",");
            }

            await videoProcessor(sb.Remove(sb.Length - 1, 1).ToString());
           // await GetVideoInfo(sb.Remove(sb.Length-1, 1).ToString());
            sb.Clear();
            
            request.PageToken = response.NextPageToken;

        } while (response.NextPageToken != null);

    }

    private async Task GetVideoInfo(string ids)
    {
        var request = _service.Videos.List("snippet");
        request.Id = ids;
        request.Key = _apiKey;
        request.MaxResults = 50; // maximum

        var response = await request.ExecuteAsync();
        
        foreach (var item in response.Items)
        {
            if (item.Snippet.DefaultLanguage != null)
                DefaultLanguages.Append(item.Snippet.DefaultLanguage);
            
            
        }
    }
}