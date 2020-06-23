using System;
using System.Collections.Generic;
using System.Linq;
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

            // Registrando o context como serviço e o BD => appsettings.json
            services.AddDbContext<AppDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // habilitando o Identity no projeto
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //JWT
            // Adiciona o manipulador de autenticacao e define o esquema de autenticaçao usado: Bearer
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

            //adiciona o middleware de autenticação
            app.UseAuthentication();

            // adiciona middleware que habilita a autorização
            app.UseAuthorization();

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
