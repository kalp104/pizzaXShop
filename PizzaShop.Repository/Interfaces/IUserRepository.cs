using System;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByAccountId(int accountid);
    Task<User?> GetUserById(int userid);
    Task<List<Country>?> GetAllCountries();
    Task<Country?> GetCountryByid(int countryid);
    Task<List<State>?> GetAllStates(int countryid);
    Task<State?> GetStateByid(int stateid);
    Task<List<City>?> GetAllCities(int stateid);
    Task<City?> GetCityByid(int cityid);
    Task<string> UpdateUser(User user);
    Task<List<ItemWithCount>> GetTopItemsOrderedAsync(int range, DateTime startDate, DateTime endDate);
    Task<List<ItemWithCount>> GetLastItemsOrderedAsync(int range, DateTime startDate, DateTime endDate);
    Task<List<GraphRevenueViewModel>> GetGraphDataAsync(DateTime startDate, DateTime endDate);
    Task<List<GraphCustomerViewModel>> GetCustomerGraphDataAsync(DateTime startDate, DateTime endDate);
    Task<List<UserTableViewModel>?> UserDetailAsync();
    Task<string> AddUser(User user);
}   
