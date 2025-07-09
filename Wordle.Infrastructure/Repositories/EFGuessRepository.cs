using Microsoft.EntityFrameworkCore;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Guesses;
using Wordle.Infrastructure.Data;

namespace Wordle.Infrastructure.Repositories;

public class EFGuessRepository : IGuessRepository
{
    private readonly WordleDbContext _context;

    public EFGuessRepository(WordleDbContext context)
    {
        _context = context;
    }

    public Task<bool> HasCorrectGuessAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default)
    {
        return _context.Guesses
            .AnyAsync(g => g.UserId == userId && g.DailyWordId == dailyWordId && g.IsCorrect, cancellationToken);
    }

    public Task<int> GetGuessCountAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default)
    {
        return _context.Guesses
            .CountAsync(g => g.UserId == userId && g.DailyWordId == dailyWordId, cancellationToken);
    }

    public Task AddAsync(Guess guess, CancellationToken cancellationToken = default)
    {
        _context.Guesses.Add(guess);
        return Task.CompletedTask;
    }

    public Task<bool> IsDuplicateGuessAsync(Guid userId, Guid dailyWordId, string guessText, CancellationToken cancellationToken = default)
    {
        var normalized = guessText.ToLowerInvariant();
        return _context.Guesses
            .AnyAsync(g =>
                g.UserId == userId &&
                g.DailyWordId == dailyWordId &&
                g.GuessText.ToLower() == normalized,
                cancellationToken);
    }
    public Task<List<Guess>> GetGuessesForUserAndWordAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default)
    {
        return _context.Guesses
            .Where(g => g.UserId == userId && g.DailyWordId == dailyWordId)
            .OrderBy(g => g.GuessedAt)
            .ToListAsync(cancellationToken);
    }

}
