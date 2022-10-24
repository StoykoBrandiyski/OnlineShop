using Moq;
using OnlineShop.Models;
using OnlineShop.Models.Enums;
using OnlineShop.Service.Data;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Tests.Services.Common;
using OnlineShop.Web.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShop.Tests.Services.Data
{
    public class OrderServiceTests
    {
        private readonly OnlineShopDbContext context;
        private IOrderService orderService;
        private Mock<IUserService> mockUserService;
        private Mock<IShoppingCartsService> mockShoppingCartsService;
        
        public OrderServiceTests()
        {
            this.context = OnlineShopDbContextInMemoryFactory.InitializeContext(); 
            this.mockUserService = new Mock<IUserService>();
            this.mockShoppingCartsService = new Mock<IShoppingCartsService>();

            this.orderService = new OrderService(this.context, 
                this.mockUserService.Object, this.mockShoppingCartsService.Object);
        }

        [Fact]
        public void CreateOrder_ShouldReturnOrder()
        {
            //Arrange
            string username = "Gosho";

            var user = new ShopUser
            {
                UserName = username
            };

            context.Users.Add(user);
            context.SaveChanges();
            
            this.mockUserService.Setup(x => x.GetUserByUsername(username)).Returns(user);
           
            //Act
            Order actualOrder = this.orderService.CreateOrder(username);

            //Assert
            Assert.NotEmpty(context.Orders);
            Assert.NotNull(actualOrder);
            Assert.Equal(OrderStatus.Processing, actualOrder.Status);
            Assert.Equal(username, actualOrder.ShopUser.UserName);
        }

        [Fact]
        public void CreateOrder_WithInvalidUsername_ShouldReturnNull()
        {
            //Arrange
            string username = "Gosho";

            //Act
            Order actualOrder = this.orderService.CreateOrder(username);

            //Assert
            Assert.Empty(context.Orders);
            Assert.Null(actualOrder);
        }

        [Fact]
        public void DeliverOrder_ShouldChangeToDeliveryStatus()
        {
            //Arrange
            var order = new Order
            {
                Status = OrderStatus.Processed
            };

            context.Orders.Add(order);
            context.SaveChanges();
            
            //Act
            this.orderService.DeliverOrder(order.Id);

            //Assert
            Assert.Equal(OrderStatus.Delivered, order.Status);
        }
        
        [Fact]
        public void DeliverOrder_WithInvalidOrderId_ShouldNotChange()
        {
            //Arrange
            var order = new Order
            {
                Status = OrderStatus.Processed
            };

            context.Orders.Add(order);
            context.SaveChanges();
            
            //Act
            this.orderService.DeliverOrder(4);

            //Assert
            Assert.Equal(OrderStatus.Processed, order.Status);
        }
        
        [Fact]
        public async Task GetDeliveredOrders_ShouldReturnCollection()
        {
            //Arrange
            Address address = new Address
            {
                Street = "Todor Kableshkov",
                City = new City { Name = "Plovdiv" }
            };
            
            var orders = new List<Order>
            {
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed},

            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
            
            //Act
            var actualOrders = this.orderService.GetDeliveredOrders();

            //Assert
            Assert.NotEmpty(actualOrders);
            Assert.Equal(2, actualOrders.Count());
            foreach (var item in actualOrders)
            {
                Assert.Equal(OrderStatus.Delivered, item.Status);
            }
        }

        [Fact]
        public async Task GetDeliveredOrders_WithWithoutDeliverOrders_ShouldReturnEmptyCollection()
        {
            //Arrange
            Address address = new Address
            {
                Street = "Todor Kableshkov",
                City = new City { Name = "Plovdiv" }
            };

            var orders = new List<Order>
            {
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed},
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
            
            //Act
            var actualOrders = this.orderService.GetDeliveredOrders();

            //Assert
            Assert.NotEmpty(context.Orders);
            Assert.Empty(actualOrders);
        }

        [Fact]
        public async Task GetProcessedOrders_ShouldReturnCollection()
        {
            //Arrange
            Address address = new Address
            {
                Street = "Todor Kableshkov",
                City = new City { Name = "Plovdiv" }
            };

            var orders = new List<Order>
            {
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed},

            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            //Act
            var actualOrders = this.orderService.GetProcessedOrders();

            //Assert
            Assert.NotEmpty(actualOrders);
            Assert.Equal(2, actualOrders.Count());
            foreach (var item in actualOrders)
            {
                Assert.Equal(OrderStatus.Processed, item.Status);
            }
        }

        [Fact]
        public async Task GetProcessedOrders_WithWithoutProcessedOrders_ShouldReturnEmptyCollection()
        {
            //Arrange
            Address address = new Address
            {
                Street = "Todor Kableshkov",
                City = new City { Name = "Plovdiv" }
            };

            var orders = new List<Order>
            {
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed},
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            //Act
            var actualOrders = this.orderService.GetProcessedOrders();

            //Assert
            Assert.NotEmpty(context.Orders);
            Assert.Empty(actualOrders);
        }

        [Fact]
        public async Task GetUnprocessedOrders_ShouldReturnCollection()
        {
            //Arrange
            Address address = new Address
            {
                Street = "Todor Kableshkov",
                City = new City { Name = "Plovdiv" }
            };

            var orders = new List<Order>
            {
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed},

            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            //Act
            var actualOrders = this.orderService.GetUnprocessedOrders();

            //Assert
            Assert.NotEmpty(actualOrders);
            Assert.Equal(2, actualOrders.Count());
            foreach (var item in actualOrders)
            {
                Assert.Equal(OrderStatus.Unprocessed, item.Status);
            }
        }

        [Fact]
        public async Task GetUnprocessedOrders_WithWithoutUnprocessedOrders_ShouldReturnEmptyCollection()
        {
            //Arrange
            Address address = new Address
            {
                Street = "Todor Kableshkov",
                City = new City { Name = "Plovdiv" }
            };

            var orders = new List<Order>
            {
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Delivered},
                new Order { DeliveryAddress = address, Status= OrderStatus.Processing},
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            //Act
            var actualOrders = this.orderService.GetUnprocessedOrders();

            //Assert
            Assert.NotEmpty(context.Orders);
            Assert.Empty(actualOrders);
        }

        [Fact]
        public async Task GetUserOrders_ShouldReturnCollection()
        {
            //Arrange
            Address address = new Address
            {
                Street = "Todor Kableshkov",
                City = new City { Name = "Plovdiv" }
            };
            string username = "Goshko";
            ShopUser user = new ShopUser
            {
                UserName = username
            };
            var orders = new List<Order>
            {
                new Order { DeliveryAddress = address, Status= OrderStatus.Processing, ShopUser = user},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed, ShopUser = user},
                new Order { DeliveryAddress = address, Status= OrderStatus.Processing, ShopUser = user},
                new Order { DeliveryAddress = address, Status= OrderStatus.Processed},
                new Order { DeliveryAddress = address, Status= OrderStatus.Unprocessed},
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            //Act
            var actualOrders = this.orderService.GetUserOrders(username);

            //Assert
            Assert.NotEmpty(actualOrders);
            Assert.Single(actualOrders);
        }

        [Fact]
        public void GetUserOrders_WithInvalidUsername_ShouldReturnEmptyCollection()
        {
            //Arrange
            string username = "Blago";
            //Act
            var actualOrders = this.orderService.GetUserOrders(username);

            //Assert
            Assert.Empty(actualOrders);
        }

        [Fact]
        public async Task OrderProductsByOrderId_ShouldReturnCollection()
        {
            //Arrange
            var order = new Order();
            var orderProducts = new List<OrderProduct>
            {
                new OrderProduct{ Product = new Product { Name = "USB 2.0" }, Order = order },
                new OrderProduct{ Product = new Product { Name = "USB 1.0" }, Order = order },
                new OrderProduct{ Product = new Product { Name = "USB 3.0" }, Order = order },
                new OrderProduct{ Product = new Product { Name = "USB 4.0" }, Order = new Order() },
            };

            await context.OrderProducts.AddRangeAsync(orderProducts);
            await context.SaveChangesAsync();

            //Act
            var actualOrders = this.orderService.OrderProductsByOrderId(order.Id);

            //Assert
            Assert.NotEmpty(actualOrders);
            Assert.Equal(3, actualOrders.Count());
        }

        [Fact]
        public void OrderProductsByOrderId_WithInvalidId_ShouldReturnEmptyCollection()
        {
            //Arrange
            int orderID = 123;
            //Act
            var actualOrders = this.orderService.OrderProductsByOrderId(orderID);

            //Assert
            Assert.Empty(actualOrders);
        }

        [Fact]
        public void GetOrderById_ShouldReturnOrder()
        {
            //Arrange
            string street = "Todor Kableshkov";
            string username = "Goshko";
            string company = "Ekont";

            Order order = new Order
            {
                DeliveryAddress = new Address
                {
                    Street = street,
                    City = new City { Name = "Plovdiv" }
                },
                ShopUser = new ShopUser
                {
                    UserName = username,
                    Company = new Company
                    {
                        Name = company
                    }
                }
            };

            context.Orders.Add(order);
            context.SaveChanges();
            
            //Act
            Order actualOrder = this.orderService.GetOrderById(order.Id);

            //Assert
            Assert.NotNull(actualOrder);
            Assert.Equal(street,actualOrder.DeliveryAddress.Street);
            Assert.Equal(username, actualOrder.ShopUser.UserName);
            Assert.Equal(company, actualOrder.ShopUser.Company.Name);
        }

        [Fact]
        public void GetOrderById_WithInvalidId_ShouldReturnNull()
        {
            //Arrange
            int id = 4;
           
            //Act
            Order actualOrder = this.orderService.GetOrderById(id);

            //Assert
            Assert.Null(actualOrder);
        }

        [Fact]
        public void GetUserOrderById_ShouldReturnUserOrder()
        {
            //Arrange
            var orderId = 3;
            var user = new ShopUser
            {
                UserName = "user@gmail.com",
                Orders = new List<Order>
                {
                    new Order { Id = 1, Status = OrderStatus.Processing },
                    new Order { Id = 2, Status = OrderStatus.Delivered },
                    new Order { Id = orderId, Status = OrderStatus.Unprocessed },
                }
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            this.mockUserService.Setup(x => x.GetUserByUsername(user.UserName)).Returns(user);

            //Act
            Order actualOrder = this.orderService.GetOrderById(3);

            //Assert
            Assert.NotNull(actualOrder);
            Assert.Equal(orderId, actualOrder.Id);
        }

        [Fact]
        public void GetUserOrderById_WithInvalidOrderId_ShouldReturnNull()
        {
            //Arrange
            var user = new ShopUser
            {
                UserName = "user@gmail.com",
                Orders = new List<Order>
                {
                    new Order { Id = 1, Status = OrderStatus.Processing },
                    new Order { Id = 2, Status = OrderStatus.Delivered },
                }
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();
            this.mockUserService.Setup(x => x.GetUserByUsername(user.UserName)).Returns(user);
           
            //Act
            Order actualOrder = this.orderService.GetUserOrderById(3,user.UserName);

            //Assert
            Assert.Null(actualOrder);
        }

        [Fact]
        public void GetUserOrderById_WithInvalidUsername_ShouldReturnNull()
        {
            //Arrange
            var user = new ShopUser
            {
                UserName = "user@gmail.com",
                Orders = new List<Order>
                {
                    new Order { Id = 1, Status = OrderStatus.Processing },
                    new Order { Id = 2, Status = OrderStatus.Delivered },
                }
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();
            this.mockUserService.Setup(x => x.GetUserByUsername(user.UserName)).Returns(user);

            //Act
            Order actualOrder = this.orderService.GetUserOrderById(1, "Todor");

            //Assert
            Assert.Null(actualOrder);
        }

        [Fact]
        public void GetProcessingOrder_ShouldReturnOrder()
        {
            //Arrange
            string street = "Todor Kableshkov";
            string username = "Goshko";

            var user = new ShopUser
            {
                UserName = username,
                Company = new Company
                {
                    Name = "Ekont"
                }
            };

            this.mockUserService.Setup(x => x.GetUserByUsername(username)).Returns(user);

            Order order = new Order
            {
                DeliveryAddress = new Address
                {
                    Street = street,
                    City = new City { Name = "Plovdiv" }
                },
                ShopUser = user,
                Status = OrderStatus.Processing               
            };

            context.Orders.Add(order);
            context.SaveChanges();

            //Act
            Order actualOrder = this.orderService.GetProcessingOrder(username);

            //Assert
            Assert.NotNull(actualOrder);
            Assert.Equal(street, actualOrder.DeliveryAddress.Street);
            Assert.Equal(OrderStatus.Processing, actualOrder.Status);
        }

        [Fact]
        public void GetProcessingOrder_WithInvalidUsername_ShouldReturnNull()
        {
            //Arrange
            string username = "Goshko";

            //Act
            Order actualOrder = this.orderService.GetProcessingOrder(username);

            //Assert
            Assert.Null(actualOrder);
        }

        [Theory]
        [InlineData(OrderStatus.Unprocessed)]
        [InlineData(OrderStatus.Delivered)]
        public void ProcessOrder_ShouldBeOk(OrderStatus status)
        {
            Order order = new Order
            {
                Status = status
            };

            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act
            this.orderService.ProcessOrder(order.Id);

            //Assert
            Assert.Equal(OrderStatus.Processed, order.Status);
        }

        [Fact]
        public void ProcessOrder_WithInvalidId_ShouldNotChange()
        {
            Order order = new Order
            {
                Status = OrderStatus.Unprocessed
            };

            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act
            this.orderService.ProcessOrder(3);

            //Assert
            Assert.Equal(OrderStatus.Unprocessed, order.Status);
        }

        [Fact]
        public void SetOrderDetails_ShouldReturnTrue()
        {
            //Arrange
            Order order = new Order { Status = OrderStatus.Processing };
            this.context.Orders.Add(order);

            Address address = new Address { Street = "str. Ivan Vazov" };
            this.context.Addresses.Add(address);

            this.context.SaveChanges();
            
            string recipient = "Ivan Ivanov";
            string recipientPhoneNumber = "09823222112";
            decimal deliveryPrice = 4.50M;

            //Act
            bool isSet = this.orderService.SetOrderDetails(order, recipient, recipientPhoneNumber, PaymentType.CashОnDelivery, address.Id, deliveryPrice);

            //Assert
            Assert.Equal(recipient, order.Recipient);
            Assert.Equal(recipientPhoneNumber, order.RecipientPhoneNumber);
            Assert.Equal(PaymentType.CashОnDelivery, order.PaymentType);
            Assert.Equal(address.Id, order.DeliveryAddressId);
        }

        [Theory]
        [InlineData("","   ")]
        [InlineData("  ","")]
        public void SetOrderDetails_WithInvalidParameters_ShouldReturnTrue(string fullName, 
            string phoneNumber)
        {
            Order order = new Order();
            
            //Act
            bool isSet = this.orderService.SetOrderDetails(order, fullName,phoneNumber, PaymentType.CashОnDelivery, 2, 4.50M);

            //Assert
            Assert.False(isSet);
        }
        
        [Fact]
        public void SetOrderStatusByInvoice_ShouldReturnTrue()
        {
            //Arrange 
            string invoiceNumber = "54168761233";
            string status = "Expired";

            Order order = new Order
            {
                InvoiceNumber = invoiceNumber
            };

            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act 
            bool isSet = this.orderService.SetOrderStatusByInvoice(invoiceNumber, status);

            //Assert
            Assert.True(isSet);
            Assert.Equal(PaymentStatus.Expired,order.PaymentStatus);
        }

        [Theory]
        [InlineData("","")]
        [InlineData("  ", "    ")]
        [InlineData("234252534", "Emparite")]
        public void SetOrderStatusByInvoice_WithInvalidParameters_ShouldReturnFalse(string invoiceNumber, string status)
        {
            //Arrange 
            Order order = new Order
            {
                InvoiceNumber = invoiceNumber,
                PaymentStatus = PaymentStatus.Unpaid
            };

            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act 
            bool isSet = this.orderService.SetOrderStatusByInvoice(invoiceNumber, status);

            //Assert
            Assert.False(isSet);
            Assert.Equal(PaymentStatus.Unpaid, order.PaymentStatus);
        }
        
        [Theory]
        [InlineData(false, 15, 1)]
        [InlineData(false, 30, 2)]
        [InlineData(false, 45, 3)]
        [InlineData(true, 25, 1)]
        [InlineData(true, 50, 2)]
        [InlineData(true, 75, 3)]
        public async Task CompleteProcessingOrder_ShouldCompleteOrder(bool isPartnerOrAdmin, decimal totalPrice, int quantity)
        {
            //Arrange
            ShopUser user = new ShopUser
            {
                UserName = "user@gmail.com",
                Orders = new List<Order> { new Order { Status = OrderStatus.Processing } },
                ShoppingCart =  new ShoppingCart()
            };

            var shoppingCartProducts = new List<ShoppingCartProduct>
            {
                new ShoppingCartProduct
                {
                     Product = new Product { Name = "USB 1.0", Price = 10, ParnersPrice = 15 },
                     ShoppingCart = user.ShoppingCart,
                     Quantity = quantity
                },
                new ShoppingCartProduct
                {
                     Product = new Product { Name = "USB 2.0", Price = 5, ParnersPrice = 10 },
                     ShoppingCart = user.ShoppingCart,
                     Quantity = quantity
                }
            };

            await this.context.Users.AddAsync(user);
            await this.context.ShoppingCartProducts.AddRangeAsync(shoppingCartProducts);
            await this.context.SaveChangesAsync();
            
            this.mockShoppingCartsService.Setup(s => s.GetAllShoppingCartProducts(user.UserName))
                                .Returns(shoppingCartProducts);
            
            this.mockUserService.Setup(u => u.GetUserByUsername(user.UserName))
                            .Returns(this.context.Users.FirstOrDefault(x => x.UserName == user.UserName));
            
            //Act
            this.orderService.CompleteProcessingOrder(user.UserName, isPartnerOrAdmin);

            Order order = this.context.Orders.FirstOrDefault(x => x.ShopUser.UserName == user.UserName);

            //Assert
            Assert.Equal(OrderStatus.Unprocessed, order.Status);
            Assert.Equal(2, order.OrderProducts.Count());
            Assert.Equal(PaymentStatus.Unpaid, order.PaymentStatus);
            Assert.Equal(totalPrice, order.TotalPrice);

        }
    }
}
