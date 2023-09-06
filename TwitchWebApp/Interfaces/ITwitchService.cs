using TwitchWebApp.Models;

namespace TwitchWebApp;

public interface ITwitchService
{
    void Authorize(bool forceVerify);
    Task<TwitchReportDto> GetReport(string? userId = null);
    Task SetAccessToken(string code);
}