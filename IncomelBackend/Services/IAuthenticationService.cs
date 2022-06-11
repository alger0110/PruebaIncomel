using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IAuthenticationService
    {
        Tuple<string, string> Authenticate(string username, string password);
    }
}
