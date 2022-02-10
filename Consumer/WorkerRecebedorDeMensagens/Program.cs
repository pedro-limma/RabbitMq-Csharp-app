using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkerRecebedorDeMensagens
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("*** Testando o consumo de mensagens com RabbitMQ + Filas ***");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ParametroExecucao>(
                        new ParametroExecucao()
                        {
                            ConnectionString = "localhost",
                            Queue = "Lancamento"
                        });
                    services.AddHostedService<Worker>();
                });
    }
}
