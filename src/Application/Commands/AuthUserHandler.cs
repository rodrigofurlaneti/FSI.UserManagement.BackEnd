using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Repositories;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Application.Commands
{
    public class AuthUserHandler : IRequestHandler<AuthUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthUserHandler(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetByEmail(request.Email);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            var hash = HashPassword(request.Password);
            if (!string.Equals(hash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Invalid credentials");

            var jwtSettings = _configuration.GetSection("Jwt");

            var key = jwtSettings["Key"];
            if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("Jwt:Key missing");

            var issuer = jwtSettings["Issuer"] ?? "demo";
            var audience = jwtSettings["Audience"] ?? "demo";

            int expiresMinutes = 60;
            var expiresStr = jwtSettings["ExpiresMinutes"];
            if (!string.IsNullOrWhiteSpace(expiresStr))
            {
                if (!int.TryParse(expiresStr, out expiresMinutes))
                    expiresMinutes = 60;
            }

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(JwtRegisteredClaimNames.Email, user.Email) },
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
            return Convert.ToHexString(bytes);
        }
    }
}
