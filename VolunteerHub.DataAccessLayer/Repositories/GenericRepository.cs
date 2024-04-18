using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected VolunteerHubContext context;

        protected DbSet<T> dataSet;

        public GenericRepository(VolunteerHubContext context)
        {
            this.context = context;
            dataSet = this.context.Set<T>();
        }

        public IList<T> GetAll()
        {
            return dataSet.ToList();
        }

        public T? GetById(long? id)
        {
            if (id == null)
            {
                return null;
            }
            return dataSet.Find(id);
        }
        

        public void Update(T obj)
        {
            dataSet.Update(obj);
        }

        public void Delete(T obj)
        {
            dataSet.Remove(obj);
        }

        public IList<T> Get(Expression<Func<T, bool>> expression)
        {
            return context.Set<T>()
            .Where(expression)
            .ToList();
        }

        public void Add(T entity)
        {
            dataSet.Add(entity);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
