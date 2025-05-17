using System;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class FeedBackRepository : IFeedBackRepository
{
    private readonly PizzaShopContext _context;

    public FeedBackRepository(PizzaShopContext context)
    {
        _context = context;       
    }

    public async Task AddFeedback(Feedback feedback)
    {   
        _context.Add(feedback);
        await _context.SaveChangesAsync();
    } 

}
