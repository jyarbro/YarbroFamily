using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace App {
    public class Program {
        public const string Version = "1.1.2";

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
