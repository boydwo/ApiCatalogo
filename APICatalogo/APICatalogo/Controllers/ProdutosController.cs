using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        //injeção de dependencia do serviço
        private readonly AppDbContext _context;
        public ProdutosController(AppDbContext contexto)
        {
            _context = contexto;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            // AsNoTracking() aumenta desempenho
            return _context.Produtos.AsNoTracking().ToList();
        }

        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }
            return produto;
        }

        [HttpPost]
        public ActionResult Post([FromBody]Produto produto)
        {
            //Desde a versão 2.1 essa validação ja é feita
            // if (!ModelState.IsValid)
            // {
            //   return BadRequest(ModelState);
            //  }

            _context.Produtos.Add(produto);
            _context.SaveChanges();
            
            return new CreatedAtRouteResult("ObterProduto",new { id = produto.ProdutoId }, produto);
        }
    }
}
