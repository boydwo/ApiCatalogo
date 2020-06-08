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
    [ApiController] // facilita para fazer validações dos models
    public class ProdutosController : ControllerBase
    {
        //injeção de dependencia do serviço
        private readonly AppDbContext _context;
        public ProdutosController(AppDbContext contexto)
        {
            _context = contexto;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get() //para operções que idependem do sistema usar async/await
        {
            // AsNoTracking() aumenta desempenho
            return await _context.Produtos.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<Produto>> Get(int id)// Task representa uma unica operação que retorna um valor
        {
            var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id);
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

        [HttpPut("{id}")]
        public ActionResult Put(int id,[FromBody] Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest();//zx\
            }

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }


        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
             var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            // var produto = _context.Produtos.Find(id);  Se for chave primaria
            
            if (produto == null)
            {
                return BadRequest();
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return produto;
        }


    }
}
