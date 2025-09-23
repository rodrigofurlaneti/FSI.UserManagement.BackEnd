using Application.DTOs;
using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (_userRepository.EmailExists(request.Email))
                throw new System.InvalidOperationException("E-mail already registered");

            var hash = HashPassword(request.Password);
            var user = new User(request.Name, request.Email, hash);
            _userRepository.Insert(user);
            var result =  new UserDto(user.Id, user.Name, user.Email, user.CreatedAt);
            return Task.FromResult(result);
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
            return Convert.ToHexString(bytes);
        }
    }
}
