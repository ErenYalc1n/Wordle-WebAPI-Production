namespace Wordle.Application.DailyWords.DTOs;

public class DailyWordListResultDto
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public List<DailyWordListItemDto> Words { get; set; } = new();
}
