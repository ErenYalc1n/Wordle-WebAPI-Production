namespace Wordle.Application.DTOs;

public class PreviousGuessDto
{
    public string GuessText { get; set; } = string.Empty;
    public DateTime GuessedAt { get; set; }
}
