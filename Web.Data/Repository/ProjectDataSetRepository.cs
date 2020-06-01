using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Web.Data.Contract;
using Web.Data.Model;

namespace Web.Data.Repository
{
    public class ProjectDataSetRepository : IProjectDataSetRepository
    {
        private readonly ApplicationDbContext context = new ApplicationDbContext();

        public IQueryable<ProjectDataSet> All => context.ProjectDataSets;

        public IQueryable<ProjectDataSet> AllIncluding(params Expression<Func<ProjectDataSet, object>>[] includeProperties)
        {
            IQueryable<ProjectDataSet> query = context.ProjectDataSets;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProjectDataSet Find(int id)
        {
            return context.ProjectDataSets.Find(id);
        }

        public void InsertOrUpdate(ProjectDataSet apiCredential)
        {
            if (apiCredential.ProjectDataSetId == default(int))
            {
                // New entity
                context.ProjectDataSets.Add(apiCredential);
            }
            else
            {
                // Existing entity
                context.Entry(apiCredential).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var projectDataSet = context.ProjectDataSets.Find(id);
            context.ProjectDataSets.Remove(projectDataSet);
        }

        public void Delete(ProjectDataSet projectDataSet)
        {
            context.ProjectDataSets.Remove(projectDataSet);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}