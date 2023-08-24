using System.Security.Principal;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTubeAnalytics.v2;
using Google.Apis.YouTubeAnalytics.v2.Data;

namespace YoutubeApi7;
/// <summary>
/// Отчеты за последние три месяца, кроме GetMonthSubsDynamic()
/// </summary>
// класс метрик. географию, возраст и прочие подробности пока почти везде опускаю,
// потому что их можно применить только к определенным метрикам - нужно уточнение, к каким
public class AnalyticsService
{
    private YouTubeAnalyticsService _service;
    private ReportsResource.QueryRequest _query;

    private string _threeMonthsAgo;
    
    public AnalyticsService(YouTubeAnalyticsService service, UserCredential credential)
    {
        _service = service;
        
        string threeMonthsAgo = "2012-01-02"; // DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
        string dateNow = DateTime.Now.ToString("yyyy-MM-dd");
        
        _threeMonthsAgo = threeMonthsAgo;
        
        // постоянные значения запроса
        _query = _service.Reports.Query();
        _query.Credential = credential;
        _query.StartDate = threeMonthsAgo;
        _query.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
        _query.Ids = "channel==MINE";
        _query.MaxResults = 250;

    }

    // базовые метрики
    public async Task<long> GetViewsAsync()
    {
        _query.Metrics = "views";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (int)((long)response.Rows[0][0]);
    }
    
    public async Task<long> GetCommentsAsync()
    {
        _query.Metrics = "comments";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (long)response.Rows[0][0];
    }
    
    public async Task<long> GetLikesAsync()
    {
        _query.Metrics = "likes";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (long)response.Rows[0][0];
    }
    
    public async Task<long> GetDislikesAsync()
    {
        _query.Metrics = "dislikes";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (long)response.Rows[0][0];
    }
    
    public async Task<long> GetAverageViewDurationAsync()
    {
        _query.Metrics = "averageViewDuration";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (long)response.Rows[0][0];
    }

    public async Task<long> GetAverageDurationAsync()
    {
        _query.Metrics = "averageViewDuration";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (long)response.Rows[0][0];
    }
    
    public async Task<long> GetVideosAddedToPlaylistsAsync()
    {
        _query.Metrics = "videosAddedToPlaylists";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (long)response.Rows[0][0];
    }
    
    public async Task<long> GetSharesAsync()
    {
        _query.Metrics = "shares";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return (long)response.Rows[0][0];
    }

    // Репорт просмотров по городам
    public async Task<QueryResponse> GetViewsReportByCityAsync()
    {
        _query.Dimensions = "city";
        _query.Sort = "-views";
        _query.Metrics = "views,estimatedMinutesWatched,averageViewDuration,averageViewPercentage," +
                         "videosAddedToPlaylists";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return response;
    }

    // Возраст, пол
    public async Task<QueryResponse> GetDemographicReportAsync()
    {
        _query.Dimensions = "ageGroup,gender";
        _query.Metrics = "viewerPercentage";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return response;
    }
    
    
    // ОС и тип девайса
    public async Task<QueryResponse> GetDeviceAndOsAsync()
    {
        _query.Dimensions = "deviceType,operatingSystem,liveOrOnDemand,subscribedStatus";
        _query.Metrics = "views,estimatedMinutesWatched";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return response;
    }
    
    // подписчики и отписчики за месяц
    public async Task<QueryResponse> GetMonthSubsDynamicAsync()
    {
        _query.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MMMM-dd");
        _query.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
        _query.Metrics = "subscribersGained,subscribersLost";
        var response = await _query.ExecuteAsync();
        
        ClearQuery();
        _query.StartDate = _threeMonthsAgo;
        
        return response;
    }

    private void ClearQuery()
    {
        _query.Dimensions = null;
        _query.Metrics = null;
        _query.Sort = null;
        _query.Filters = null;
    }
    

}