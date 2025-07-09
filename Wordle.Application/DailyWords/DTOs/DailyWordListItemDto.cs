namespace Wordle.Application.DailyWords.DTOs;
public class DailyWordListItemDto
{
    public string Word { get; set; } = default!;
    public DateOnly Date { get; set; }
}
