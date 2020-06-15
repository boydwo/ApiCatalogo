using APICatalogo.Context;
using APICatalogo.Filter;
using APICatalogo.Models;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController] // facilita para fazer validações dos models
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        public ProdutosController(IUnitOfWork contexto)
        {
            _uof = contexto;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Produto>>Get() 
        {
            return _uof.ProdutoRepository.Get().ToList();

        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)// Task representa uma unica operação que retorna um valor
        {
            // chamdno erros de exceção 
            //throw new Exception("Exception ao retornar produto pelo id");

            var produto =  _uof.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }
            return produto;
        }

        [HttpPost]
        public ActionResult Post([FromBody]Produto produto)
        {
            //Desde a versão 2.1 essa validação ja é feita atrave do [ApiController] 
            // if (!ModelState.IsValid)
            // {
            //   return BadRequest(ModelState);
            //  }

            _uof.Produtos.Add(produto);
            _uof.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();//zx\
            }

            _uof.Entry(produto).State = EntityState.Modified;
            _uof.SaveChanges();
            return Ok();
        }


        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            var produto = _uof.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            // var produto = _uof.Produtos.Find(id);  Se for chave primaria

            if (produto == null)
            {
                return BadRequest();
            }

            _uof.Produtos.Remove(produto);
            _uof.SaveChanges();
            return produto;
        }


    }
}
