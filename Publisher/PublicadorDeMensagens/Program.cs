using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Text;
using System.Text.Json;

namespace PublicadorDeMensagens
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logger =
                new LoggerConfiguration()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                    .CreateLogger();

            logger.Information("Testando o envio de mensagens para uma Fila do RabbitMQ");

            string queueName = "Eventos";

            logger.Information($"Queue = {queueName}");

            try
            {
                ConnectionFactory factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                };
                using IConnection connection = factory.CreateConnection();
                using IModel channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: "Eventos.Mensagem", type: ExchangeType.Fanout);

                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueBind(queue: queueName,
                                  exchange: "Eventos.Mensagem",
                                  routingKey: "");

                Evento transacao = new Evento
                {
                    Mensagem = "Confia que vai ser publicada"
                };

                for (int i = 0; i < 10; i++)
                {
                    channel.BasicPublish(exchange: "Eventos.Mensagem",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(transacao)));

                    logger.Information($"[Mensagem enviada] {JsonSerializer.Serialize(transacao)}");
                }

                logger.Information("Concluido o envio de mensagens");
            }
            catch (Exception ex)
            {
                logger.Error($"Exceção: {ex.GetType().FullName} | " + $"Mensagem: {ex.Message}");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    internal record Evento
    {
        public Evento()
        {
            Id = Guid.NewGuid();
            DataEvento = DateTime.UtcNow;
        }

        public Evento(string dados)
        {
            Id = Guid.NewGuid(); 
            DataEvento = DateTime.UtcNow;
            Mensagem = dados;

        }

        public Guid Id { get; private init; }
        public DateTime DataEvento { get; set; }
        public string Mensagem { get; set; }

    }
}
