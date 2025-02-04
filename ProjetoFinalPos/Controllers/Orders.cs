using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinalPos.Models;
using ProjetoFinalPos.Repositories;
using ProjetoFinalPos.Transport;

namespace ProjetoFinalPos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Orders : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Orders(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of orders.</returns>
        /// <response code="200">Returns the list of orders.</response>
        /// <response code="204">If no orders are found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var result = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Client)
                .ToListAsync();

            if (!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order with the specified ID.</returns>
        /// <response code="200">Returns the order with the specified ID.</response>
        /// <response code="404">If the order is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Client)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="order">The updated order object.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the order is successfully updated.</response>
        /// <response code="400">If the ID in the URL does not match the ID in the order object.</response>
        /// <response code="404">If the order is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="orderDto">The order data transfer object containing order details.</param>
        /// <returns>The created order.</returns>
        /// <response code="201">Returns the newly created order.</response>
        /// <response code="400">If the client ID or product ID is invalid, or if there is not enough stock.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> PostOrder(OrderDto orderDto)
        {
            var client = await _context.Clients.FindAsync(orderDto.ClientId);
            if (client == null)
            {
                return BadRequest("Invalid client ID");
            }

            var order = new Order
            {
                OrderDate = orderDto.OrderDate,
                ClientId = orderDto.ClientId,
                Client = client,
                OrderItems = new List<OrderItem>()
            };

            double totalAmount = 0;

            foreach (var item in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Invalid product ID: {item.ProductId}");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    return BadRequest($"Not enough stock for product ID: {item.ProductId}. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                }

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Product = product,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                totalAmount += orderItem.TotalPrice;
                order.OrderItems.Add(orderItem);

                // Subtract the quantity from the stock
                product.StockQuantity -= item.Quantity;

                // Update the product in the context
                _context.Entry(product).State = EntityState.Modified;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        /// <summary>
        /// Deletes an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the order is successfully deleted.</response>
        /// <response code="404">If the order is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
