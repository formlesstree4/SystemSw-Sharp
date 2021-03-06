using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SystemSw_Api.Models;
using SystemSw_Core.Extron;
using SystemSw_Core.Extron.Devices;

namespace SystemSw_Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SystemSW WebAPI", Version = "v1" });
            });
            services.AddSingleton<ICommunicationDevice>((provider) => 
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var extronCfg = new ExtronConfiguration();
                config.GetSection("Extron").Bind(extronCfg);
                if (extronCfg.Type.Equals("serial", System.StringComparison.OrdinalIgnoreCase))
                {
                    return new SerialCommunicationDevice(extronCfg.Port, false, extronCfg.ReadTimeout);
                }
                else
                {
                    return new TestCommDevice();
                }
            });
            services.AddSingleton<ExtronCommunicator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SystemSW WebAPI v1"));
            }
            app.UseCors((b) => {
                b.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
