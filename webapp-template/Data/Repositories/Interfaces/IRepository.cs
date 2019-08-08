using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Data.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Zwraca krotkę o podanym id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(int id);
        /// <summary>
        /// Zwraca wszystkie krotki
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();
        /// <summary>
        /// Zwraca krotki spełniające podany w predykacie warunek
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// Zwraca krotki spełniające warunek i posortowane (domyślnie rosnąco)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetOrdered<TKey>(Expression<Func<TEntity, TKey>> predicate, bool descending = false);

        /// <summary>
        /// Dodaje krotkę
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);
        /// <summary>
        /// Dodaje kolekcję krotek
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Usuwa krotkę
        /// </summary>
        /// <param name="entity"></param>
        void Remove(TEntity entity);
        /// <summary>
        /// Usuwa kolekcję krotek
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}