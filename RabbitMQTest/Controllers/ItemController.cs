using Microsoft.AspNetCore.Mvc;
using RabbitMQTest.Models;
using RabbitMQTest.Repository;

namespace RabbitMQTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IItemRepository _repository;

        public ItemController(ILogger<ItemController> logger, IItemRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        public IActionResult DeleteItem([FromBody] Item item)
        {
            _repository.DeleteItem(item);
            return Ok(item);
        }

        
    }
}