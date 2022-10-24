namespace OnlineShop.Models
{
    public class OrderProduct 
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public string ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}