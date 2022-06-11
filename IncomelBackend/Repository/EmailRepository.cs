using Data;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Serilog;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class EmailRepository : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EmailRepository(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public Tuple<bool, string> SendEmail(string to, string subject, string html)
        {
            /*
                **LOGEARSE CON LAS CREDENCIALES SMTP EN  https://ethereal.email/messages PARA EVALUAR LOS RESULTADOS DE EMISIÓN DE CORREO.
                *ESTA ES UNA APLICACIÓN EMULA EL ENVIO DEL CORREO ELECTRÓNICO.
                *
                * O bien Se puede Colocar las credenciales de SMTP
                * 
                * 
             */
            try
            {
                var email = new MimeMessage();

                email.From.Add( MailboxAddress.Parse(_emailConfig.Email) );
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };

                using (var stmp = new SmtpClient())
                {
                    stmp.Connect(_emailConfig.Host, _emailConfig.Port, SecureSocketOptions.StartTls);
                    stmp.Authenticate(_emailConfig.Email, _emailConfig.Password);
                    stmp.Send(email);
                    stmp.Disconnect(true);
                }

                return Tuple.Create(true, string.Empty);
            }
            catch (Exception ex)
            {
                Log.Error("EmailRepository - SendEmail: " + ex.Message, ex);
                return Tuple.Create(false, ex.Message);
                
            }
        }
    }
}
