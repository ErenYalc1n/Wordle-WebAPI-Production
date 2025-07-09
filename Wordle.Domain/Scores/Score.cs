using Wordle.Domain.DailyWords;
using Wordle.Domain.Users;

public sealed class Score
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DailyWordId { get; set; }
    public DateOnly Date { get; set; }
    public int Point { get; set; }
    public User User { get; set; } = default!;
    public DailyWord DailyWord { get; set; } = default!;
}

