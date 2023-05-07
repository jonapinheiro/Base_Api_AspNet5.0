using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MkW_Data.Conexao;
using MkW_Models.Dto;
using MkW_Models.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MkW_Safety.User
{
    public class LoginService
    {
        private IMapper _mapper;
        private IConfiguration _config;
        private Conexao _conn;

        public LoginService(IMapper mapper, IConfiguration Configuration, Conexao conn)
        {
            _mapper = mapper;
            _config = Configuration;
            _conn = conn;
        }

        #region Método para Validar Usuário no login - Inserido por Jonathan - Data: 22/04/2023
        public async Task<EntityUserLogin> ValidarUsuario(EntityUserLogin user)
        {
            EntityUserLogin log = new EntityUserLogin();

            UserDto _userR = _mapper.Map<UserDto>(user);

            //////// TESTE ////////
            log.Id = 1;
            log.Login = "jhonnyP";
            log.Senha = "AbcDeGHij";
            log.MD5 = "SDFSD45648D989G8F9D9816";
            log.Type = 4;
            return log;
            ///////// FIM TESTE //////////


            //try
            //{
            //    var json = JsonConvert.SerializeObject(_userR);
            //    var data = new StringContent(json, Encoding.UTF8, "application/json");

            //    var url = _conn.GetAddress() + "ServiceSAC.svc/Login";
            //    using var client = new HttpClient();

            //    var response = await client.PostAsync(url, data);

            //    string result = response.Content.ReadAsStringAsync().Result;
            //    Console.WriteLine(result);

            //    return JsonConvert.DeserializeObject<EntityUserLogin>(result);

            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}     

        }
        #endregion

        #region Gerar Token - Inserido por Jonathan - Data: Inicio Projeto API 
        public string GerarToken(EntityUserLogin user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Login.ToString()),
                    new Claim(ClaimTypes.Role, RoleFactory(user.Type))
                }),
                Expires = DateTime.UtcNow.AddHours(10),
                Issuer = issuer,
                Audience = audience,

                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        #endregion

        #region Role Factory Cria regra de restrição por Departamento - Inserido por Jonathan - Data: Inicio Projeto API ( Não Utilizado )
        // Montar a regra para Devolver as rotas autorizadas **** Autorização TESTE, somente por cargo ****
        private static string RoleFactory(int roleNumber)
        {
            switch (roleNumber)
            {
                case 1:
                    return "Admin";

                case 2:
                    return "Gerente";

                case 3:
                    return "Caixa";

                case 4:
                    return "Total";


                default:
                    throw new Exception();
            }
        }
        #endregion
    }
}
