using Domain.Configuration;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class DevAuthUserHandler : IRequestHandler<AuthUserCommand, string>
    {
        private readonly DevelopmentUserOptions _devUser;
        private readonly IConfiguration _configuration;

        public DevAuthUserHandler(IOptions<DevelopmentUserOptions> devUserOptions, IConfiguration configuration)
        {
            _devUser = devUserOptions.Value;
            _configuration = configuration;
        }

        public Task<string> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            // ⚡ 1. Validar o usuário vindo do appsettings.json
            if (request.Email != _devUser.Username || request.Password != _devUser.Password)
                throw new UnauthorizedAccessException("Invalid credentials");

            // ⚡ 2. Pegar configurações do JWT
            var jwtSettings = _configuration.GetSection("Jwt");

            var key = jwtSettings["Key"];
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Jwt:Key missing");

            var issuer = jwtSettings["Issuer"] ?? "demo";
            var audience = jwtSettings["Audience"] ?? "demo";

            int expiresMinutes = 60;
            var expiresStr = jwtSettings["ExpiresMinutes"];
            if (!string.IsNullOrWhiteSpace(expiresStr))
            {
                if (!int.TryParse(expiresStr, out expiresMinutes))
                    expiresMinutes = 60;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, request.Email) },
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(tokenString);
        }
    }
}