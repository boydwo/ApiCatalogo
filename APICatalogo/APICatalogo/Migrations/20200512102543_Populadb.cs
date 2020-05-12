using Microsoft.EntityFrameworkCore.Migrations;

namespace APICatalogo.Migrations
{
    public partial class Populadb : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Bebidas','http://macoratti.net/Imagens/1.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Lanches','http://macoratti.net/Imagens/2.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Sobremesas','http://macoratti.net/Imagens/3.jpg')");


            mb.Sql("Insert into Produtos(Nome, Descricao,Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) Values('Cocacola Diet'," +
                "'Reefrigerante de Cola 350ml', 5.45, 'http://macoratti.net/Imagens/coca.jpg',50, now()," +
                "(Select CategoriaId from Categorias where Nome='Bebidas'))");

            mb.Sql("Insert into Produtos(Nome, Descricao,Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) Values('Lanche de Atum'," +
                "'Lanche de Atum com maionese', 8.50, 'http://macoratti.net/Imagens/atum.jpg',10, now()," +
                "(Select CategoriaId from Categorias where Nome='Lanches'))");

            mb.Sql("Insert into Produtos(Nome, Descricao,Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) Values('Pudim 100g'," +
                "'Pudim de Leite Condensado', 6.75, 'http://macoratti.net/Imagens/pudim.jpg',20, now()," +
                "(Select CategoriaId from Categorias where Nome='Sobremesas'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
            mb.Sql("Delete from Produtos");
        }
    }
}
