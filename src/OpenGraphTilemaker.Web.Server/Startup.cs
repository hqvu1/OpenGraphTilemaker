using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using OpenGraphTilemaker.Web.Client.Features.CryptoWatch;
using OpenGraphTilemaker.Web.Server.ServerApp.Diagnostics;

namespace OpenGraphTilemaker.Web.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<CryptoWatchOptions>(Program.Configuration.GetSection("CryptoWatch"));

            services.AddServerSideBlazor<Client.Startup>();

            services.AddResponseCompression(options => {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.UseResponseCompression();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSerilogClient();

            app.UseServerSideBlazor<Client.Startup>();
        }
    }
}
