
using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _context;

        // injetando o contexto
        public Repository(AppDbContext contexto)
        {
            _context = contexto;
        }

        // Lista de entidades
        public IQueryable<T> Get()
        {
            return _context.Set<T>().AsNoTracking();
        }

        // Retornando uma entidade, usado um delegate lambda e um predicate pra validar o criterio
        public T GetById(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().SingleOrDefault(predicate);
        }


        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
        }

    }
}
