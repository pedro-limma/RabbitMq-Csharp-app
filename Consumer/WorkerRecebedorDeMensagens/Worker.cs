using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerRecebedorDeMensagens
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ParametroExecucao _parametroExecucao;

        public Worker(ILogger<Worker> logger,
                      ParametroExecucao ParametroExecucao)
        {
            logger.LogInformation($"Queue = {ParametroExecucao.Queue}");

            _logger = logger;
            _parametroExecucao = ParametroExecucao;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Aguardando mensagens...");

            ConnectionFactory factory = new()
            {
               HostName = _parametroExecucao.ConnectionString
            };

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.QueueDeclare(queue: _parametroExecucao.Queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            EventingBasicConsumer consumer = new(channel);

            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue: _parametroExecucao.Queue,
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker ativo em: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                await Task.Delay(1000, stoppingToken);
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            _logger.LogInformation(
                $"[Nova mensagem | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                Encoding.UTF8.GetString(e.Body.ToArray()));
        }
    }
}
