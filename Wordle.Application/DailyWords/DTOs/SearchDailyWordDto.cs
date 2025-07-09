namespace Wordle.Application.DailyWords.DTOs;

public class SearchDailyWordDto
{
    public Guid Id { get; set; }
    public string Word { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string Status { get; set; } = default!;
}
