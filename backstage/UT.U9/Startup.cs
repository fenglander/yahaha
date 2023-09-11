
namespace UT.U9;

[AppStartup(100)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfigurableOptions<UtU9Options>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}