using JwtAPi.Models;
using JwtAPi.Repositories;
using JwtAPi.Services;
using JwtAPi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtAPi.Controllers
{
        [ApiController]
        [Route("[controller]")]
        public class LoginController : ControllerBase
        {
            [HttpPost]
            [Route("Login")]
            public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] LoginUser users)
            {
                // Recupera o usuario 
                var user = UserRepository.GetUser(users.UserName, users.Password);

                // Se o usuario ser Nulo e se ele existir gerar o Token
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado ou senha incorreta." });
                }
                else
                {
                //Gerar Token
                var token = TokenService.GenerateToken(user);
                //Gerar RefreshToken
                var refreshToken = TokenService.GenerateRefreshToken();
                //Salva o RefreshToken em uma fila 
                TokenService.SaveRefreshToken(user.UserName, refreshToken);

                   //Oculta o id
                    user.Id = 0;
                    //Oculta a senha 
                    user.Password = " ";

                    //Retorna os dados 
                    return new
                    {
                        user = user,
                        token = token,
                        refreshToken = refreshToken,
                    };
                }
            }

           [HttpPost]
           [Route("Refresh")]

           public IActionResult Refresh([FromBody] PostRefreshToken token)
           {
            
            var principal = TokenService.GetPrincipalFromExpiredToken(token.Token);
            var username = principal.Identity.Name;
            var saveRefreshToken = TokenService.GetRefreshToken(username);
            if (saveRefreshToken != token.RefreshToken)
                throw new SecurityTokenException("Invalid refresh token!");

            var newJwtToken = TokenService.GenerateToken(principal.Claims);
            var newRefreshToken = TokenService.GenerateRefreshToken();

            TokenService.DeleteRefreshToken(username, token.RefreshToken);
            TokenService.SaveRefreshToken(username, newRefreshToken);

            return new ObjectResult(new 
            {
                token= newJwtToken,
                refreshToken=newRefreshToken
            });
        }
        }
}
