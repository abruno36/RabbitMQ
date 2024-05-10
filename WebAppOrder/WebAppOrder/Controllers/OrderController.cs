using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using WebAppOrder.Domain;

namespace WebAppOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger) { 
            _logger = logger;
        }

        [HttpPost]
        public IActionResult InsertOrder(Order order)
        {
            try
            {
                #region
                        var factory = new ConnectionFactory { HostName = "localhost" };
                        using var connection = factory.CreateConnection();
                        using var channel = connection.CreateModel();

                        channel.QueueDeclare(queue: "orderQueue",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        string message = JsonSerializer.Serialize(order);
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: string.Empty,
                                             routingKey: "orderQueue",
                                             basicProperties: null,
                                             body: body);
                        Console.WriteLine($" [x] Sent {message}");

                        //Console.WriteLine(" Press [enter] to exit.");
                        //Console.ReadLine();

                #endregion
                return Accepted(order);
            }catch(Exception ex)
            {
                _logger.LogError("Erro ao tentar criar um novo pedido", ex);
                return new StatusCodeResult(500);
            }
            
        }
    }
}
