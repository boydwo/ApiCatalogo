using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filter;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
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

            services.AddControllers();

            //Vai ser criada cada vez que for solicitada
            services.AddTransient<IMeuServico, MeuServico>();

            //remove excessao no Json
            services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
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

            //Adiciona o middleware que execua o endpoint do request atual
            app.UseEndpoints(endpoints =>
            {
                // adiciona os endpoints para actions dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}
