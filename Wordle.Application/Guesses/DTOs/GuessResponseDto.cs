namespace Wordle.Application.DTOs;

public class GuessResponseDto
{
    public Guid GuessId { get; set; }
    public string GuessText { get; set; } = string.Empty;
    public List<LetterResult> LetterResults { get; set; } = new();

    public int CurrentAttempt { get; set; }
    public int RemainingAttempts { get; set; }

    public bool IsCorrect { get; set; } 
    public List<PreviousGuessDto> PreviousGuesses { get; set; } = new();
}

public class LetterResult
{
    public char Letter { get; set; }
    public string Status { get; set; } = string.Empty;
}
