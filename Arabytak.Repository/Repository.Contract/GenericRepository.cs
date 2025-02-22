using Arabytak.Core.Entities;
using Arabytak.Core.Repositories.Contract;
using Arabytak.Core.Specification;
using Arabytak.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arabytak.Repository.Repository.Contract
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ArabytakContext _dbContext;

        public GenericRepository(ArabytakContext DbContext)
        {
            _dbContext = DbContext;
        }
        public async Task AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {
           _dbContext.Remove(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //if (typeof(T) == typeof(Brand))
            //{
            //    return (IReadOnlyList<T>) await _dbContext.Set<Brand>()
            //        .ToListAsync();
            //}
            //return await _dbContext.Set<T>().ToListAsync();
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public Task<T> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

            public async Task<T> GetByIdWithSpecAsync( ISpecification<T> spec)
            {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
            }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async  Task<T> GetById(int id) => await _dbContext.Set<T>().FindAsync(id);

        public async Task<T> GetWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public void UpdateAsync(T entity)
        {
            _dbContext.Update(entity);
        }

        //to avoid the repeat code use this method
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);/*SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec)*/
        }
    }
}
