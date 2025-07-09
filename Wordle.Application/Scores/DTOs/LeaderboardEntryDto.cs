namespace Wordle.Application.Scores.DTOs;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public int Point { get; set; }
}
