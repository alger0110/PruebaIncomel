using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class UserRepository: IUserService
    {
        private readonly IncomelDBContext _incomelDBContext;
        private readonly PasswordHasher<User> _hasher;
        private readonly FrontendConfig _frontConfig;
        private readonly TemplateEmailsConfig _templateEmailsConfig;
        private readonly IEmailService _emailService;
        public UserRepository(IncomelDBContext incomelDBContext, IOptions<FrontendConfig> frontendConfig, IOptions<TemplateEmailsConfig> templateEmailsConfig, IEmailService emailService)
        {
            _incomelDBContext = incomelDBContext;
            _hasher = new PasswordHasher<User>();
            _frontConfig = frontendConfig.Value;
            _templateEmailsConfig = templateEmailsConfig.Value;
            _emailService = emailService;
        }

        public Tuple<User, string> Create(User user)
        {
            try
            {
                var cuser = this.GetUser(x => x.Email == user.Email);
                if (cuser != null)
                    return Tuple.Create(new User(), "Ya existe un usuario estas credenciales");

                user.PasswordHash = _hasher.HashPassword(user, user.PasswordHash);

                _incomelDBContext.Users.Add(user);
                _incomelDBContext.SaveChanges();

                user.PasswordHash = string.Empty;
                return Tuple.Create(user, "");
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - Create: " + ex.Message, ex);
                return Tuple.Create(new User(), ex.Message);
            }
        }

        public Tuple<bool, string> Delete(int userId)
        {
            try
            {
                var user = this.GetUser(x => x.Id == userId);

                if (!user.Item3)
                {
                    return Tuple.Create(false, user.Item2);
                }

                _incomelDBContext.Users.Remove(user.Item1);
                _incomelDBContext.SaveChanges();

                return Tuple.Create(true, "");
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - Delete: " + ex.Message, ex);
                return Tuple.Create(false, ex.Message);
            }
        }

        public Tuple<User, string, bool> Update(User user)
        {
            try
            {
                if (user.PasswordHash == null)
                {
                    var cuser = this.GetUser(x => x.Email == user.Email, true);
                    user.PasswordHash = cuser.Item1.PasswordHash;
                }
                else
                {
                    user.PasswordHash = _hasher.HashPassword(user, user.PasswordHash);
                }

                _incomelDBContext.Entry(user).State = EntityState.Modified;
                _incomelDBContext.SaveChanges();

                return Tuple.Create(user, "Usuario Actualizado con éxito.", true);
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - Update: " + ex.Message, ex);
                return Tuple.Create(new User(), ex.Message, false);
            }
        }

        public Tuple<User, string, bool> GetUser(Func<User, bool> filter, bool seePassword = false)
        {
            try
            {
                User user = _incomelDBContext.Users
                    .Include(x => x.UserRole).ThenInclude(x => x.AppRole)
                    .Include(x => x.UserRoleOptions).ThenInclude(x => x.Option)
                    .FirstOrDefault(filter);

                if (user == null)
                {
                    return Tuple.Create(new User(), "No existe un usuario con estas credenciales.", false);
                }

                if (!seePassword)
                {
                    user.PasswordHash = string.Empty;
                }

                return Tuple.Create(user, string.Empty, true);
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - GetAppUser: " + ex.Message, ex);
                return Tuple.Create(new User(), ex.Message, false);
            }
        }

        public Tuple<List<User>, string, bool> GetUsers(Func<User, bool> filter = null)
        {
            try
            {
                List<User> users = new List<User>();
                if (filter == null)
                {
                    users = _incomelDBContext.Users.ToList();
                }
                else
                {
                    users = _incomelDBContext.Users.Where(filter).ToList();
                }

                if (users == null)
                {
                    return Tuple.Create(new List<User>(), "No se encontrario registros", false);
                }

                return Tuple.Create(users, "", true);
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - GetAppUsers: " + ex.Message, ex);
                return Tuple.Create(new List<User>(), ex.Message, false);
            }
        }

        public Tuple<User, string, bool> Login(string username, string password)
        {
            try
            {
                var result = this.GetUser(x => x.Email == username, true);
                if (result.Item2 != string.Empty)
                {
                    return Tuple.Create(new User(), result.Item2, false);
                }
                User user = result.Item1;


                var ps = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (ps == PasswordVerificationResult.Failed)
                {
                    return Tuple.Create(new User(), "No existe un usuario con estas credenciales.", false);
                }
                user.PasswordHash = String.Empty;
                return Tuple.Create(user, "", true);
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - Login: " + ex.Message, ex);
                return Tuple.Create(new User(), ex.Message, false);
            }
        }

        public Tuple<string, bool> ForgotPasswordSend(string email, DateTime birthDate)
        {
            try
            {
                var user = this.GetUser(x => x.Email == email && x.BirthDate == birthDate);
                if (!user.Item3)
                {
                    return Tuple.Create(user.Item2, false);
                }

                PasswordToken passwordToken = new PasswordToken()
                {
                    Value = Guid.NewGuid().ToString(),
                    UserId = (int)user.Item1.Id,
                    Valid = true
                };

                _incomelDBContext.PasswordTokens.Add(passwordToken);
                _incomelDBContext.SaveChanges();

                string url = $"{_frontConfig.URL}/security/forgotPassword/{passwordToken.Value}";
                string name = $"{user.Item1.Name} {user.Item1.LastName}";


                var emails = _emailService.SendEmail(email, "Solicitud de Cambio de Contraseña", string.Format(_templateEmailsConfig.ForgotPassword, name, url, url));

                if (!emails.Item1)
                {
                    return Tuple.Create(emails.Item2, false);
                }

                return Tuple.Create("Se enviaron instrucciones a su Correo Electrónico.", true);

            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - ForgotPasswordSend: " + ex.Message, ex);
                return Tuple.Create( ex.Message, false);
            }
        }

        public Tuple<string, bool> ForgotPasswordValidation(string token)
        {
            try
            {
                var tokendb = _incomelDBContext.PasswordTokens.Include(x => x.User).FirstOrDefault(x => x.Value == token);

                if (tokendb == null)
                {
                    return Tuple.Create("No existe un registro con este valor.", false);
                }

                if (!tokendb.Valid)
                {
                    return Tuple.Create("Este url ya no es válido.", false);
                }

                tokendb.Valid = false;
                _incomelDBContext.Entry(tokendb).State = EntityState.Modified;
                _incomelDBContext.SaveChanges();

                return Tuple.Create(tokendb.User.Email, true);
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository - ForgotPasswordValidation: " + ex.Message, ex);
                return Tuple.Create(ex.Message, false);
            }
        }
    }
}
