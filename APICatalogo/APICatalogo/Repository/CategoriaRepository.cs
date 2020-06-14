using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext contexto) : base(contexto)
        {

        }
        public IEnumerable<Categoria> GetCategoriasProdutos()
        {
            return Get().Include(x => x.Produtos);
        }
    }
}
