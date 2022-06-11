using Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Repository
{
    public class AuthenticationRepository: IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly TokenManagement _tokenManagement;

        public AuthenticationRepository(IUserService userService, IOptions<TokenManagement> tokenManagement)
        {
            _userService = userService;
            _tokenManagement = tokenManagement.Value;
        }

        public Tuple<string, string> Authenticate(string username, string password)
        {
            try
            {
                var login = _userService.Login(username, password);
                if (!login.Item3)
                {
                    return Tuple.Create("", login.Item2);
                }
                List<Claim> claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.Sid, login.Item1.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, login.Item1.Name));
                claims.Add(new Claim(ClaimTypes.Surname, login.Item1.LastName));
                claims.Add(new Claim(ClaimTypes.Email, login.Item1.Email));

                foreach (var role in login.Item1.UserRole)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.AppRole.Name));
                    List<UserRoleOption> appUserRoleOptions = login.Item1.UserRoleOptions.Where(x => x.RoleId == role.RoleId).ToList();
                    foreach (var item in appUserRoleOptions)
                    {
                        claims.Add(new Claim(role.AppRole.Name, item.Option.Id.ToString()));
                    }
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var jwtToken = new JwtSecurityToken(_tokenManagement.Issuer, _tokenManagement.Audience, claims, expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration), signingCredentials: credentials);


                return Tuple.Create(new JwtSecurityTokenHandler().WriteToken(jwtToken), "");
            }
            catch (Exception ex)
            {
                Log.Error("AuthenticationRepository - Authenticate:" + ex.Message);
                return Tuple.Create("", ex.Message);
            }
        }
    }
}
