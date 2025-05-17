using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzaShopContext _context;

    public OrderRepository(PizzaShopContext context)
    {
        _context = context;       
    }

    public async Task<Order?> GetOrderById(int orderid)
    {
        return await _context.Orders
                     .Where(x => x.Orderid == orderid)
                     .FirstOrDefaultAsync();
    }

    public async Task<List<OrderItemMapping>?> GetAllOrderItemMappings()
    {
        return await _context.OrderItemMappings
                     .ToListAsync();
    }

    public async Task<List<Order>?> GetOrderByFilterDates(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
                     .Where(o => o.Createdat >= startDate && o.Createdat <= endDate)
                     .ToListAsync();
    }


    public async Task<List<OrderCutstomerViewModel>?> GetAllCustomerOrderMappingAsync()
    {
            var result = await (
                from mapping in _context.OrdersCustomersMappings
                join c in _context.Customers on mapping.Customerid equals c.Customerid
                join o in _context.Orders on mapping.Orderid equals o.Orderid into ordersGroup
                from o in ordersGroup.DefaultIfEmpty()
                where c.Isdeleted == false
                where o == null || o.Isdeleted == false
                select new OrderCutstomerViewModel
                {
                    Orderid = o != null ? o.Orderid : 0,
                    Customerid = c.Customerid,
                    Orderdescription = o != null ? o.Orderdescription : null,
                    Createdat = o != null ? o.Createdat : null,
                    Status = o != null ? o.Status : 0,
                    Paymentmode = o != null ? o.Paymentmode : 0,
                    Ratings = o != null ? o.Ratings : null,
                    Totalamount = o != null ? o.Totalamount : 0,
                    Customername = c.Customername,
                    Customeremail = c.Customeremail,
                    Customerphone = c.Customerphone,
                }
            ).ToListAsync();
            return result;
    }

    public async Task<OrderDetailsHelperViewModel?> GetOrderDetailsByOrderId(int orderId)
    {
            var result = await (
                from mapping in _context.OrdersCustomersMappings
                join c in _context.Customers on mapping.Customerid equals c.Customerid
                join o in _context.Orders on mapping.Orderid equals o.Orderid
                where c.Isdeleted == false
                where o.Isdeleted == false
                where o.Orderid == orderId
                orderby o.Createdat descending
                select new OrderDetailsHelperViewModel
                {
                    Orderid = o.Orderid,
                    Customerid = c.Customerid,
                    Orderdescription = o.Orderdescription,
                    Createdat = o.Createdat,
                    CompletedAt = o.Completedat,
                    Status = o.Status,
                    Paymentmode = o.Paymentmode,
                    Ratings = o.Ratings,
                    Totalamount = o.Totalamount,
                    Customername = c.Customername,
                    Customeremail = c.Customeremail,
                    Customerphone = c.Customerphone,
                    Totalpersons = o.Totalpersons,
                }
            ).FirstOrDefaultAsync();
            return result;
    }


    public async Task<Feedback?> GetFeedbackByOrderId(int orderid)
    {
        var result = await _context.Feedbacks
                .Where(f => f.Orderid == orderid)
                .FirstOrDefaultAsync();

        return result;
    }

    public async Task<List<OrdersTablesMapping>> GetTableByORderId(int orderid)
    {
        return await _context.OrdersTablesMappings.Where(o => o.Orderid == orderid).ToListAsync();
    }

    public async Task<List<OrderItemModifiersMappingViewModel>> GetOIMByOrderId(int orderid)
    {
        List<OrderItemModifiersMappingViewModel> result = await (
                from oi in _context.OrderItemMappings
                join oim in _context.OrderItemModifiersMappings
                on oi.Orderitemmappingid equals oim.Orderitemmappingid
                where oi.Orderid == orderid
                select new OrderItemModifiersMappingViewModel
                {
                    Mappingid = oim.Mappingid,
                    Orderitemmappingid = oi.Orderitemmappingid,
                    orderId = oi.Orderid,
                    itemId = oi.Itemid,
                    Modifierid = oim.Modifierid,
                    status = oi.Status ?? 0,
                    totalQuantity = oi.Totalquantity ?? 0,
                    ReadyQuantity = oi.Readyquantity ?? 0

                }
        ).ToListAsync();
        return result;
    }

    public async Task<List<TaxAmountViewModel>?> GetTaxByOrderId(int OrderId)
    {
        List<TaxAmountViewModel>? result = await (
                from ot in _context.OrderTaxMappings
                where ot.Orderid == OrderId 
                select new TaxAmountViewModel
                {
                    TaxId = ot.Taxid,
                    TaxAmount = ot.Totalamount
                }
        ).ToListAsync();

        return result;
    }



    public async Task<string> AddOrder(Order order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> AddOrdersTablesMapping(OrdersTablesMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> AddOrdersCustomersMapping(OrdersCustomersMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<List<OrderTax>> GetTaxesByOrderId(int orderId){
            var result = await (
                from ot in _context.OrderTaxMappings
                join t in _context.TaxAndFees on ot.Taxid equals t.Taxid
                where ot.Orderid == orderId
                select new OrderTax
                {
                    TaxId = t.Taxid,
                    TaxName = t.Taxname,
                    TaxType = t.Taxtype,
                    TaxAmount = t.Taxamount,
                    Isenabled = t.Isenabled
                }
            ).ToListAsync();

            return result;
        }

    public async Task<OrdersTablesMapping?> GetOrderByTableId(int tableid)
    {
        OrdersTablesMapping? result = await (
                from ot in _context.OrdersTablesMappings
                join o in _context.Orders on ot.Orderid equals o.Orderid
                where ot.Tableid == tableid && o.Isdeleted == false
                orderby o.Createdat descending
                select new OrdersTablesMapping
                {
                    Orderid = o.Orderid,
                    Tableid = ot.Tableid
                }
        ).FirstOrDefaultAsync();
        return result;
    }


    public async Task<List<OrderItemModifiersMapping>> GetAllOrderItemModifiersMapping(){
        return await _context.OrderItemModifiersMappings.ToListAsync();
    }
    public async Task<List<OrderItemMapping>> GetAllOrderItemMapping(){
        return await _context.OrderItemMappings.ToListAsync();
    }

    public async Task<List<OrderItemModifierJoinModelView>> GetAllOrderItemModifierJoin()
    {
        List<OrderItemModifierJoinModelView>? result = await (
                from oi in _context.OrderItemMappings
                join oim in _context.OrderItemModifiersMappings
                    on oi.Orderitemmappingid equals oim.Orderitemmappingid
                select new OrderItemModifierJoinModelView
                {
                    OrderItemMappingId = oi.Orderitemmappingid,
                    orderId = oi.Orderid,
                    itemId = oi.Itemid,
                    modifierId = oim.Modifierid,
                }
        ).ToListAsync();
        return result;
    }


    public async Task<OrderItemMapping?> GetOrderItemMappingById(int OrderItemMappingId){
        return await _context.OrderItemMappings.Where(x => x.Orderitemmappingid == OrderItemMappingId).FirstOrDefaultAsync();
    }

    public async Task<string> UpdateOrderItemMapping(OrderItemMapping order)
    {
        try{
            _context.Update(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> UpdateOrderTaxMapping(OrderTaxMapping order)
    {
        try{
            _context.Update(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> AddOrderTaxMapping(OrderTaxMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> DeleteOrderTaxMapping(OrderTaxMapping order)
    {
        try{
            _context.Remove(order);
            await _context.SaveChangesAsync();
            return "removed";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> AddOrderItemMapping(OrderItemMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    
    public async Task<string> AddOrderItemModifiersMapping(OrderItemModifiersMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }


    public async Task<List<OrderItemMapping>?> GetOrderItemMappingByOrderId(int OrderId)
    {
        List<OrderItemMapping>? result = await (
                from oi in _context.OrderItemMappings
                where oi.Orderid == OrderId
                select new OrderItemMapping
                {
                    Orderitemmappingid = oi.Orderitemmappingid,
                    Itemid = oi.Itemid,
                    Orderid = oi.Orderid,
                    Totalquantity = oi.Totalquantity,
                    Readyquantity = oi.Readyquantity,
                    Status = oi.Status,
                    Specialmessage = oi.Specialmessage,
                    Createdat = oi.Createdat
                }
        ).ToListAsync();

        return result;
    }

    public async Task<string> UpdateOrder(Order order)
    {
        try{
            _context.Update(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<List<OrderItemModifiersMapping>> GetOIMByOrderItemMappingId(int Orderitemmappingid)
    {
        return await _context.OrderItemModifiersMappings.Where(o => o.Orderitemmappingid == Orderitemmappingid).ToListAsync();
    }

    public async Task RemoveRangeOfOrderItemModifiersMappings(List<OrderItemModifiersMapping> mappings)
    {
        _context.OrderItemModifiersMappings.RemoveRange(mappings);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderItemMapping(OrderItemMapping orderItemMapping)
    {
        _context.OrderItemMappings.Remove(orderItemMapping);
        await _context.SaveChangesAsync();
    }


    public async Task<List<OrderTaxMapping>?> GetOrderTaxMappingByOrderId(int OrderId)
    {
        List<OrderTaxMapping>? result = await (
                from ot in _context.OrderTaxMappings
                where ot.Orderid == OrderId
                select new OrderTaxMapping
                {
                    Ordertaxmappingid = ot.Ordertaxmappingid,
                    Taxid = ot.Taxid,
                    Orderid = ot.Orderid,
                    Totalamount = ot.Totalamount,
                    Createdat = ot.Createdat,
                    Editedat = ot.Editedat,
                    Createdbyid = ot.Createdbyid,
                }
        ).ToListAsync();

        return result;
    }

}
