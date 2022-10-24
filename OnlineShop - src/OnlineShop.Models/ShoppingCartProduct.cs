namespace OnlineShop.Models
{
    public class ShoppingCartProduct
    {
        public string ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        public string ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
    }
}