using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerHub.DataAccessLayer.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IList<T> GetAll();
        T? GetById(long? id);
        void Update(T entity);
        void Delete(T entity);
        IList<T> Get(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void Save();
    }
}