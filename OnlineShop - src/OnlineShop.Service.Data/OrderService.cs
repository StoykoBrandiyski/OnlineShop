using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Models.Enums;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Service.Data
{
    public class OrderService : IOrderService
    {
        private const int BULGARIAN_HOURS_FROM_UTC_TIME = 2;

        private readonly OnlineShopDbContext dbContext;
        private readonly IUserService userService;
        private readonly IShoppingCartsService shoppingCartsService;

        public OrderService(OnlineShopDbContext dbContext, IUserService userService, IShoppingCartsService shoppingCartsService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
            this.shoppingCartsService = shoppingCartsService;
        }


        public void CompleteProcessingOrder(string username, bool isPartnerOrAdmin)
        {
            Order order = this.GetProcessingOrder(username);

            if (order == null)
            {
                return;
            }

            var shoppingCartProducts = this.shoppingCartsService.GetAllShoppingCartProducts(username).ToList();

            if (shoppingCartProducts.Count == 0 || shoppingCartProducts == null)
            {
                return;
            }

            List<OrderProduct> orderProducts = new List<OrderProduct>();

            foreach (var shopProduct in shoppingCartProducts)
            {
                OrderProduct orderProduct = new OrderProduct
                {
                    Order = order,
                    Product = shopProduct.Product,
                    Quantity = shopProduct.Quantity,
                    Price = (isPartnerOrAdmin ? shopProduct.Product.ParnersPrice : shopProduct.Product.Price)
                    //Price = shopProduct.Product.Price
                };

                orderProducts.Add(orderProduct);
            }

            this.shoppingCartsService.DeleteAllProductFromShoppingCart(username);

            order.OrderDate = DateTime.UtcNow.AddHours(BULGARIAN_HOURS_FROM_UTC_TIME);
            order.Status = OrderStatus.Unprocessed;
            order.PaymentStatus = PaymentStatus.Unpaid;
            order.OrderProducts = orderProducts;
            order.TotalPrice = order.OrderProducts.Sum(x => x.Quantity * x.Price);
            order.InvoiceNumber = order.Id.ToString().PadLeft(10, '0');

            this.dbContext.SaveChanges();
        }
        

        public Order CreateOrder(string username)
        {
            ShopUser user = this.userService.GetUserByUsername(username);

            if (user == null)
            {
                return null;
            }

            Order order = new Order
            {
                Status = OrderStatus.Processing,
                ShopUser = user
            };

            this.dbContext.Orders.Add(order);
            this.dbContext.SaveChanges();

            return order;
        }

        public void DeliverOrder(int id)
        {
            Order orderDb = this.dbContext.Orders
                .FirstOrDefault(order => order.Id == id
                && order.Status == OrderStatus.Processed);

            if(orderDb == null)
            {
                return;
            }

            orderDb.Status = OrderStatus.Delivered;
            orderDb.DeliveryDate = DateTime.UtcNow.AddHours(BULGARIAN_HOURS_FROM_UTC_TIME);
            this.dbContext.SaveChanges();

        }

        public IEnumerable<Order> GetDeliveredOrders()
        {
            var orders = this.dbContext.Orders.Include(o => o.DeliveryAddress)
                        .ThenInclude(o => o.City)
                        .Include(x => x.OrderProducts)
                        .Where(x => x.Status == OrderStatus.Delivered);

            return orders;
        }

        public Order GetOrderById(int orderId)
        {
            Order orderDb = this.dbContext.Orders.Include(order => order.DeliveryAddress)
                        .ThenInclude(address => address.City)
                        .Include(order => order.ShopUser)
                        .ThenInclude(user => user.Company)
                        .FirstOrDefault(order => order.Id == orderId);

            return orderDb;
        }

        public IEnumerable<Order> GetProcessedOrders()
        {
            var orders = this.dbContext.Orders.Include(order => order.DeliveryAddress)
                       .ThenInclude(address => address.City)
                       .Include(order => order.OrderProducts)
                       .Where(x => x.Status == OrderStatus.Processed);

            return orders;
        }

        //N + 1 problem ?
        public Order GetProcessingOrder(string username)
        {
            ShopUser user = this.userService.GetUserByUsername(username);

            if (user == null)
            {
                return null;
            }
            
            // Maybe improve user.Orders instead.
            Order orderDb = this.dbContext.Orders.Include(order => order.DeliveryAddress)
                                      .ThenInclude(address => address.City)
                                      .Include(order => order.OrderProducts)
                                      .FirstOrDefault(order => order.ShopUser.UserName == username && order.Status == OrderStatus.Processing);
            
            return orderDb;
        }

        public IEnumerable<Order> GetUnprocessedOrders()
        {
            var orders = this.dbContext.Orders.Include(order => order.DeliveryAddress)
                          .ThenInclude(address => address.City)
                          .Include(order => order.OrderProducts)
                          .Where(x => x.Status == OrderStatus.Unprocessed);

            return orders;
        }
        
        public Order GetUserOrderById(int orderId, string username)
        {
            Order orderDb = this.dbContext.Orders.Include(order => order.DeliveryAddress)
                    .ThenInclude(address => address.City)
                    .Include(order => order.ShopUser)
                    .ThenInclude(user => user.Company)
                    .FirstOrDefault(order => order.Id == orderId 
                        && order.ShopUser.UserName == username);

            return orderDb;
        }

        //N + 1 problem ?
        public IEnumerable<Order> GetUserOrders(string name)
        {
            var ordersDb = this.dbContext.Orders
                .Where(order => order.ShopUser.UserName == name
                && order.Status != OrderStatus.Processing)
                .ToArray();

            return ordersDb;
        }

        public IEnumerable<OrderProduct> OrderProductsByOrderId(int id)
        {
            var orderProducts = this.dbContext.OrderProducts
                .Include(order => order.Product)
                .ThenInclude(product => product.Images)
                .Where(order => order.OrderId == id)
                .ToArray();

            return orderProducts;
        }

        public void ProcessOrder(int id)
        {
            Order orderDb = this.dbContext.Orders
                .FirstOrDefault(order => order.Id == id
                && order.Status == OrderStatus.Unprocessed || order.Status == OrderStatus.Delivered);

            if (orderDb == null)
            {
                return;
            }

            orderDb.Status = OrderStatus.Processed;
            orderDb.DispatchDate = DateTime.UtcNow.AddHours(BULGARIAN_HOURS_FROM_UTC_TIME);
            this.dbContext.SaveChanges();
        }
        
        public bool SetOrderDetails(Order order, string fullName, string phoneNumber, PaymentType paymentType, int deliveryAddressId, decimal deliveryPrice)
        {
            if (order == null || string.IsNullOrWhiteSpace(fullName) 
                || string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            order.Recipient = fullName;
            order.RecipientPhoneNumber = phoneNumber;
            order.PaymentType = paymentType;
            order.DeliveryAddressId = deliveryAddressId;
            order.DeliveryPrice = deliveryPrice;

            this.dbContext.Update(order);
            this.dbContext.SaveChanges();

            return true;
        }
        
        public bool SetOrderStatusByInvoice(string invoiceNumber, string status)
        {
            var isOrderStatus = Enum.TryParse<PaymentStatus>(status, true, out PaymentStatus paymentStatus);
            var order = this.dbContext.Orders.FirstOrDefault(x => x.InvoiceNumber == invoiceNumber);

            if (order == null || !isOrderStatus)
            {
                return false;
            }

            order.PaymentStatus = (PaymentStatus)paymentStatus;
            this.dbContext.SaveChanges();
            return true;
        }

    }
}
