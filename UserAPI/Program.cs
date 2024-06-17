using UserAPI.Data;
using Microsoft.Extensions.DependencyInjection;
namespace UserAPI
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DataContext>();

            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}"); 
            });

            app.Run();
        }
    }
}
