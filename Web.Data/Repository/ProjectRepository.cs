using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Web.Data.Contract;
using Web.Data.Model;

namespace Web.Data.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext context = new ApplicationDbContext();

        public IQueryable<Project> All => context.Projects;

        public IQueryable<Project> AllIncluding(params Expression<Func<Project, object>>[] includeProperties)
        {
            IQueryable<Project> query = context.Projects;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Project Find(int id)
        {
            return context.Projects.Find(id);
        }

        public void InsertOrUpdate(Project project)
        {
            if (project.ProjectId == default(int))
            {
                // New entity
                context.Projects.Add(project);
            }
            else
            {
                // Existing entity
                context.Entry(project).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var project = context.Projects.Find(id);
            context.Projects.Remove(project);
        }

        public void Delete(Project project)
        {
            context.Projects.Remove(project);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}