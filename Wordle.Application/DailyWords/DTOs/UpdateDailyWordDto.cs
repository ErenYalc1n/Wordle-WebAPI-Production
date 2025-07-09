namespace Wordle.Application.DailyWords.DTOs;

public class UpdateDailyWordDto
{
    public DateOnly OldDate { get; set; }
    public DateOnly NewDate { get; set; }
    public string Word { get; set; } = default!;
}
