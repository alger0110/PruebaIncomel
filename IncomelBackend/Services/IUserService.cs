using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IUserService
    {
        Tuple<List<User>, string, bool> GetUsers(Func<User, bool> filter = null);
        Tuple<User, string, bool> Update(User user);
        Tuple<User, string> Create(User user);
        Tuple<bool, string> Delete(int user);
        Tuple<User, string, bool> GetUser(Func<User, bool> filter, bool seePassword = false);
        Tuple<User, string, bool> Login(string username, string password);
        Tuple<string, bool> ForgotPasswordSend(string email, DateTime birthDate);
        Tuple<string, bool> ForgotPasswordValidation(string token);
    }
}
