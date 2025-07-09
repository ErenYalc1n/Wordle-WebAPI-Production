namespace Wordle.Application.Scores.DTOs;

public class LeaderboardResponseDto
{
    public List<LeaderboardEntryDto> Top10 { get; set; } = new();
    public LeaderboardEntryDto? CurrentUser { get; set; }
}
