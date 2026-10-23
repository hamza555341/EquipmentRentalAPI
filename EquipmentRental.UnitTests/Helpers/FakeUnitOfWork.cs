using EquipmentRental.Application.Common.Contracts;

namespace EquipmentRental.UnitTests.Helpers;

/// <summary>
/// UnitOfWork وهمي: بيدّي لكل نوع Entity نفس الـ FakeRepository كل مرة،
/// وبيعدّ كام مرة اتنادى SaveChangesAsync (عشان نتأكد إن الحفظ حصل أو ما حصلش).
/// </summary>
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

    // اختصار للتست: عشان نوصل للـ Items من غير Cast
    public FakeRepository<TEntity> Set<TEntity>() where TEntity : class
        => (FakeRepository<TEntity>)Repository<TEntity>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }
}
