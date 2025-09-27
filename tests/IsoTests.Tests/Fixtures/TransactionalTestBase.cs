using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IsoTests.Tests.Fixtures;

public abstract class TransactionalTestBase<TContext> : IAsyncLifetime
    where TContext : DbContext
{
    protected TContext Db { get; private set; } = null!;
    private IDbContextTransaction _transaction = null!;

    private readonly IDbContextFactory<TContext> _factory;

    protected TransactionalTestBase(IDbContextFactory<TContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask InitializeAsync()
    {
        Db = await _factory.CreateDbContextAsync();

        _transaction = await Db.Database.BeginTransactionAsync();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _transaction.RollbackAsync();
        await Db.DisposeAsync();
    }
}
