using EquipmentRental.Application.Common.Contracts;

namespace EquipmentRental.UnitTests.Helpers;


public class FakeUnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();

    public int SaveChangesCallCount { get; private set; }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (!_repositories.TryGetValue(typeof(TEntity), out var repo))
        {
            repo = new FakeRepository<TEntity>();
            _repositories[typeof(TEntity)] = repo;
        }

        return (IGenericRepository<TEntity>)repo;
    }

    public FakeRepository<TEntity> Set<TEntity>() where TEntity : class
        => (FakeRepository<TEntity>)Repository<TEntity>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }
}
