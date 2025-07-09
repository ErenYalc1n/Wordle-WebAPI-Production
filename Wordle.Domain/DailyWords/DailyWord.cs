namespace Wordle.Domain.DailyWords
{
    public sealed class DailyWord
    {
        public Guid Id { get; set; }
        public string Word { get; set; } = default!;
        public DateTime Date { get; set; }
    }
}
