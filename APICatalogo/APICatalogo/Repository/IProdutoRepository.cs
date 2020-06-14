using APICatalogo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    interface IProdutoRepository : IRepository<Produto>
    {
        IEnumerable<Produto> GetProdutoPorPreco();


    }
}
