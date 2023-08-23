using System.Security.Principal;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTubeAnalytics.v2;
using Google.Apis.YouTubeAnalytics.v2.Data;

namespace YoutubeApi7;

public class AnalyticsService
{
    private YouTubeAnalyticsService _service;
    private string _threeMonthsAgo;
    private string _dateNow;

    public AnalyticsService(YouTubeAnalyticsService service)
    {
        _service = service;
        _threeMonthsAgo = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
        _dateNow = DateTime.Now.ToString("yyyy-MM-dd");
    }

    // продолжительность просмотра, дизлайки, лайки, просмотры, комменты по странам
    public async Task<QueryResponse> GetBasicReport()
    {
        var query = _service.Reports.Query();
        query.StartDate = _threeMonthsAgo;
        query.EndDate = _dateNow;
        query.Dimensions = "country";
        query.Metrics = "views,comments,likes,dislikes,estimatedMinutesWatched,averageViewDuration";
        query.Ids = "channel==MINE";
        var response = await query.ExecuteAsync();
        return response;
    }
    
    // Репорт просмотров по городам
    public async Task<QueryResponse> GetViewsReportByCity()
    {
        var query = _service.Reports.Query();
        query.StartDate = _threeMonthsAgo;
        query.EndDate = _dateNow; 
        query.Dimensions = "city";
        query.MaxResults = 250;
        query.Sort = "-views";
        query.Metrics = "views,estimatedMinutesWatched,averageViewDuration,averageViewPercentage";
        query.Ids = "channel==MINE";
        var response = await query.ExecuteAsync();
        return response;
    }

    // Возраст, пол
    public async Task<QueryResponse> GetDemographicReport()
    {
        var query = _service.Reports.Query();
        query.StartDate = _threeMonthsAgo;
        query.EndDate = _dateNow;
        query.Dimensions = "ageGroup,gender";
        query.Metrics = "viewerPercentage";
        query.Ids = "channel==MINE";
        var response = await query.ExecuteAsync();
        return response;
    }

    // репосты
    public async Task<QueryResponse> GetShares()
    {
        var query = _service.Reports.Query();
        query.StartDate = _threeMonthsAgo;
        query.EndDate = _dateNow;
        query.Dimensions = "ageGroup,gender";
        query.Metrics = "viewerPercentage";
        query.Ids = "channel==MINE";
        var response = await query.ExecuteAsync();
        return response;
    }
    
    // ОС и тип девайса
    public async Task<QueryResponse> GetDeviceAndOs()
    {
        var query = _service.Reports.Query();
        query.StartDate = _threeMonthsAgo;
        query.EndDate = _dateNow;
        query.Dimensions = "deviceType,operatingSystem,liveOrOnDemand,subscribedStatus";
        query.Metrics = "views,estimatedMinutesWatched";
        query.Ids = "channel==MINE";
        var response = await query.ExecuteAsync();
        return response;
    }
    

}