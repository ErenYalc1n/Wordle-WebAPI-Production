using Wordle.Domain.Guesses;

namespace Wordle.Application.Common.Interfaces;

public interface IGuessRepository
{
    Task<bool> HasCorrectGuessAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default);
    Task<int> GetGuessCountAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default);
    Task AddAsync(Guess guess, CancellationToken cancellationToken = default);
    Task<bool> IsDuplicateGuessAsync(Guid userId, Guid dailyWordId, string guessText, CancellationToken cancellationToken = default);
    Task<List<Guess>> GetGuessesForUserAndWordAsync(Guid userId, Guid wordId, CancellationToken cancellationToken);

}
