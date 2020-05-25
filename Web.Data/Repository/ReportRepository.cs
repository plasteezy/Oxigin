using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Web.Data.Contract;
using Web.Data.Model;

namespace Web.Data.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext context = new ApplicationDbContext();

        public string UserId { get; set; }

        public IQueryable<SavedQuery> All => context.SavedQueries;

        public IQueryable<SavedQuery> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SavedQuery> AllIncluding(params Expression<Func<SavedQuery, object>>[] includeProperties)
        {
            IQueryable<SavedQuery> query = context.SavedQueries;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public SavedQuery Find(int id)
        {
            return context.SavedQueries.Find(id);
        }

        public void InsertOrUpdate(SavedQuery entity)
        {
            if (entity.Id == default(int))
            {
                // New entity
                context.SavedQueries.Add(entity);
            }
            else
            {
                // Existing entity
                context.Entry(entity).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var tsqlquery = context.SavedQueries.Find(id);
            context.SavedQueries.Remove(tsqlquery);
        }

        public void Delete(SavedQuery entity)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}