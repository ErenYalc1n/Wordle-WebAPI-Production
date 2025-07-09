namespace Wordle.Application.Common.Interfaces;

public interface IScoreRepository
{
    Task AddAsync(Score score);
    Task<bool> ExistsForUserAndWordAsync(Guid userId, Guid wordId);

    Task<List<Score>> GetTopScoresAsync(DateOnly fromDate, DateOnly toDate, int topCount);
    Task<Score?> GetUserScoreAsync(Guid userId, DateOnly fromDate, DateOnly toDate);
    Task<Score?> GetByUserAndDateAsync(Guid userId, DateOnly date);
    Task<List<Score>> GetAllByUserAsync(Guid userId);

}
