using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OrderService.Models;
using RabbitMQ.Client;

public class RabbitMQService : IAsyncDisposable
{
    private readonly IConnection? _connection;
    private readonly IChannel? _channel;

    public RabbitMQService()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                AutomaticRecoveryEnabled = true // Habilita reconexão automática
            };

            Console.WriteLine("🔄 Tentando conectar ao RabbitMQ...");

            // Criar conexão e canal de forma assíncrona
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            // Declara o Exchange
            _channel.ExchangeDeclareAsync("order_exchange", ExchangeType.Fanout).GetAwaiter().GetResult();

            Console.WriteLine("✅ Conectado ao RabbitMQ com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao conectar ao RabbitMQ: {ex.Message}");
            throw;
        }
    }

    public async Task PublishMessageAsync(Order order)
    {
        if (_channel is null)
        {
            Console.WriteLine("❌ Canal RabbitMQ não inicializado!");
            return;
        }
        string message = JsonSerializer.Serialize(order);

        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: "order_exchange",
            routingKey: "",
            body: body
        );

        Console.WriteLine($"📨 Mensagem publicada: {message}");
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
                Console.WriteLine("🔌 Canal RabbitMQ fechado.");
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                Console.WriteLine("🔌 Conexão com RabbitMQ fechada.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erro ao fechar RabbitMQ: {ex.Message}");
        }
    }
}
