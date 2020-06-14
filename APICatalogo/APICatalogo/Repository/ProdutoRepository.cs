using APICatalogo.Context;
using APICatalogo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {

        public ProdutoRepository(AppDbContext contexto): base(contexto)
        {

        }


        public IEnumerable<Produto> GetProdutoPorPreco()
        {
            return Get().OrderBy(c => c.Preco).ToList();
        }
    }
}
