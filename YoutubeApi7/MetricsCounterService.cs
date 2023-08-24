namespace YoutubeApi7;

/// <summary>
/// All metrics count for last 3 months
/// </summary>
public class MetricsCounterService
{
    private DataApiService _dataApiService;
    private AnalyticsService _analyticsService;

    public MetricsCounterService(AnalyticsService analyticsService, DataApiService apiService)
    {
        _dataApiService = apiService;
        _analyticsService = analyticsService;
    }

    public async Task<long> GetMeanViewsAsync()
    {
        long views = await _analyticsService.GetViewsAsync();
        long videos = await _analyticsService.GetVideosAddedToPlaylistsAsync();

        return views / videos;
    }

    public async Task<long> GetMeanCommentsAsync()
    {
        long comments = await _analyticsService.GetCommentsAsync();
        long videos = await _analyticsService.GetVideosAddedToPlaylistsAsync();
        
        if (videos == 0) return 0;

        return comments / videos;
    }

    public async Task<long> GetCommentsViewsAsync()
    {
        long comments = await _analyticsService.GetCommentsAsync();
        long views = await _analyticsService.GetViewsAsync();
        
        if (views == 0) return 0;

        return comments / views;
    }

    public async Task<long> GetLikesDislikesAsync()
    {
        long likes = await _analyticsService.GetLikesAsync();
        long dislikes = await _analyticsService.GetDislikesAsync();
        
        if (dislikes == 0) return 0;

        return likes / dislikes;
    }

    public async Task<long> GetMeanLikesAsync()
    {
        long likes = await _analyticsService.GetLikesAsync();
        long videos = await _analyticsService.GetVideosAddedToPlaylistsAsync();
        
        if (videos == 0) return 0;

        return likes / videos;
    }

    public async Task<long> GetMeanVideoAddFrequencyAsync()
    {
        long videos = await _analyticsService.GetVideosAddedToPlaylistsAsync();
        int months = 3;
        
        if (videos == 0) return 0;

        return videos / months;
    }

    public async Task<long> GetEngagementRateAsync()
    {
        long subs = (long)(await _dataApiService.GetSubscriberCountAsync());
        if (subs == 0) return 0;
        
        return await GetMeanLikesAsync()
               + await GetMeanLikesAsync()
               + await GetLikesDislikesAsync()
               + await GetMeanSharesAsync()
               + await GetMeanCommentsAsync()
               / subs * 100;
    }

    public async Task<long> GetMeanSharesAsync()
    {
        long shares = await _analyticsService.GetSharesAsync();
        long videos = await _analyticsService.GetVideosAddedToPlaylistsAsync();

        if (videos == 0) return 0;
        
        return shares / videos;
    }

    public async Task<long> GetMeanDislikesAsync()
    {
        long dislikes = await _analyticsService.GetDislikesAsync();
        long videos = await _analyticsService.GetVideosAddedToPlaylistsAsync();

        if (videos == 0) return 0;
        
        return dislikes / videos;
    }
}