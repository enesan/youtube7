namespace TwitchWebApp.Models;

public class TwitchReportDto
{
    public Guid Id { get; set; }
    public double MeanViewers { get; set; }
    public double MeanViews { get; set; }
    public long Subscribers { get; set; }
    public long Views { get; set; } 
    public double StreamsAddingFrequency { get; set; }
    public double MeanUniqueViewers { get; set; }
    public double MeanUniqueCommenters { get; set; }
    public double MeanCommentsCount { get; set; }
    public double ViewersUniqueCommenters { get; set; }
    public double CommentsViews { get; set; }
    public TimeSpan MeanStreamingTime { get; set; }
    public double MeanClipViews { get; set; }
    public int SubsGained { get; set; }
    public int SubsLost { get; set; }
}