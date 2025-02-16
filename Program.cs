using OrderService.Models;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddSingleton<OrderPublisher>();

var app = builder.Build();

app.MapPost("/orders", async (Order order, OrderPublisher orderPublisher) =>
{
    await orderPublisher.Publish(order);
    return Results.Ok($"Pedido {order.Id} criado com sucesso!");
});

app.Run();
