using System.Text.Json;
using OrderService.Models;

namespace OrderService.Services;

public class OrderPublisher
{
    private readonly RabbitMQService _rabbitMQService;

    public OrderPublisher(RabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public async Task Publish(Order order)
    {
        await _rabbitMQService.PublishMessageAsync(order);
    }
}
