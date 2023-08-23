namespace YoutubeApi7;

public class MetricsCounterService
{
    private DataApiService _dataApiService;
    private AnalyticsService _analyticsService;

    public MetricsCounterService(DataApiService apiService, AnalyticsService analyticsService)
    {
        _dataApiService = apiService;
        _analyticsService = analyticsService;
    }
}