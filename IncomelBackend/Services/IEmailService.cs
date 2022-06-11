using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IEmailService
    {
        Tuple<bool,string> SendEmail(string to, string subject, string html);
    }
}
