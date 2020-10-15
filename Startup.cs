using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordBackend.Services;

namespace RecordBackend
{
    public class ConfigureOptions
    {
        public string ConnectionStringUserAPI { get; set; }
    }

    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //private IHostingEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionStringLoginApi = Configuration.GetConnectionString("ConnectionLoginAPI");

            services.AddTransient<IRpcRepository, RpcRepository>();


            //https://toster.ru/q/540665
            services.Configure<ConfigureOptions>(options =>
            {
                options.ConnectionStringUserAPI = connectionStringLoginApi;
                //options.CurrentEnvironment = CurrentEnvironment;
            });

            /*https azure
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });*/

            //services.AddResponseCaching();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env/*, ILoggerFactory loggerFactory*/)
        {
            //loggerFactory.AddConsole();
            /*https azure
            var options = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(options);*/

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            /*else
            {
                app.UseHsts();
                //app.UseExceptionHandler("/Home/Error");
            }*/


            //app.UseHttpsRedirection();

            //https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?tabs=aspnetcore2x&view=aspnetcore-2.2
            /*app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                // For GetTypedHeaders, add: using Microsoft.AspNetCore.Http;
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromMinutes(30)
                    };

                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });*/
       
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
