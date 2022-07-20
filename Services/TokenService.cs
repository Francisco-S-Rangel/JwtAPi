using JwtAPi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace JwtAPi.Services
{
    public class TokenService
    {
        public static bool ValidateLifetime { get; private set; }

        public static string GenerateToken(User user)
        {
            // O JwtSecurityTokenHandler() é a instancia do pacote using System.IdentityModel.Tokens.Jwt; que vai gerar o token
            var tokenHandler = new JwtSecurityTokenHandler();
            // O  Encoding.ASCII.GetBytes(Settings.SecretKey) é responsavel por incriptar a SecretKey ou seja senha se acesso do Token
            var key = Encoding.ASCII.GetBytes(Settings.SecretKey);
            // O SecutityTokenDescriptor é o Token por si só - as informações que o token terá
            var tokenDescription = new SecurityTokenDescriptor
            {
                //Os Claims(Afirmações) servem para vc definir os dados que estaram presentes dentro do Token 
                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name , user.UserName),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Role , user.Role),
                new Claim(ClaimTypes.Gender, user.Gender)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                // O algoritmo que o JWT será feito 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            // Por fim o metodo CreateToken cria o Token
            var token = tokenHandler.CreateToken(tokenDescription);
           
            // O retorno deve ser no formato do Token !!
            return tokenHandler.WriteToken(token);
        }

        public static string GenerateToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                /* Enquanto no metodo acima ele recebe o usuario, nesse metodo 
                 ele recebe as claims(Dados dentro do token)*/
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var refreshToken = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(refreshToken);
        }

        //Gera o refreshToken
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[256];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        
        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {

            var key = Encoding.ASCII.GetBytes(Settings.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
           
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidOperationException("Invalid token");

            return principal;
               
        }

        //Onde os Tokens serão armazenados 
        //É uma lista de dupla (string, string) ,pois seram armazenados o Username e o RefreshToken
        private static List<(string, string)> _refreshTokens = new();

        public static void SaveRefreshToken(string username, string refreshToken)
        {
            _refreshTokens.Add((username, refreshToken));
        }

        public static string GetRefreshToken(string username)
        {
            return _refreshTokens.FirstOrDefault(x => x.Item1 == username).Item2;
        }

        public static void DeleteRefreshToken(string username,string refreshToken)
        {
            var item = _refreshTokens.FirstOrDefault(x=>x.Item1 == username && x.Item2 == refreshToken);
            _refreshTokens.Remove(item);
        }

    }
}
