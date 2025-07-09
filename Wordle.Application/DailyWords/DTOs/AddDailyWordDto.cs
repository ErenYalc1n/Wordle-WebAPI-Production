namespace Wordle.Application.DailyWords.DTOs;

public class AddDailyWordDto
{
    public string Word { get; set; } = default!;
    public DateOnly Date { get; set; }
}
