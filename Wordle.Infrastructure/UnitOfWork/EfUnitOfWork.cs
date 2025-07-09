using Wordle.Domain.Common;
using Wordle.Infrastructure.Data;

namespace Wordle.Infrastructure.Common;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly WordleDbContext _context;

    public EfUnitOfWork(WordleDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
