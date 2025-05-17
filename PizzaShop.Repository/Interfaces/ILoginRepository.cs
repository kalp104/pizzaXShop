using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ILoginRepository
{
    Task<Account?> GetAccountByEmail(string email);
    Task<Account?> GetAccountById(int accountid);
    Task<string> UpdateAccount(Account account);
    Task UpdatePasswordResetRequest(PasswordResetRequest resetRequest);
    Task AddPasswordResetRequest(PasswordResetRequest resetRequest);
    Task<PasswordResetRequest?> GetTokenData(string token);
    Task<Account?> GetAccountByUsername(string username);
    Task<string> AddAccount(Account account);
}
