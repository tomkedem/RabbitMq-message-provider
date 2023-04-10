using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQTest.Models;
using System.Text;

namespace RabbitMQTest.Repository
{
    public class ItemRepository : IItemRepository
    {
        public void DeleteItem(Item request)
        {           
            Publish(request);
        }

       
        public void Publish(Item item)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
           
            using (var connection = factory.CreateConnection())
            
            using (var channel = connection.CreateModel())
            {
                var policy = new Dictionary<string, object>
                {
                    { "ha-mode", "all" }, // High availability mode
                    { "ha-sync-mode", "automatic" }, // Synchronous mirroring mode
                    { "ha-promote-on-failure", "1" }, // Automatic promotion of mirrors to master
                    { "message-ttl", 5000 } // TTL in milliseconds (5 seconds)
                };
                var jsonPolicy = JsonConvert.SerializeObject(policy);
                var properties = channel.CreateBasicProperties();
                properties.Expiration = "5000"; // Message TTL in milliseconds

                
                var queueName = "DeleteItemTest";

                channel.ExchangeDeclare(exchange: queueName,
                         type: ExchangeType.Fanout,
                         durable: true,
                         arguments: new Dictionary<string, object>
                         {
                             { "x-ha-policy", "nodes" },
                             { "x-ha-policy-params", jsonPolicy }
                         });
                channel.QueueDeclare(queueName, false, false, false, policy);

                var message = JsonConvert.SerializeObject(item);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", queueName, properties, body);
            }
        }
    }
}
