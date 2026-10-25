using System.Linq.Expressions;
using EquipmentRental.Application.Common.Contracts;

namespace EquipmentRental.UnitTests.Helpers;


public class FakeRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    public List<TEntity> Items { get; } = new();

    public TEntity Seed(TEntity entity)
    {
        if (GetId(entity) == 0)
        {
            var nextId = Items.Count == 0 ? 1 : Items.Max(GetId) + 1;
            typeof(TEntity).GetProperty("Id")!.SetValue(entity, nextId);
        }

        Items.Add(entity);
        return entity;
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Seed(entity);
        return Task.CompletedTask;
    }

    public Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var wanted = Convert.ToInt32(id);
        return Task.FromResult(Items.FirstOrDefault(e => GetId(e) == wanted));
    }

    public Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(predicate.Compile()));

    public Task<IReadOnlyList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TEntity> result = filter is null
            ? Items.ToList()
            : Items.Where(filter.Compile()).ToList();

        return Task.FromResult(result);
    }

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Any(predicate.Compile()));

    public void Remove(TEntity entity) => Items.Remove(entity);

    private static int GetId(TEntity entity)
        => (int)typeof(TEntity).GetProperty("Id")!.GetValue(entity)!;
}
