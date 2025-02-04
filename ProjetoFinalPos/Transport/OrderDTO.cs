namespace ProjetoFinalPos.Transport
{
    public class OrderDto
    {
        public DateTime OrderDate { get; set; }
        public int ClientId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
