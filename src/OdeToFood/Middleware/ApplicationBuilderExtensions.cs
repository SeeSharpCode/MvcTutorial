using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNodeModules(this IApplicationBuilder app, string root)
        {
            string path = Path.Combine(root, "node_modules");

            PhysicalFileProvider provider = new PhysicalFileProvider(path);

            StaticFileOptions options = new StaticFileOptions();
            options.RequestPath = "/node_modules";
            options.FileProvider = provider;

            app.UseStaticFiles(options);

            return app;
        }
    }
}
