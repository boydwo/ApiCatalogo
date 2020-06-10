using APICatalogo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APICatalogo.Extensions
{
    public static class ApiExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            // usando o Middleware
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json"; //tipo de retorno

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                          
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Trace = contextFeature.Error.StackTrace

                        }.ToString());
                    }
                });
            });
        }
    }
}
