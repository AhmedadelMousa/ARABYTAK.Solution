using Arabytak.Core.Entities;
using Arabytak.Core.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arabytak.Core.Repositories.Contract
{
    public interface IGenericRepository <T> where T : BaseEntity
    {
        Task<T> GetAsync(int id);
        Task<T> GetByIdWithSpecAsync( ISpecification<T> speci);
       Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<T> GetWithSpecAsync(ISpecification<T> spec);
        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task<int> GetCountAsync(ISpecification<T> spec);
        Task<T> GetById(int id);
    }
}
