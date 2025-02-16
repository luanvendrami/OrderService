namespace OrderService.Models;

public record Order(Guid Id, string Description, decimal Amount);
