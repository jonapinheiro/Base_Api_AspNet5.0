using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MkW_Models.Dto;
using MkW_Models.Models;
using MkW_Safety.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWeb_API.Controllers
{
    [ApiController]
    public class LoginCashController : ControllerBase
    {
        private IMapper _mapper;
        private LoginService _loginService;

        public LoginCashController(IMapper mapper , LoginService loginService)
        {
            _mapper = mapper;
            _loginService = loginService;
        }

        #region Get - Teste Usuario : Inserido 22/04/2023 - Jonathan Pinheiro 
        [HttpPost]
        [AllowAnonymous]
        [Route("/api/auth")]
        public async Task<IActionResult> Login([FromBody] EntityUserLogin loginDetalhes) // receber usuário e MD5(montado no cliente)  com (login+senha+st&ts0m) 
        {
            try
            {
                EntityUserLogin user = await _loginService.ValidarUsuario(loginDetalhes);

                if (user == null)
                    return BadRequest(new { Message = "Email e/ou senha está(ão) inválido(s)." });


                if (loginDetalhes.MD5 != user.MD5)
                    return BadRequest(new { Message = "Email e/ou senha está(ão) inválido(s)." });


                var token = _loginService.GerarToken(user);

                LoginDto _UserPage = _mapper.Map<LoginDto>(user);

                return Ok(new
                {
                    Token = token,
                    Usuario = _UserPage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Ocorreu um erro na aplicação, por favor tente novamente." });
            }


        }
        #endregion

    }
}
