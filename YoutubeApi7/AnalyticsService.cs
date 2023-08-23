using System.Security.Principal;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTubeAnalytics.v2;
using Google.Apis.YouTubeAnalytics.v2.Data;

namespace YoutubeApi7;

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
    public async Task<QueryResponse> GetBasicReport()
    {
        _query.Metrics = "views,comments,likes,dislikes,averageViewDuration,";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return response;
    }
    
    // Репорт просмотров по городам
    public async Task<QueryResponse> GetViewsReportByCity()
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
    public async Task<QueryResponse> GetDemographicReport()
    {
        _query.Dimensions = "ageGroup,gender";
        _query.Metrics = "viewerPercentage";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return response;
    }
    
    
    // ОС и тип девайса
    public async Task<QueryResponse> GetDeviceAndOs()
    {
        _query.Dimensions = "deviceType,operatingSystem,liveOrOnDemand,subscribedStatus";
        _query.Metrics = "views,estimatedMinutesWatched";
        var response = await _query.ExecuteAsync();
        ClearQuery();
        return response;
    }
    
    // подписчики и отписчики за месяц
    public async Task<QueryResponse> GetMonthSubsDynamic()
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