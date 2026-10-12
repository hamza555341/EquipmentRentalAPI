using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Common.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class;

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
