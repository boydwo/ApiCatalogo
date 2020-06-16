
using System.Collections.Generic;
using System.Linq;
using APICatalogo.Models;
using APICatalogo.Repository;
using APICatalogo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(IUnitOfWork contexto, IConfiguration config, ILogger<CategoriasController> logger)
        {
            _context = contexto;
            _configuration = config;
            _logger = logger;
        }

        [HttpGet("autor")]
        public string GetAutor()
        {
            _logger.LogInformation(" ======= GET api/categorias/produtos  ========= ");
            var autor = _configuration["autor"];
            //var conexao = _configuration["ConnectionStrings: DefaultConnection"];

            return $"Autor: {autor}";// Conexao: {conexao}; 
        }

        //utilizando FromService
        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuservico, string nome)
        {
            return meuservico.Saudacao(nome);
        }

        //novo endpoint
        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            // retorna dados relacionados
            return _context.CategoriaRepository.GetCategoriasProdutos().ToList();
        }



        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            return _context.CategoriaRepository.Get().ToList();
        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _context.CategoriaRepository.GetById(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound();
            }
            return categoria;
        }


        [HttpPost]
        public ActionResult Post([FromBody]Categoria categoria)
        {
            _context.CategoriaRepository.Add(categoria);
            _context.Commit();

            return new CreatedAtRouteResult("ObterProduto", new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest();
            }

            _context.CategoriaRepository.Update(categoria);
            _context.Commit();
            return Ok();
        }


        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            var categoria = _context.CategoriaRepository.GetById(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound();
            }

            _context.CategoriaRepository.Delete(categoria);
            _context.Commit();
            return categoria;
        }
    }
}