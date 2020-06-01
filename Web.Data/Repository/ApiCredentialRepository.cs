using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Web.Data.Contract;
using Web.Data.Model;

namespace Web.Data.Repository
{
    public class ApiCredentialRepository : IApiCredentialRepository
    {
        private readonly ApplicationDbContext context = new ApplicationDbContext();

        public IQueryable<ApiCredential> All
        {
            get { return context.ApiCredentials; }
        }

        public IQueryable<ApiCredential> AllIncluding(
            params Expression<Func<ApiCredential, object>>[] includeProperties)
        {
            IQueryable<ApiCredential> query = context.ApiCredentials;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public ApiCredential Find(int id)
        {
            return context.ApiCredentials.Find(id);
        }

        public void InsertOrUpdate(ApiCredential apiCredential)
        {
            if (apiCredential.ApiCredentialId == default(int))
            {
                // New entity
                context.ApiCredentials.Add(apiCredential);
            }
            else
            {
                // Existing entity
                context.Entry(apiCredential).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var apiCredential = context.ApiCredentials.Find(id);
            context.ApiCredentials.Remove(apiCredential);
        }

        public void Delete(ApiCredential apiCredential)
        {
            context.ApiCredentials.Remove(apiCredential);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}