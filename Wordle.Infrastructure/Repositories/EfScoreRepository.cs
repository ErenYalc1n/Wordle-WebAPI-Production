using Microsoft.EntityFrameworkCore;
using Wordle.Application.Common.Interfaces;
using Wordle.Infrastructure.Data;

public class EfScoreRepository : IScoreRepository
{
    private readonly WordleDbContext _context;

    public EfScoreRepository(WordleDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Score score)
    {
        await _context.Scores.AddAsync(score);
    }

    public async Task<bool> ExistsForUserAndWordAsync(Guid userId, Guid wordId)
    {
        return await _context.Scores.AnyAsync(s => s.UserId == userId && s.DailyWordId == wordId);
    }

    public async Task<List<Score>> GetTopScoresAsync(DateOnly fromDate, DateOnly toDate, int topCount)
    {
        return await _context.Scores
            .Include(s => s.User)
            .Where(s => s.Date >= fromDate && s.Date <= toDate)
            .GroupBy(s => s.UserId)
            .Select(g => new Score
            {
                UserId = g.Key,
                Point = g.Sum(x => x.Point),
                User = g.First().User
            })
            .OrderByDescending(x => x.Point)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<Score?> GetUserScoreAsync(Guid userId, DateOnly fromDate, DateOnly toDate)
    {
        var totalPoint = await _context.Scores
            .Where(s => s.UserId == userId && s.Date >= fromDate && s.Date <= toDate)
            .SumAsync(s => (int?)s.Point);

        if (totalPoint is null) return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        return new Score
        {
            UserId = userId,
            Point = totalPoint.Value,
            User = user
        };
    }
    public async Task<Score?> GetByUserAndDateAsync(Guid userId, DateOnly date)
    {
        return await _context.Scores
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Date == date);
    }
    public async Task<List<Score>> GetAllByUserAsync(Guid userId)
    {
        return await _context.Scores
            .Include(s => s.User)
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }


}
