using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Sandbox.DataAccess;
using Sandbox.DataAccess.DataBase;
using Sandbox.Services;
using Sandbox.Services.Implementations;

namespace Sandbox
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
            services.AddLogging(builder => builder.ClearProviders().AddConsole());
            services.AddControllers().AddNewtonsoftJson();

            services.AddHealthChecks().AddDbContextCheck<PostgresDbContext>();
            services.Configure<PostgresOptions>(Configuration.GetSection(nameof(PostgresOptions)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FunctionalTestingSandbox", Version = "v1"});
            });

            services.AddDbContext<PostgresDbContext>();
            services.AddTransient<IDataAccess, PostgresDataAccess>();

            services.AddHttpClient("google");
            services.AddHttpClient(string.Empty);
            
            services.Configure<ClientConfig>(Configuration.GetSection($"Kafka{nameof(ClientConfig)}"));
            services.AddTransient<IKafkaClientService, KafkaClientService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FunctionalTestingSandbox v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}