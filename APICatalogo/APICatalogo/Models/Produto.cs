using APICatalogo.validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto : IValidatableObject
    {

        [Key]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatorio")]
        [StringLength(20, ErrorMessage = "O nome deve ter entre 5 e 20 caracters", MinimumLength = 5)]
        [PrimeiraLetraMaiuscula] // chamando classe de Attributo de validação
        public string Nome { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "A descrição deve ter no maximo {1} caracteres")]
        public string Descricao { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8, 2)")] // definindo o formato da coluna no BD
        [Range(1,10000, ErrorMessage = "O preço deve estar entre {1} e {2}")]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 10)]
        public string ImagemUrl { get; set; }

        public float Estoque { get; set; }

        public DateTime DataCadastro { get; set; }


        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(this.Nome))
            {
                var primeiraLetra = this.Nome[0].ToString();
                if(primeiraLetra != primeiraLetra.ToUpper())

                {
                    //  indica que é um retornador
                    yield return new ValidationResult("A primeira letra do produto deve ser maiuscula",
                        new[]
                        {
                            nameof(this.Nome)
                        });
                }
            }

            if(this.Estoque <= 0)
            {
                yield return new ValidationResult("o Estoque deve ser maior que 0",
                    new[]
                    {
                        nameof(this.Nome)
                    });
            }
        }
    }
}
