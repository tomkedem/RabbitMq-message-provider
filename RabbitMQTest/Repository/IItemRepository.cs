using RabbitMQTest.Models;

namespace RabbitMQTest.Repository
{
    public interface IItemRepository
    {
        void DeleteItem(Item item);
    }
}