using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EquipmentRental.Application.Common.Contracts
{
    public interface IGenericRepository<TEntity>
     where TEntity : class
    {
        Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetByIdAsync(
            object id,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        void Remove(TEntity entity);
    }
}
