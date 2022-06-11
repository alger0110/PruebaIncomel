using IncomelBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading.Tasks;

namespace IncomelBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        public UserController(IAuthenticationService authenticationService, IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        [HttpPost, AllowAnonymous, Route("Authenticate")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Debe completar los campos solicitados." });
            }

            var login = _authenticationService.Authenticate(request.Username, request.Password);

            if (login.Item2 != "")
            {
                return BadRequest(new { Message = login.Item2 });
            }

            return Ok(new { Token = login.Item1 } );
        }

        [HttpPost, AllowAnonymous, Route("ForgotPasswordSend")]
        public IActionResult ForgotPasswordSend([FromBody] ForgotPasswordSendRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Debe completar los campos solicitados." });
            }

            var forgotpas = _userService.ForgotPasswordSend(request.Email, DateTime.Parse(request.BirthDate));


            if (!forgotpas.Item2)
            {
                return BadRequest(new { Message = forgotpas.Item1 });
            }


            return Ok(new { Message = forgotpas.Item1 });
        }

        [HttpPost, AllowAnonymous, Route("ForgotPasswordValidation")]
        public IActionResult ForgotPasswordValidation([FromBody] ForgotPasswordValidationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Debe completar los campos solicitados." });
            }

            var forgotpas = _userService.ForgotPasswordValidation(request.Token);


            if (!forgotpas.Item2)
            {
                return BadRequest(new { Message = forgotpas.Item1 });
            }


            return Ok(new { Message = forgotpas.Item1 });
        }

        [HttpPost, AllowAnonymous, Route("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Debe completar los campos solicitados." });
            }

            var user = _userService.GetUser(x => x.Email == request.Email);
            if (!user.Item3)
            {
                return BadRequest(new { Message = user.Item2 });
            }

            user.Item1.PasswordHash = request.NewPassword;

            var upuser = _userService.Update(user.Item1);

            if (!upuser.Item3)
            {
                return BadRequest(new { Message = upuser.Item2 });
            }


            return Ok(new { Message = upuser.Item2 });
        }
    }
}
