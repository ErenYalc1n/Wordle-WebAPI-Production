namespace Wordle.Domain.Guesses;

public class Guess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DailyWordId { get; set; }
    public string GuessText { get; set; } = string.Empty;
    public DateTime GuessedAt { get; set; }
    public bool IsCorrect { get; set; }
}
