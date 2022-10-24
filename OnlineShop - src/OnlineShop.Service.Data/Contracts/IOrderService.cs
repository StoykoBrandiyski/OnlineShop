using OnlineShop.Models;
using OnlineShop.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IOrderService
    {
        Order CreateOrder(string username);

        Order GetProcessingOrder(string username);

        void CompleteProcessingOrder(string username, bool isPartnerOrAdmin);

        Order GetOrderById(int orderId);

        bool SetOrderDetails(Order order, string fullName, string phoneNumber, PaymentType paymentType, int deliveryAddressId, decimal deliveryPrice);

        void ProcessOrder(int id);

        IEnumerable<Order> GetUserOrders(string name);

        IEnumerable<Order> GetUnprocessedOrders();

        IEnumerable<Order> GetProcessedOrders();

        IEnumerable<Order> GetDeliveredOrders();

        void DeliverOrder(int id);

        IEnumerable<OrderProduct> OrderProductsByOrderId(int id);

        Order GetUserOrderById(int orderId, string username);

        bool SetOrderStatusByInvoice(string invoice, string status);
    }
}
