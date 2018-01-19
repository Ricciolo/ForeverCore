using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.Use(OnRequest);
        }

        private readonly Dictionary<string, Action<ProcessManager>> _actions = new Dictionary<string, Action<ProcessManager>>(StringComparer.OrdinalIgnoreCase)
        {
            { "/stop", p => p.Stop() },
            { "/restart", p => p.Restart() },
            { "/start", p => p.Start() },
            { "/exit", p => p.Exit() }
        };

        private async Task OnRequest(HttpContext context, Func<Task> next)
        {
            if (context.Request.Method == "GET")
            {
                var process = context.RequestServices.GetRequiredService<ProcessManager>();

                // Look for the command
                if (_actions.TryGetValue(context.Request.Path.Value, out Action<ProcessManager> action))
                {
                    // Execute command asyncronously
                    Task.Run(() => action(process)).GetAwaiter();

                    // Send response
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Done!");
                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Unknown command!");
                }
            }
        }
    }
}
