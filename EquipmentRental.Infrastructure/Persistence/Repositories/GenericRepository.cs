using EquipmentRental.Application.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EquipmentRental.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity>
      : IGenericRepository<TEntity>
      where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(
            object id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(
                new[] { id }, cancellationToken);
        }

        public async Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(
                predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(
                predicate, cancellationToken);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
