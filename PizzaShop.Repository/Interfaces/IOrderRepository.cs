using System;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetOrderById(int orderid);
    Task<List<OrderItemMapping>?> GetAllOrderItemMappings();
    Task<List<Order>?> GetOrderByFilterDates(DateTime startDate, DateTime endDate);
    Task<List<OrderCutstomerViewModel>?> GetAllCustomerOrderMappingAsync();
    Task<OrderDetailsHelperViewModel?> GetOrderDetailsByOrderId(int orderId);
    Task<Feedback?> GetFeedbackByOrderId(int orderid);
    Task<List<OrdersTablesMapping>> GetTableByORderId(int orderid);
    Task<List<OrderItemModifiersMappingViewModel>> GetOIMByOrderId(int orderid);
    Task<List<TaxAmountViewModel>?> GetTaxByOrderId(int OrderId);
    Task<List<OrderTax>> GetTaxesByOrderId(int orderId);
    Task<string> AddOrder(Order order);
    Task<string> AddOrderItemModifiersMapping(OrderItemModifiersMapping order);
    Task<string> AddOrdersCustomersMapping(OrdersCustomersMapping order);
    Task<string> AddOrdersTablesMapping(OrdersTablesMapping order);
    Task DeleteOrderItemMapping(OrderItemMapping orderItemMapping);
    Task<OrdersTablesMapping?> GetOrderByTableId(int tableid);
    Task<List<OrderItemModifiersMapping>> GetAllOrderItemModifiersMapping();
    Task<List<OrderItemMapping>> GetAllOrderItemMapping();
    Task<List<OrderItemModifierJoinModelView>> GetAllOrderItemModifierJoin();
    Task<OrderItemMapping?> GetOrderItemMappingById(int OrderItemMappingId);
    Task<string> UpdateOrderItemMapping(OrderItemMapping order);
    Task<string> AddOrderItemMapping(OrderItemMapping order);
    Task<List<OrderItemMapping>?> GetOrderItemMappingByOrderId(int OrderId);
    Task<string> UpdateOrder(Order order);
    Task<string> UpdateOrderTaxMapping(OrderTaxMapping order);
    Task<string> AddOrderTaxMapping(OrderTaxMapping order);
    Task<string> DeleteOrderTaxMapping(OrderTaxMapping order);
    Task<List<OrderItemModifiersMapping>> GetOIMByOrderItemMappingId(int Orderitemmappingid);
    Task RemoveRangeOfOrderItemModifiersMappings(List<OrderItemModifiersMapping> mappings);
    Task<List<OrderTaxMapping>?> GetOrderTaxMappingByOrderId(int OrderId);




}
