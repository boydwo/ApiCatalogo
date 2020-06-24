using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filter;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace APICatalogo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // incluindo CORS
            services.AddCors();

            // Registrando o mapeamento do DTO
            var mappingConfig = new MapperConfiguration(mc =>
           {
               mc.AddProfile(new MappingProfile());
           });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Registrando Logging
            services.AddScoped<ApiLoggingFilter>();

            // Registrando o context como servi�o e o BD => appsettings.json
            services.AddDbContext<AppDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // habilitando o Identity no projeto
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //JWT
            // Adiciona o manipulador de autenticacao e define o esquema de autentica�ao usado: Bearer
            // Valida o emissor, a audiencia e a chave usando a chave secreta e valida a assinatura
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = Configuration["TokenConfiguration:Audience"],
                    ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:key"]))
                });

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "APICatalogo",
                    Description = "Cat�logo de produtos e Categorias",
                    TermsOfService = new Uri("https://marcos-tulio-rodrigues.net/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Paj�",
                        Email = "macoratti@yahoo.com",
                        Url = new Uri("https://marcos-tulio-rodrigues.net/terms"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Usar sobre LICX",
                        Url = new Uri("https://marcos-tulio-rodrigues.net/terms")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddControllers();

            //Vai ser criada cada vez que for solicitada
            services.AddTransient<IMeuServico, MeuServico>();

            //remove excessao no Json
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // adicionando midlleare de tratamentos de erro
            app.ConfigureExceptionHandler();

            // adiciona middlware para redirecionar https
            app.UseHttpsRedirection();

            // adiciona middleware de roteamento
            app.UseRouting();

            //adiciona o middleware de autentica��o
            app.UseAuthentication();

            // adiciona middleware que habilita a autoriza��o
            app.UseAuthorization();

            //Swagger
            app.UseSwagger();

            //SwaggerUI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Cat�logo de Produtos e Categorias");
            });

            //adicionando CORS
            //  restringindo CORS  app.UseCors(opt => opt.WithOrigins("https://www.apirequest.io/").WithMethods("GET"));
            app.UseCors(op => op.AllowAnyOrigin()); // permite qualquer origem.

            //Adiciona o middleware que execua o endpoint do request atual
            app.UseEndpoints(endpoints =>
            {
                // adiciona os endpoints para actions dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}
