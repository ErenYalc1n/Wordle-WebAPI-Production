namespace Wordle.Application.Scores.DTOs;

public class MyScoreDto
{
    public string Nickname { get; set; } = string.Empty;
    public int TotalPoint { get; set; }
    public int GameCount { get; set; }
    public double AveragePoint { get; set; }
}
