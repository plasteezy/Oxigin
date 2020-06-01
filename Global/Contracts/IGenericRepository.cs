using System;
using System.Linq;
using System.Linq.Expressions;

namespace Global.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> All { get; }

        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// When implemented, returns an entity of type T
        /// </summary>
        /// <param name="id">int: unique identifier</param>
        /// <returns>returns the found entity of type T as a task</returns>
        T Find(int id);

        void InsertOrUpdate(T apiCredential);

        /// <summary>
        /// When implemented, deletes an entity
        /// </summary>
        /// <param name="id">int: unique identifier</param>
        /// <returns>returns a task</returns>
        void Delete(int id);

        /// <summary>
        /// When implemented, deletes an entity
        /// </summary>
        /// <param name="entity">T: entity to be deleted</param>
        /// <returns>returns a task</returns>
        void Delete(T entity);

        void Save();
    }
}